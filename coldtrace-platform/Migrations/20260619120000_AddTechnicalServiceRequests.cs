using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddTechnicalServiceRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        name: "f_k_technical_service_requests_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_technical_service_requests_assets_asset_id",
                        column: x => x.asset_id,
                        principalTable: "assets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_technical_service_requests_organization_id",
                table: "technical_service_requests",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "i_x_technical_service_requests_asset_id",
                table: "technical_service_requests",
                column: "asset_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "technical_service_requests");
        }
    }
}
