using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class RestrictAssetDeletionDependencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_asset_settings_assets_asset_id",
                table: "asset_settings");

            migrationBuilder.DropForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "f_k_iot_devices_assets_asset_id",
                table: "iot_devices");

            migrationBuilder.DropForeignKey(
                name: "f_k_maintenance_schedules_assets_asset_id",
                table: "maintenance_schedules");

            migrationBuilder.DropForeignKey(
                name: "f_k_technical_service_requests_assets_asset_id",
                table: "technical_service_requests");

            migrationBuilder.AddForeignKey(
                name: "f_k_asset_settings_assets_asset_id",
                table: "asset_settings",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_iot_devices_assets_asset_id",
                table: "iot_devices",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_maintenance_schedules_assets_asset_id",
                table: "maintenance_schedules",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_technical_service_requests_assets_asset_id",
                table: "technical_service_requests",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_asset_settings_assets_asset_id",
                table: "asset_settings");

            migrationBuilder.DropForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents");

            migrationBuilder.DropForeignKey(
                name: "f_k_iot_devices_assets_asset_id",
                table: "iot_devices");

            migrationBuilder.DropForeignKey(
                name: "f_k_maintenance_schedules_assets_asset_id",
                table: "maintenance_schedules");

            migrationBuilder.DropForeignKey(
                name: "f_k_technical_service_requests_assets_asset_id",
                table: "technical_service_requests");

            migrationBuilder.AddForeignKey(
                name: "f_k_asset_settings_assets_asset_id",
                table: "asset_settings",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_incidents_assets_asset_id",
                table: "incidents",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_iot_devices_assets_asset_id",
                table: "iot_devices",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "f_k_maintenance_schedules_assets_asset_id",
                table: "maintenance_schedules",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_technical_service_requests_assets_asset_id",
                table: "technical_service_requests",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
