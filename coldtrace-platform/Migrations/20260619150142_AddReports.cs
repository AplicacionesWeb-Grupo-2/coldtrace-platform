using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "reports",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    uuid = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    type = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    asset_count = table.Column<int>(type: "int", nullable: false),
                    reading_count = table.Column<int>(type: "int", nullable: false),
                    out_of_range_reading_count = table.Column<int>(type: "int", nullable: false),
                    incident_count = table.Column<int>(type: "int", nullable: false),
                    open_incident_count = table.Column<int>(type: "int", nullable: false),
                    average_temperature = table.Column<double>(type: "double", nullable: true),
                    average_humidity = table.Column<double>(type: "double", nullable: true),
                    compliance_percentage = table.Column<double>(type: "double", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_reports", x => x.id);
                    table.ForeignKey(
                        name: "f_k_reports_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_reports_organization_id_generated_at",
                table: "reports",
                columns: new[] { "organization_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_reports_organization_id_uuid",
                table: "reports",
                columns: new[] { "organization_id", "uuid" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reports");
        }
    }
}
