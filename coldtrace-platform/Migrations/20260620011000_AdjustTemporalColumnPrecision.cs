using ColdTrace.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260620011000_AdjustTemporalColumnPrecision")]
    public partial class AdjustTemporalColumnPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            AlterDateTime(migrationBuilder, "asset_settings", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "asset_settings", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "assets", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "assets", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "gateways", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "gateways", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "detected_at", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "incidents", "acknowledged_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "resolved_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "last_notification_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "escalated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "incidents", "corrective_action_registered_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "iot_devices", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "iot_devices", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "locations", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "locations", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "scheduled_date", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "notifications", "delivered_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "notifications", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "notifications", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "organizations", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "organizations", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "reports", "period_start", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "reports", "period_end", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "reports", "generated_at", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "reports", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "reports", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "sensor_readings", "recorded_at", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "sensor_readings", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "sensor_readings", "updated_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "requested_at", "datetime(6)", false);
            AlterDateTime(migrationBuilder, "technical_service_requests", "closed_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "created_at", "datetime(6)", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "updated_at", "datetime(6)", true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            AlterDateTime(migrationBuilder, "asset_settings", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "asset_settings", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "assets", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "assets", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "gateways", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "gateways", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "detected_at", "datetime", false);
            AlterDateTime(migrationBuilder, "incidents", "acknowledged_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "resolved_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "last_notification_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "escalated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "incidents", "corrective_action_registered_at", "datetime", true);
            AlterDateTime(migrationBuilder, "iot_devices", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "iot_devices", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "locations", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "locations", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "scheduled_date", "datetime", false);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "maintenance_schedules", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "notifications", "delivered_at", "datetime", true);
            AlterDateTime(migrationBuilder, "notifications", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "notifications", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "organizations", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "organizations", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "reports", "period_start", "datetime", false);
            AlterDateTime(migrationBuilder, "reports", "period_end", "datetime", false);
            AlterDateTime(migrationBuilder, "reports", "generated_at", "datetime", false);
            AlterDateTime(migrationBuilder, "reports", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "reports", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "sensor_readings", "recorded_at", "datetime", false);
            AlterDateTime(migrationBuilder, "sensor_readings", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "sensor_readings", "updated_at", "datetime", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "requested_at", "datetime", false);
            AlterDateTime(migrationBuilder, "technical_service_requests", "closed_at", "datetime", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "created_at", "datetime", true);
            AlterDateTime(migrationBuilder, "technical_service_requests", "updated_at", "datetime", true);
        }

        private static void AlterDateTime(
            MigrationBuilder migrationBuilder,
            string table,
            string column,
            string type,
            bool nullable)
        {
            var nullability = nullable ? "NULL" : "NOT NULL";
            migrationBuilder.Sql($"ALTER TABLE `{table}` MODIFY `{column}` {type} {nullability};");
        }
    }
}
