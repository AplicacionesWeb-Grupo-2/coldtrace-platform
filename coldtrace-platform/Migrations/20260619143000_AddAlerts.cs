using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "incidents",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    asset_id = table.Column<int>(type: "int", nullable: true),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    reading_id = table.Column<int>(type: "int", nullable: true),
                    asset_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    device_name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    type = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    severity = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    status = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false),
                    value = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: true),
                    detected_at = table.Column<DateTimeOffset>(type: "datetime", nullable: false),
                    acknowledged_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    acknowledged_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    resolved_by = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    resolution_notes = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true),
                    last_notification_status = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true),
                    last_notification_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    notification_count = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_incidents", x => x.id);
                    table.ForeignKey(
                        name: "f_k_incidents_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "f_k_incidents_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    incident_id = table.Column<int>(type: "int", nullable: false),
                    channel = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    recipient = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true),
                    message = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: false),
                    status = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false),
                    delivered_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    failure_reason = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_notifications", x => x.id);
                    table.ForeignKey(
                        name: "f_k_notifications_incidents_incident_id",
                        column: x => x.incident_id,
                        principalTable: "incidents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_notifications_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_asset_id",
                table: "incidents",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_organization_id_asset_id",
                table: "incidents",
                columns: new[] { "organization_id", "asset_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_organization_id_reading_id",
                table: "incidents",
                columns: new[] { "organization_id", "reading_id" });

            migrationBuilder.CreateIndex(
                name: "i_x_incidents_organization_id_status",
                table: "incidents",
                columns: new[] { "organization_id", "status" });

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_incident_id",
                table: "notifications",
                column: "incident_id");

            migrationBuilder.CreateIndex(
                name: "i_x_notifications_organization_id_incident_id",
                table: "notifications",
                columns: new[] { "organization_id", "incident_id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "incidents");
        }
    }
}
