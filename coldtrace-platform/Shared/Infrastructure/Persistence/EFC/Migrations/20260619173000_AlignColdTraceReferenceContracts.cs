using System;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260619173000_AlignColdTraceReferenceContracts")]
    public partial class AlignColdTraceReferenceContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "device_type",
                table: "iot_devices",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "model",
                table: "iot_devices",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "measurement_type",
                table: "iot_devices",
                type: "varchar(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "measurement_parameters",
                table: "iot_devices",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "reading_frequency_seconds",
                table: "iot_devices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "calibration_status",
                table: "iot_devices",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateOnly>(
                name: "last_calibration_date",
                table: "iot_devices",
                type: "date",
                nullable: false,
                defaultValueSql: "'1970-01-01'");

            migrationBuilder.AddColumn<DateOnly>(
                name: "next_calibration_date",
                table: "iot_devices",
                type: "date",
                nullable: false,
                defaultValueSql: "'1970-01-01'");

            migrationBuilder.DropColumn(
                name: "metric",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "value",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "unit",
                table: "sensor_readings");

            migrationBuilder.AddColumn<int>(
                name: "asset_id",
                table: "sensor_readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "gateway_id",
                table: "sensor_readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "sensor_readings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "temperature",
                table: "sensor_readings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "humidity",
                table: "sensor_readings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "out_of_range",
                table: "sensor_readings",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "motion_detected",
                table: "sensor_readings",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "image_captured",
                table: "sensor_readings",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "battery_level",
                table: "sensor_readings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "signal_strength",
                table: "sensor_readings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "escalated_at",
                table: "incidents",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "escalated_by",
                table: "incidents",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "escalation_reason",
                table: "incidents",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "corrective_action_registered_at",
                table: "incidents",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "corrective_action_registered_by",
                table: "incidents",
                type: "varchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "corrective_action",
                table: "incidents",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: true);

            migrationBuilder.RenameColumn(
                name: "temperature_min",
                table: "asset_settings",
                newName: "minimum_temperature")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "temperature_max",
                table: "asset_settings",
                newName: "maximum_temperature")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "humidity_min",
                table: "asset_settings",
                newName: "minimum_humidity")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "humidity_max",
                table: "asset_settings",
                newName: "maximum_humidity")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.AddColumn<string>(
                name: "uuid",
                table: "asset_settings",
                type: "varchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "asset_types",
                table: "asset_settings",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "iot_device_types",
                table: "asset_settings",
                type: "varchar(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<int>(
                name: "calibration_frequency_days",
                table: "asset_settings",
                type: "int",
                nullable: false,
                defaultValue: 30);

            migrationBuilder.AddColumn<string>(
                name: "temperature_unit",
                table: "asset_settings",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "C");

            migrationBuilder.AddColumn<string>(
                name: "humidity_unit",
                table: "asset_settings",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "%");

            migrationBuilder.AddColumn<string>(
                name: "weight_unit",
                table: "asset_settings",
                type: "varchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "kg");

            migrationBuilder.CreateIndex(
                name: "i_x_asset_settings_organization_id_asset_id",
                table: "asset_settings",
                columns: new[] { "organization_id", "asset_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_asset_id",
                table: "sensor_readings",
                column: "asset_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_gateway_id",
                table: "sensor_readings",
                column: "gateway_id");

            migrationBuilder.CreateIndex(
                name: "i_x_sensor_readings_organization_id_asset_id_recorded_at",
                table: "sensor_readings",
                columns: new[] { "organization_id", "asset_id", "recorded_at" });

            migrationBuilder.Sql("""
                UPDATE sensor_readings sr
                INNER JOIN iot_devices d ON sr.iot_device_id = d.id
                INNER JOIN assets a ON d.asset_id = a.id
                SET sr.asset_id = a.id,
                    sr.gateway_id = d.gateway_id,
                    sr.location_id = a.location_id;
                """);

            migrationBuilder.Sql("""
                DELETE FROM sensor_readings
                WHERE asset_id = 0
                   OR gateway_id = 0
                   OR location_id = 0;
                """);

            migrationBuilder.AddForeignKey(
                name: "f_k_sensor_readings_assets_asset_id",
                table: "sensor_readings",
                column: "asset_id",
                principalTable: "assets",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_sensor_readings_gateways_gateway_id",
                table: "sensor_readings",
                column: "gateway_id",
                principalTable: "gateways",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_sensor_readings_assets_asset_id",
                table: "sensor_readings");

            migrationBuilder.DropForeignKey(
                name: "f_k_sensor_readings_gateways_gateway_id",
                table: "sensor_readings");

            migrationBuilder.DropIndex(
                name: "i_x_asset_settings_organization_id_asset_id",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "uuid",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "asset_types",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "iot_device_types",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "calibration_frequency_days",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "temperature_unit",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "humidity_unit",
                table: "asset_settings");

            migrationBuilder.DropColumn(
                name: "weight_unit",
                table: "asset_settings");

            migrationBuilder.RenameColumn(
                name: "minimum_temperature",
                table: "asset_settings",
                newName: "temperature_min")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "maximum_temperature",
                table: "asset_settings",
                newName: "temperature_max")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "minimum_humidity",
                table: "asset_settings",
                newName: "humidity_min")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.RenameColumn(
                name: "maximum_humidity",
                table: "asset_settings",
                newName: "humidity_max")
                .Annotation("Relational:ColumnType", "double");

            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_asset_id",
                table: "sensor_readings");

            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_gateway_id",
                table: "sensor_readings");

            migrationBuilder.DropIndex(
                name: "i_x_sensor_readings_organization_id_asset_id_recorded_at",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "device_type",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "model",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "measurement_type",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "measurement_parameters",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "reading_frequency_seconds",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "calibration_status",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "last_calibration_date",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "next_calibration_date",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "asset_id",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "gateway_id",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "temperature",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "humidity",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "out_of_range",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "motion_detected",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "image_captured",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "battery_level",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "signal_strength",
                table: "sensor_readings");

            migrationBuilder.DropColumn(
                name: "escalated_at",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "escalated_by",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "escalation_reason",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "corrective_action_registered_at",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "corrective_action_registered_by",
                table: "incidents");

            migrationBuilder.DropColumn(
                name: "corrective_action",
                table: "incidents");

            migrationBuilder.AddColumn<string>(
                name: "metric",
                table: "sensor_readings",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "value",
                table: "sensor_readings",
                type: "decimal(12,4)",
                precision: 12,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "unit",
                table: "sensor_readings",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }
    }
}
