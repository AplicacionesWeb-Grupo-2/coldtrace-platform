using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeAlertLifecycleValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE incidents
                SET status = LOWER(status),
                    severity = LOWER(severity),
                    last_notification_status = LOWER(last_notification_status)
                """);
            migrationBuilder.Sql("UPDATE ai_resolution_plans SET status = LOWER(status)");
            migrationBuilder.Sql(
                "UPDATE notifications SET channel = LOWER(channel), status = LOWER(status)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE incidents
                SET status = UPPER(status),
                    severity = UPPER(severity),
                    last_notification_status = UPPER(last_notification_status)
                """);
            migrationBuilder.Sql("UPDATE ai_resolution_plans SET status = UPPER(status)");
            migrationBuilder.Sql(
                "UPDATE notifications SET channel = UPPER(channel), status = UPPER(status)");
        }
    }
}
