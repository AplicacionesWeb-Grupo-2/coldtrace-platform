using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class AddBillingWebhookEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "billing_webhook_events",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    provider = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    event_id = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    event_type = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    organization_id = table.Column<int>(type: "int", nullable: true),
                    provider_customer_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    provider_subscription_id = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    processed_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    metadata = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_billing_webhook_events", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "uk_billing_webhook_events_provider_event",
                table: "billing_webhook_events",
                columns: new[] { "provider", "event_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "billing_webhook_events");
        }
    }
}
