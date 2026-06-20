using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260619191000_AlignSpringElementCollections")]
    public partial class AlignSpringElementCollections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "asset_settings_asset_types",
                columns: table => new
                {
                    asset_settings_id = table.Column<int>(type: "int", nullable: false),
                    asset_type = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_asset_settings_asset_types", x => new { x.asset_settings_id, x.asset_type });
                    table.ForeignKey(
                        name: "f_k_asset_settings_asset_types_asset_settings_id",
                        column: x => x.asset_settings_id,
                        principalTable: "asset_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "asset_settings_iot_device_types",
                columns: table => new
                {
                    asset_settings_id = table.Column<int>(type: "int", nullable: false),
                    iot_device_type = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_asset_settings_iot_device_types",
                        x => new { x.asset_settings_id, x.iot_device_type });
                    table.ForeignKey(
                        name: "f_k_asset_settings_iot_device_types_asset_settings_id",
                        column: x => x.asset_settings_id,
                        principalTable: "asset_settings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "iot_device_measurement_parameters",
                columns: table => new
                {
                    iot_device_id = table.Column<int>(type: "int", nullable: false),
                    measurement_parameter = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_iot_device_measurement_parameters",
                        x => new { x.iot_device_id, x.measurement_parameter });
                    table.ForeignKey(
                        name: "f_k_iot_device_measurement_parameters_iot_device_id",
                        column: x => x.iot_device_id,
                        principalTable: "iot_devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.Sql("""
                INSERT INTO asset_settings_asset_types (asset_settings_id, asset_type)
                SELECT s.id, jt.asset_type
                FROM asset_settings s
                JOIN JSON_TABLE(s.asset_types, '$[*]' COLUMNS (asset_type VARCHAR(255) PATH '$')) jt
                WHERE JSON_VALID(s.asset_types);
                """);

            migrationBuilder.Sql("""
                INSERT INTO asset_settings_iot_device_types (asset_settings_id, iot_device_type)
                SELECT s.id, jt.iot_device_type
                FROM asset_settings s
                JOIN JSON_TABLE(s.iot_device_types, '$[*]' COLUMNS (iot_device_type VARCHAR(255) PATH '$')) jt
                WHERE JSON_VALID(s.iot_device_types);
                """);

            migrationBuilder.Sql("""
                INSERT INTO iot_device_measurement_parameters (iot_device_id, measurement_parameter)
                SELECT d.id, jt.measurement_parameter
                FROM iot_devices d
                JOIN JSON_TABLE(d.measurement_parameters, '$[*]' COLUMNS (measurement_parameter VARCHAR(255) PATH '$')) jt
                WHERE JSON_VALID(d.measurement_parameters);
                """);

            migrationBuilder.DropColumn(
                name: "asset_types",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "iot_device_types",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "measurement_parameters",
                table: "iot_devices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "asset_types",
                table: "asset_settings",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "iot_device_types",
                table: "asset_settings",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "measurement_parameters",
                table: "iot_devices",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.Sql("""
                UPDATE asset_settings s
                SET asset_types = COALESCE((
                    SELECT JSON_ARRAYAGG(t.asset_type)
                    FROM asset_settings_asset_types t
                    WHERE t.asset_settings_id = s.id
                ), JSON_ARRAY());
                """);

            migrationBuilder.Sql("""
                UPDATE asset_settings s
                SET iot_device_types = COALESCE((
                    SELECT JSON_ARRAYAGG(t.iot_device_type)
                    FROM asset_settings_iot_device_types t
                    WHERE t.asset_settings_id = s.id
                ), JSON_ARRAY());
                """);

            migrationBuilder.Sql("""
                UPDATE iot_devices d
                SET measurement_parameters = COALESCE((
                    SELECT JSON_ARRAYAGG(p.measurement_parameter)
                    FROM iot_device_measurement_parameters p
                    WHERE p.iot_device_id = d.id
                ), JSON_ARRAY());
                """);

            migrationBuilder.DropTable(
                name: "asset_settings_asset_types");

            migrationBuilder.DropTable(
                name: "asset_settings_iot_device_types");

            migrationBuilder.DropTable(
                name: "iot_device_measurement_parameters");
        }
    }
}
