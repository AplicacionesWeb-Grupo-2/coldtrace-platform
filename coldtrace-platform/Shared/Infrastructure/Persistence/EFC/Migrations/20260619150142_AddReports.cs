using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "maintenance_schedules",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: false),
                    uuid = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    scheduled_date = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    frequency_days = table.Column<int>(type: "int", nullable: true),
                    responsible_user_id = table.Column<int>(type: "int", nullable: true),
                    observations = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    status = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_maintenance_schedules", x => x.id);
                    table.ForeignKey(
                        name: "f_k_maintenance_schedules_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_maintenance_schedules_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

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

            migrationBuilder.CreateTable(
                name: "technical_service_requests",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    code = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: false),
                    asset_location_id = table.Column<int>(type: "int", nullable: false),
                    asset_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    incident_id = table.Column<int>(type: "int", nullable: true),
                    issue_description = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: false),
                    priority = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    requested_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    closure_summary = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    evidence = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    closed_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_technical_service_requests", x => x.id);
                    table.ForeignKey(
                        name: "f_k_technical_service_requests_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_technical_service_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_maintenance_schedules_asset_id",
                table: "maintenance_schedules",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "i_x_maintenance_schedules_organization_id",
                table: "maintenance_schedules",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_reports_organization_id_generated_at",
                table: "reports",
                columns: new[] { "organization_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_reports_organization_id_uuid",
                table: "reports",
                columns: new[] { "organization_id", "uuid" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_technical_service_requests_asset_id",
                table: "technical_service_requests",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "i_x_technical_service_requests_organization_id",
                table: "technical_service_requests",
                column: "organization_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_incidents_organizations_organization_id",
                table: "incidents",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_notifications_incidents_incident_id",
                table: "notifications",
                column: "incident_id",
                principalTable: "incidents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_notifications_organizations_organization_id",
                table: "notifications",
                column: "organization_id",
                principalTable: "organizations",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "f_k_incidents_organizations_organization_id",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "f_k_notifications_incidents_incident_id",
                table: "notifications");

            migrationBuilder.DropForeignKey(
                name: "f_k_notifications_organizations_organization_id",
                table: "notifications");

            migrationBuilder.DropTable(
                name: "maintenance_schedules");

            migrationBuilder.DropTable(
                name: "reports");

            migrationBuilder.DropTable(
                name: "technical_service_requests");
        }
    }
}
