using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class RestrictIotDeviceDeletionWithReadings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_sensor_readings_iot_devices_iot_device_id",
                table: "sensor_readings");

            migrationBuilder.AddForeignKey(
                name: "f_k_sensor_readings_iot_devices_iot_device_id",
                table: "sensor_readings",
                column: "iot_device_id",
                principalTable: "iot_devices",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_sensor_readings_iot_devices_iot_device_id",
                table: "sensor_readings");

            migrationBuilder.AddForeignKey(
                name: "f_k_sensor_readings_iot_devices_iot_device_id",
                table: "sensor_readings",
                column: "iot_device_id",
                principalTable: "iot_devices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
