using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddSensorReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sensor_readings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    iot_device_id = table.Column<int>(type: "int", nullable: false),
                    metric = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    value = table.Column<decimal>(type: "decimal(12,4)", precision: 12, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_sensor_readings", x => x.id);
                    table.ForeignKey(
                        name: "f_k_sensor_readings_iot_devices_iot_device_id",
                        column: x => x.iot_device_id,
                        principalTable: "iot_devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_sensor_readings_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_iot_device_id",
                table: "sensor_readings",
                column: "iot_device_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_organization_id",
                table: "sensor_readings",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_organization_id_recorded_at",
                table: "sensor_readings",
                columns: new[] { "organization_id", "recorded_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensor_readings");
        }
    }
}
