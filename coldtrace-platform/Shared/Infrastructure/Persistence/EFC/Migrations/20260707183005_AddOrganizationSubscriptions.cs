using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizationSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organization_subscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    organization_id = table.Column<int>(type: "int", nullable: false),
                    plan_code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    provider = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    provider_customer_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    provider_subscription_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    current_period_start = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    current_period_end = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    cancel_at_period_end = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    metadata = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_organization_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "f_k_organization_subscriptions_organizations_organization_id",
                        column: x => x.organization_id,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "i_x_organization_subscriptions_organization_id",
                table: "organization_subscriptions",
                column: "organization_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_organization_subscriptions_provider_customer_id",
                table: "organization_subscriptions",
                column: "provider_customer_id");

            migrationBuilder.CreateIndex(
                name: "i_x_organization_subscriptions_provider_subscription_id",
                table: "organization_subscriptions",
                column: "provider_subscription_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organization_subscriptions");
        }
    }
}
