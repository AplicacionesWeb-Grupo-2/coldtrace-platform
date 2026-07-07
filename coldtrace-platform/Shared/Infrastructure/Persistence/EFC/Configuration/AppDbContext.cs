using ColdTrace.Platform.Alerts.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Billing.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Reports.Domain.Model.Aggregates;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration.Extensions;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Application database context.
/// </summary>
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.AddInterceptors(new AuditableEntityInterceptor());
        base.OnConfiguring(builder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Organization>().HasKey(organization => organization.Id);
        builder.Entity<Organization>().Property(organization => organization.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Organization>().Property(organization => organization.LegalName).IsRequired().HasMaxLength(200);
        builder.Entity<Organization>().Property(organization => organization.CommercialName).IsRequired()
            .HasMaxLength(200);
        builder.Entity<Organization>().Property(organization => organization.TaxId).HasMaxLength(32);
        builder.Entity<Organization>().Property(organization => organization.ContactEmail).IsRequired()
            .HasMaxLength(256);
        builder.Entity<Organization>().HasIndex(organization => organization.ContactEmail).IsUnique();
        builder.Entity<Organization>().HasIndex(organization => organization.TaxId).IsUnique();

        builder.Entity<Role>().HasKey(role => role.Id);
        builder.Entity<Role>().Property(role => role.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Role>().Property(role => role.Name).IsRequired().HasMaxLength(64);
        builder.Entity<Role>().Property(role => role.Label).IsRequired().HasMaxLength(128);
        builder.Entity<Role>().HasIndex(role => role.Name).IsUnique();
        builder.Entity<Role>().HasData(
            new { Id = 1, Name = "super-admin", Label = "Super Administrator" },
            new { Id = 2, Name = "administrator", Label = "Administrator" },
            new { Id = 3, Name = "operations-manager", Label = "Operations Manager" },
            new { Id = 4, Name = "operator", Label = "Operator" },
            new { Id = 5, Name = "auditor", Label = "Auditor" });
        builder.Entity<Role>().OwnsMany(role => role.Permissions, permissionsBuilder =>
        {
            permissionsBuilder.ToTable("role_permissions");
            permissionsBuilder.WithOwner().HasForeignKey("RoleId");
            permissionsBuilder.HasKey("RoleId", nameof(Permission.Id));
            permissionsBuilder.Property("RoleId").HasColumnName("role_id").IsRequired();
            permissionsBuilder.Property(permission => permission.Id).HasColumnName("id").IsRequired()
                .ValueGeneratedNever();
            permissionsBuilder.Property(permission => permission.Resource).HasColumnName("resource").IsRequired()
                .HasMaxLength(64);
            permissionsBuilder.Property(permission => permission.Action).HasColumnName("action").IsRequired()
                .HasMaxLength(64);
            permissionsBuilder.Property(permission => permission.Description).HasColumnName("description")
                .IsRequired()
                .HasMaxLength(256);
            permissionsBuilder.HasData(
                PermissionSeed(1, 1, "administrators", "manage",
                    "roles-permissions.permissions.manage-administrators"),
                PermissionSeed(1, 2, "users", "manage", "roles-permissions.permissions.manage-users"),
                PermissionSeed(1, 3, "assets", "manage", "roles-permissions.permissions.manage-assets"),
                PermissionSeed(1, 4, "reports", "view", "roles-permissions.permissions.view-reports"),
                PermissionSeed(1, 5, "alerts", "update", "roles-permissions.permissions.resolve-alerts"),
                PermissionSeed(1, 6, "assets", "view", "roles-permissions.permissions.monitor-assets"),
                PermissionSeed(1, 7, "operations", "view", "roles-permissions.permissions.read-only"),
                PermissionSeed(2, 2, "users", "manage", "roles-permissions.permissions.manage-users"),
                PermissionSeed(2, 3, "assets", "manage", "roles-permissions.permissions.manage-assets"),
                PermissionSeed(2, 4, "reports", "view", "roles-permissions.permissions.view-reports"),
                PermissionSeed(2, 5, "alerts", "update", "roles-permissions.permissions.resolve-alerts"),
                PermissionSeed(2, 6, "assets", "view", "roles-permissions.permissions.monitor-assets"),
                PermissionSeed(2, 7, "operations", "view", "roles-permissions.permissions.read-only"),
                PermissionSeed(3, 3, "assets", "manage", "roles-permissions.permissions.manage-assets"),
                PermissionSeed(3, 5, "alerts", "update", "roles-permissions.permissions.resolve-alerts"),
                PermissionSeed(3, 4, "reports", "view", "roles-permissions.permissions.view-reports"),
                PermissionSeed(4, 6, "assets", "view", "roles-permissions.permissions.monitor-assets"),
                PermissionSeed(4, 5, "alerts", "update", "roles-permissions.permissions.resolve-alerts"),
                PermissionSeed(5, 4, "reports", "view", "roles-permissions.permissions.view-reports"),
                PermissionSeed(5, 7, "operations", "view", "roles-permissions.permissions.read-only"));
        });

        builder.Entity<User>().HasKey(user => user.Id);
        builder.Entity<User>().Property(user => user.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<User>().Property(user => user.Uuid).HasMaxLength(32);
        builder.Entity<User>().Property(user => user.OrganizationUserId);
        builder.Entity<User>().Property(user => user.FirstName).IsRequired().HasMaxLength(120);
        builder.Entity<User>().Property(user => user.LastName).IsRequired().HasMaxLength(120);
        builder.Entity<User>().Property(user => user.Email).IsRequired().HasMaxLength(256);
        builder.Entity<User>().HasIndex(user => user.Email).IsUnique();
        builder.Entity<User>()
            .HasOne(user => user.Organization)
            .WithMany()
            .HasForeignKey(user => user.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Entity<User>()
            .HasOne(user => user.Role)
            .WithMany()
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<OrganizationSubscription>().HasKey(subscription => subscription.Id);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.Id).IsRequired()
            .ValueGeneratedOnAdd();
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.PlanCode).IsRequired()
            .HasMaxLength(40);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.Status).IsRequired()
            .HasMaxLength(30);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.Provider).IsRequired()
            .HasMaxLength(30);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.ProviderCustomerId)
            .HasMaxLength(255);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.ProviderSubscriptionId)
            .HasMaxLength(255);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.CurrentPeriodStart);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.CurrentPeriodEnd);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.CancelAtPeriodEnd)
            .IsRequired();
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.Metadata)
            .HasMaxLength(2000);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.CreatedAt);
        builder.Entity<OrganizationSubscription>().Property(subscription => subscription.UpdatedAt);
        builder.Entity<OrganizationSubscription>()
            .HasIndex(subscription => subscription.OrganizationId)
            .IsUnique();
        builder.Entity<OrganizationSubscription>()
            .HasIndex(subscription => subscription.ProviderCustomerId);
        builder.Entity<OrganizationSubscription>()
            .HasIndex(subscription => subscription.ProviderSubscriptionId);
        builder.Entity<OrganizationSubscription>()
            .HasOne(subscription => subscription.Organization)
            .WithMany()
            .HasForeignKey(subscription => subscription.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_organization_subscriptions_organizations_organization_id");

        builder.Entity<Location>().HasKey(location => location.Id);
        builder.Entity<Location>().Property(location => location.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Location>().Property(location => location.Name).IsRequired().HasMaxLength(200);
        builder.Entity<Location>().Property(location => location.Type).IsRequired().HasMaxLength(80);
        builder.Entity<Location>().Property(location => location.Address).HasMaxLength(256);
        builder.Entity<Location>().Property(location => location.Description).HasMaxLength(512);
        builder.Entity<Location>().Property(location => location.Status).IsRequired().HasMaxLength(64);
        builder.Entity<Location>().Property(location => location.CreatedAt);
        builder.Entity<Location>().Property(location => location.UpdatedAt);
        builder.Entity<Location>()
            .HasIndex(location => new { location.OrganizationId, location.Name })
            .IsUnique();
        builder.Entity<Location>()
            .HasOne(location => location.Organization)
            .WithMany()
            .HasForeignKey(location => location.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_locations_organizations_organization_id");

        builder.Entity<Gateway>().HasKey(gateway => gateway.Id);
        builder.Entity<Gateway>().Property(gateway => gateway.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Gateway>().Property(gateway => gateway.Uuid).IsRequired().HasMaxLength(128);
        builder.Entity<Gateway>().Property(gateway => gateway.Name).IsRequired().HasMaxLength(200);
        builder.Entity<Gateway>().Property(gateway => gateway.Network).IsRequired().HasMaxLength(120);
        builder.Entity<Gateway>().Property(gateway => gateway.Status).IsRequired().HasMaxLength(64);
        builder.Entity<Gateway>().Property(gateway => gateway.CreatedAt);
        builder.Entity<Gateway>().Property(gateway => gateway.UpdatedAt);
        builder.Entity<Gateway>()
            .HasIndex(gateway => new { gateway.OrganizationId, gateway.Uuid })
            .IsUnique();
        builder.Entity<Gateway>()
            .HasOne(gateway => gateway.Organization)
            .WithMany()
            .HasForeignKey(gateway => gateway.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_gateways_organizations_organization_id");
        builder.Entity<Gateway>()
            .HasOne(gateway => gateway.Location)
            .WithMany()
            .HasForeignKey(gateway => gateway.LocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_gateways_locations_location_id");

        //HU-48
        
        builder.Entity<Asset>().HasKey(asset => asset.Id);
        builder.Entity<Asset>().Property(asset => asset.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Asset>().Property(asset => asset.Uuid).IsRequired().HasMaxLength(128);
        builder.Entity<Asset>().Property(asset => asset.Type).IsRequired().HasMaxLength(80);
        builder.Entity<Asset>().Property(asset => asset.Name).IsRequired().HasMaxLength(200);
        builder.Entity<Asset>().Property(asset => asset.Capacity).IsRequired();
        builder.Entity<Asset>().Property(asset => asset.Description).HasMaxLength(512);
        builder.Entity<Asset>().Property(asset => asset.Status).IsRequired().HasMaxLength(64);
        builder.Entity<Asset>().Property(asset => asset.CreatedAt);
        builder.Entity<Asset>().Property(asset => asset.UpdatedAt);
        builder.Entity<Asset>()
            .HasIndex(asset => new { asset.OrganizationId, asset.Uuid })
            .IsUnique();
        builder.Entity<Asset>()
            .HasOne(asset => asset.Organization)
            .WithMany()
            .HasForeignKey(asset => asset.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_assets_organizations_organization_id");
        builder.Entity<Asset>()
            .HasOne(asset => asset.Location)
            .WithMany()
            .HasForeignKey(asset => asset.LocationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_assets_locations_location_id");

        builder.Entity<AssetSettings>().HasKey(settings => settings.Id);
        builder.Entity<AssetSettings>().Property(settings => settings.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AssetSettings>().Property(settings => settings.Uuid).IsRequired().HasMaxLength(128);
        builder.Entity<AssetSettings>().Ignore(settings => settings.AssetTypes);
        builder.Entity<AssetSettings>().Ignore(settings => settings.IotDeviceTypes);
        builder.Entity<AssetSettings>().OwnsMany(settings => settings.AssetTypeEntries, assetTypesBuilder =>
        {
            assetTypesBuilder.ToTable("asset_settings_asset_types");
            assetTypesBuilder.Property<int>("AssetSettingsId").HasColumnName("asset_settings_id").IsRequired();
            assetTypesBuilder.Property(entry => entry.AssetType).HasColumnName("asset_type").IsRequired()
                .HasMaxLength(255);
            assetTypesBuilder.HasKey("AssetSettingsId", nameof(AssetSettingsAssetType.AssetType));
            assetTypesBuilder.WithOwner()
                .HasForeignKey("AssetSettingsId")
                .HasConstraintName("f_k_asset_settings_asset_types_asset_settings_id");
        });
        builder.Entity<AssetSettings>().OwnsMany(settings => settings.IotDeviceTypeEntries, iotDeviceTypesBuilder =>
        {
            iotDeviceTypesBuilder.ToTable("asset_settings_iot_device_types");
            iotDeviceTypesBuilder.Property<int>("AssetSettingsId").HasColumnName("asset_settings_id").IsRequired();
            iotDeviceTypesBuilder.Property(entry => entry.IotDeviceType).HasColumnName("iot_device_type")
                .IsRequired().HasMaxLength(255);
            iotDeviceTypesBuilder.HasKey("AssetSettingsId", nameof(AssetSettingsIotDeviceType.IotDeviceType));
            iotDeviceTypesBuilder.WithOwner()
                .HasForeignKey("AssetSettingsId")
                .HasConstraintName("f_k_asset_settings_iot_device_types_asset_settings_id");
        });
        builder.Entity<AssetSettings>().Property(settings => settings.MinimumTemperature).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.MaximumTemperature).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.MinimumHumidity).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.MaximumHumidity).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.CalibrationFrequencyDays).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.TemperatureUnit).IsRequired().HasMaxLength(16);
        builder.Entity<AssetSettings>().Property(settings => settings.HumidityUnit).IsRequired().HasMaxLength(16);
        builder.Entity<AssetSettings>().Property(settings => settings.WeightUnit).IsRequired().HasMaxLength(16);
        builder.Entity<AssetSettings>().Property(settings => settings.ReadingFrequencySeconds).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.AlertThresholdMinutes).IsRequired();
        builder.Entity<AssetSettings>().Property(settings => settings.CreatedAt);
        builder.Entity<AssetSettings>().Property(settings => settings.UpdatedAt);
        builder.Entity<AssetSettings>()
            .HasIndex(settings => new { settings.OrganizationId, settings.AssetId })
            .IsUnique();
        builder.Entity<AssetSettings>()
            .HasOne(settings => settings.Organization)
            .WithMany()
            .HasForeignKey(settings => settings.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_asset_settings_organizations_organization_id");
        builder.Entity<AssetSettings>()
            .HasOne(settings => settings.Asset)
            .WithMany()
            .HasForeignKey(settings => settings.AssetId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_asset_settings_assets_asset_id");

        builder.Entity<IotDevice>().HasKey(device => device.Id);
        builder.Entity<IotDevice>().Property(device => device.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<IotDevice>().Property(device => device.Uuid).IsRequired().HasMaxLength(128);
        builder.Entity<IotDevice>().Property(device => device.Name).IsRequired().HasMaxLength(200);
        builder.Entity<IotDevice>().Property(device => device.DeviceType).IsRequired().HasMaxLength(80);
        builder.Entity<IotDevice>().Property(device => device.Model).IsRequired().HasMaxLength(120);
        builder.Entity<IotDevice>().Property(device => device.MeasurementType).IsRequired().HasMaxLength(120);
        builder.Entity<IotDevice>().Ignore(device => device.MeasurementParameters);
        builder.Entity<IotDevice>().OwnsMany(device => device.MeasurementParameterEntries, parametersBuilder =>
        {
            parametersBuilder.ToTable("iot_device_measurement_parameters");
            parametersBuilder.Property<int>("IotDeviceId").HasColumnName("iot_device_id").IsRequired();
            parametersBuilder.Property(entry => entry.MeasurementParameter).HasColumnName("measurement_parameter")
                .IsRequired().HasMaxLength(255);
            parametersBuilder.HasKey("IotDeviceId", nameof(IotDeviceMeasurementParameter.MeasurementParameter));
            parametersBuilder.WithOwner()
                .HasForeignKey("IotDeviceId")
                .HasConstraintName("f_k_iot_device_measurement_parameters_iot_device_id");
        });
        builder.Entity<IotDevice>().Property(device => device.ReadingFrequencySeconds).IsRequired();
        builder.Entity<IotDevice>().Property(device => device.Status).IsRequired().HasMaxLength(64);
        builder.Entity<IotDevice>().Property(device => device.CalibrationStatus).IsRequired().HasMaxLength(64);
        builder.Entity<IotDevice>().Property(device => device.LastCalibrationDate)
            .HasConversion(
                date => date.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
            .HasColumnType("date")
            .IsRequired();
        builder.Entity<IotDevice>().Property(device => device.NextCalibrationDate)
            .HasConversion(
                date => date.ToDateTime(TimeOnly.MinValue),
                dateTime => DateOnly.FromDateTime(dateTime))
            .HasColumnType("date")
            .IsRequired();
        builder.Entity<IotDevice>().Property(device => device.CreatedAt);
        builder.Entity<IotDevice>().Property(device => device.UpdatedAt);
        builder.Entity<IotDevice>()
            .HasIndex(device => new { device.OrganizationId, device.Uuid })
            .IsUnique();
        builder.Entity<IotDevice>()
            .HasIndex(device => device.GatewayId);
        builder.Entity<IotDevice>()
            .HasIndex(device => device.AssetId);
        builder.Entity<IotDevice>()
            .HasOne(device => device.Organization)
            .WithMany()
            .HasForeignKey(device => device.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_iot_devices_organizations_organization_id");
        builder.Entity<IotDevice>()
            .HasOne(device => device.Gateway)
            .WithMany()
            .HasForeignKey(device => device.GatewayId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_iot_devices_gateways_gateway_id");
        builder.Entity<IotDevice>()
            .HasOne(device => device.Asset)
            .WithMany()
            .HasForeignKey(device => device.AssetId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("f_k_iot_devices_assets_asset_id");

        builder.Entity<SensorReading>().HasKey(reading => reading.Id);
        builder.Entity<SensorReading>().Property(reading => reading.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<SensorReading>().Property(reading => reading.AssetId).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.GatewayId).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.LocationId).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.Temperature).HasPrecision(8, 2);
        builder.Entity<SensorReading>().Property(reading => reading.Humidity).HasPrecision(8, 2);
        builder.Entity<SensorReading>().Property(reading => reading.OutOfRange).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.RecordedAt).IsRequired();
        builder.Entity<SensorReading>().Property(reading => reading.MotionDetected);
        builder.Entity<SensorReading>().Property(reading => reading.ImageCaptured);
        builder.Entity<SensorReading>().Property(reading => reading.BatteryLevel);
        builder.Entity<SensorReading>().Property(reading => reading.SignalStrength);
        builder.Entity<SensorReading>().Property(reading => reading.CreatedAt);
        builder.Entity<SensorReading>().Property(reading => reading.UpdatedAt);
        builder.Entity<SensorReading>()
            .HasIndex(reading => reading.OrganizationId);
        builder.Entity<SensorReading>()
            .HasIndex(reading => reading.AssetId);
        builder.Entity<SensorReading>()
            .HasIndex(reading => reading.IotDeviceId);
        builder.Entity<SensorReading>()
            .HasIndex(reading => reading.GatewayId);
        builder.Entity<SensorReading>()
            .HasIndex(reading => new { reading.OrganizationId, reading.RecordedAt });
        builder.Entity<SensorReading>()
            .HasIndex(reading => new { reading.OrganizationId, reading.AssetId, reading.RecordedAt });
        builder.Entity<SensorReading>()
            .HasOne(reading => reading.Organization)
            .WithMany()
            .HasForeignKey(reading => reading.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_sensor_readings_organizations_organization_id");
        builder.Entity<SensorReading>()
            .HasOne(reading => reading.Asset)
            .WithMany()
            .HasForeignKey(reading => reading.AssetId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_sensor_readings_assets_asset_id");
        builder.Entity<SensorReading>()
            .HasOne(reading => reading.IotDevice)
            .WithMany()
            .HasForeignKey(reading => reading.IotDeviceId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_sensor_readings_iot_devices_iot_device_id");
        builder.Entity<SensorReading>()
            .HasOne(reading => reading.Gateway)
            .WithMany()
            .HasForeignKey(reading => reading.GatewayId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_sensor_readings_gateways_gateway_id");
        builder.Entity<Incident>().HasKey(incident => incident.Id);
        builder.Entity<Incident>().Property(incident => incident.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Incident>().Property(incident => incident.DeviceId);
        builder.Entity<Incident>().Property(incident => incident.ReadingId);
        builder.Entity<Incident>().Property(incident => incident.AssetName).HasMaxLength(200);
        builder.Entity<Incident>().Property(incident => incident.DeviceName).HasMaxLength(200);
        builder.Entity<Incident>().Property(incident => incident.Type).IsRequired().HasMaxLength(120);
        builder.Entity<Incident>().Property(incident => incident.Severity).IsRequired().HasMaxLength(32);
        builder.Entity<Incident>().Property(incident => incident.Status).IsRequired().HasMaxLength(64);
        builder.Entity<Incident>().Property(incident => incident.Value).HasMaxLength(120);
        builder.Entity<Incident>().Property(incident => incident.DetectedAt).IsRequired();
        builder.Entity<Incident>().Property(incident => incident.AcknowledgedAt);
        builder.Entity<Incident>().Property(incident => incident.AcknowledgedBy).HasMaxLength(256);
        builder.Entity<Incident>().Property(incident => incident.EscalatedAt);
        builder.Entity<Incident>().Property(incident => incident.EscalatedBy).HasMaxLength(256);
        builder.Entity<Incident>().Property(incident => incident.EscalationReason).HasMaxLength(1024);
        builder.Entity<Incident>().Property(incident => incident.CorrectiveActionRegisteredAt);
        builder.Entity<Incident>().Property(incident => incident.CorrectiveActionRegisteredBy).HasMaxLength(256);
        builder.Entity<Incident>().Property(incident => incident.CorrectiveAction).HasMaxLength(1024);
        builder.Entity<Incident>().Property(incident => incident.ResolvedAt);
        builder.Entity<Incident>().Property(incident => incident.ResolvedBy).HasMaxLength(256);
        builder.Entity<Incident>().Property(incident => incident.ResolutionNotes).HasMaxLength(1024);
        builder.Entity<Incident>().Property(incident => incident.LastNotificationStatus).HasMaxLength(32);
        builder.Entity<Incident>().Property(incident => incident.LastNotificationAt);
        builder.Entity<Incident>().Property(incident => incident.NotificationCount).IsRequired();
        builder.Entity<Incident>().Property(incident => incident.CreatedAt);
        builder.Entity<Incident>().Property(incident => incident.UpdatedAt);
        builder.Entity<Incident>()
            .HasIndex(incident => new { incident.OrganizationId, incident.Status });
        builder.Entity<Incident>()
            .HasIndex(incident => new { incident.OrganizationId, incident.AssetId });
        builder.Entity<Incident>()
            .HasIndex(incident => new { incident.OrganizationId, incident.ReadingId });
        builder.Entity<Incident>()
            .HasOne(incident => incident.Organization)
            .WithMany()
            .HasForeignKey(incident => incident.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_incidents_organizations_organization_id");
        builder.Entity<Incident>()
            .HasOne(incident => incident.Asset)
            .WithMany()
            .HasForeignKey(incident => incident.AssetId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("f_k_incidents_assets_asset_id");

        builder.Entity<AiResolutionPlan>().HasKey(plan => plan.Id);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<AiResolutionPlan>().Property(plan => plan.Status).IsRequired().HasMaxLength(32);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.Summary).IsRequired().HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ProbableCause).IsRequired().HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().OwnsMany(plan => plan.RecommendedSteps, stepsBuilder =>
        {
            stepsBuilder.ToTable("ai_resolution_plan_steps");
            stepsBuilder.Property<int>("AiResolutionPlanId").HasColumnName("ai_resolution_plan_id").IsRequired();
            stepsBuilder.Property(step => step.Sequence).HasColumnName("sequence").IsRequired().ValueGeneratedNever();
            stepsBuilder.Property(step => step.Action).HasColumnName("action").IsRequired().HasMaxLength(1024);
            stepsBuilder.Property(step => step.Rationale).HasColumnName("rationale").IsRequired().HasMaxLength(1024);
            stepsBuilder.Property(step => step.ExpectedOutcome).HasColumnName("expected_outcome").IsRequired()
                .HasMaxLength(1024);
            stepsBuilder.HasKey("AiResolutionPlanId", nameof(AiResolutionPlanStep.Sequence));
            stepsBuilder.WithOwner()
                .HasForeignKey("AiResolutionPlanId")
                .HasConstraintName("f_k_ai_resolution_plan_steps_ai_resolution_plan_id");
        });
        builder.Entity<AiResolutionPlan>().Property(plan => plan.CorrectiveActionDraft).IsRequired()
            .HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ResolutionNotesDraft).IsRequired()
            .HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.EscalationRecommended).IsRequired();
        builder.Entity<AiResolutionPlan>().Property(plan => plan.EscalationUrgency).IsRequired().HasMaxLength(32);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.EscalationReason).IsRequired().HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().OwnsMany(plan => plan.RequiredEvidenceItems, evidenceBuilder =>
        {
            evidenceBuilder.ToTable("ai_resolution_plan_required_evidence");
            evidenceBuilder.Property<int>("Id").HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
            evidenceBuilder.Property<int>("AiResolutionPlanId").HasColumnName("ai_resolution_plan_id").IsRequired();
            evidenceBuilder.Property(item => item.Value).HasColumnName("value").IsRequired().HasMaxLength(512);
            evidenceBuilder.HasKey("Id");
            evidenceBuilder.WithOwner()
                .HasForeignKey("AiResolutionPlanId")
                .HasConstraintName("f_k_ai_resolution_plan_required_evidence_ai_resolution_plan_id");
        });
        builder.Entity<AiResolutionPlan>().OwnsMany(plan => plan.UncertaintyNoteItems, notesBuilder =>
        {
            notesBuilder.ToTable("ai_resolution_plan_uncertainty_notes");
            notesBuilder.Property<int>("Id").HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
            notesBuilder.Property<int>("AiResolutionPlanId").HasColumnName("ai_resolution_plan_id").IsRequired();
            notesBuilder.Property(item => item.Value).HasColumnName("value").IsRequired().HasMaxLength(512);
            notesBuilder.HasKey("Id");
            notesBuilder.WithOwner()
                .HasForeignKey("AiResolutionPlanId")
                .HasConstraintName("f_k_ai_resolution_plan_uncertainty_notes_ai_resolution_plan_id");
        });
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ModelProvider).IsRequired().HasMaxLength(64);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ModelName).IsRequired().HasMaxLength(128);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ProviderMetadata).HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.GeneratedAt).IsRequired();
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ApprovedAt);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.ApprovedBy).HasMaxLength(256);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.RejectedAt);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.RejectedBy).HasMaxLength(256);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.RejectionReason).HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.FinalCorrectiveAction).HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.FinalResolutionNotes).HasMaxLength(1024);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.CreatedAt);
        builder.Entity<AiResolutionPlan>().Property(plan => plan.UpdatedAt);
        builder.Entity<AiResolutionPlan>()
            .HasIndex(plan => new { plan.OrganizationId, plan.IncidentId, plan.GeneratedAt });
        builder.Entity<AiResolutionPlan>()
            .HasIndex(plan => new { plan.OrganizationId, plan.Status });
        builder.Entity<AiResolutionPlan>()
            .HasOne(plan => plan.Organization)
            .WithMany()
            .HasForeignKey(plan => plan.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_ai_resolution_plans_organizations_organization_id");
        builder.Entity<AiResolutionPlan>()
            .HasOne(plan => plan.Incident)
            .WithMany()
            .HasForeignKey(plan => plan.IncidentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_ai_resolution_plans_incidents_incident_id");

        builder.Entity<Notification>().HasKey(notification => notification.Id);
        builder.Entity<Notification>().Property(notification => notification.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Notification>().Property(notification => notification.Channel).IsRequired().HasMaxLength(32);
        builder.Entity<Notification>().Property(notification => notification.Recipient).HasMaxLength(256);
        builder.Entity<Notification>().Property(notification => notification.Message).IsRequired().HasMaxLength(512);
        builder.Entity<Notification>().Property(notification => notification.Status).IsRequired().HasMaxLength(32);
        builder.Entity<Notification>().Property(notification => notification.DeliveredAt);
        builder.Entity<Notification>().Property(notification => notification.FailureReason).HasMaxLength(512);
        builder.Entity<Notification>().Property(notification => notification.CreatedAt);
        builder.Entity<Notification>().Property(notification => notification.UpdatedAt);
        builder.Entity<Notification>()
            .HasIndex(notification => new { notification.OrganizationId, notification.IncidentId });
        builder.Entity<Notification>()
            .HasOne(notification => notification.Organization)
            .WithMany()
            .HasForeignKey(notification => notification.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("f_k_notifications_organizations_organization_id");
        builder.Entity<Notification>()
            .HasOne(notification => notification.Incident)
            .WithMany()
            .HasForeignKey(notification => notification.IncidentId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_notifications_incidents_incident_id");

        builder.Entity<Report>().HasKey(report => report.Id);
        builder.Entity<Report>().Property(report => report.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<Report>().Property(report => report.Uuid).IsRequired().HasMaxLength(128);
        builder.Entity<Report>().Property(report => report.Type).IsRequired().HasMaxLength(80);
        builder.Entity<Report>().Property(report => report.Title).IsRequired().HasMaxLength(200);
        builder.Entity<Report>().Property(report => report.PeriodStart).IsRequired();
        builder.Entity<Report>().Property(report => report.PeriodEnd).IsRequired();
        builder.Entity<Report>().Property(report => report.GeneratedAt).IsRequired();
        builder.Entity<Report>().Property(report => report.AssetCount).IsRequired();
        builder.Entity<Report>().Property(report => report.ReadingCount).IsRequired();
        builder.Entity<Report>().Property(report => report.OutOfRangeReadingCount).IsRequired();
        builder.Entity<Report>().Property(report => report.IncidentCount).IsRequired();
        builder.Entity<Report>().Property(report => report.OpenIncidentCount).IsRequired();
        builder.Entity<Report>().Property(report => report.AverageTemperature);
        builder.Entity<Report>().Property(report => report.AverageHumidity);
        builder.Entity<Report>().Property(report => report.CompliancePercentage);
        builder.Entity<Report>().Property(report => report.CreatedAt);
        builder.Entity<Report>().Property(report => report.UpdatedAt);
        builder.Entity<Report>()
            .HasIndex(report => new { report.OrganizationId, report.GeneratedAt });
        builder.Entity<Report>()
            .HasIndex(report => new { report.OrganizationId, report.Uuid })
            .IsUnique();
        builder.Entity<Report>()
            .HasOne(report => report.Organization)
            .WithMany()
            .HasForeignKey(report => report.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_reports_organizations_organization_id");
        
        builder.Entity<MaintenanceSchedule>().HasKey(s => s.Id);
        builder.Entity<MaintenanceSchedule>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MaintenanceSchedule>().Property(s => s.Uuid).IsRequired().HasMaxLength(255);
        builder.Entity<MaintenanceSchedule>().Property(s => s.ScheduledDate).IsRequired();
        builder.Entity<MaintenanceSchedule>().Property(s => s.FrequencyDays);
        builder.Entity<MaintenanceSchedule>().Property(s => s.ResponsibleUserId);
        builder.Entity<MaintenanceSchedule>().Property(s => s.Observations).HasMaxLength(512);
        builder.Entity<MaintenanceSchedule>().Property(s => s.Status).IsRequired().HasMaxLength(64);
        builder.Entity<MaintenanceSchedule>().Property(s => s.CreatedAt);
        builder.Entity<MaintenanceSchedule>().Property(s => s.UpdatedAt);
        builder.Entity<MaintenanceSchedule>()
            .HasOne(s => s.Organization)
            .WithMany()
            .HasForeignKey(s => s.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_maintenance_schedules_organizations_organization_id");
        builder.Entity<MaintenanceSchedule>()
            .HasOne(s => s.Asset)
            .WithMany()
            .HasForeignKey(s => s.AssetId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_maintenance_schedules_assets_asset_id");

        builder.Entity<TechnicalServiceRequest>().HasKey(r => r.Id);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Code).IsRequired().HasMaxLength(255);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.AssetLocationId).IsRequired();
        builder.Entity<TechnicalServiceRequest>().Property(r => r.AssetName).HasMaxLength(200);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.IncidentId);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.IssueDescription).IsRequired().HasMaxLength(1024);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Priority).IsRequired().HasMaxLength(32);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Status).IsRequired().HasMaxLength(64);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.RequestedBy).HasMaxLength(256);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.RequestedAt).IsRequired();
        builder.Entity<TechnicalServiceRequest>().Property(r => r.ClosedAt);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.ClosureSummary).HasMaxLength(1024);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Evidence).HasMaxLength(1024);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.ClosedBy).HasMaxLength(256);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.CreatedAt);
        builder.Entity<TechnicalServiceRequest>().Property(r => r.UpdatedAt);
        builder.Entity<TechnicalServiceRequest>()
            .HasOne(r => r.Organization)
            .WithMany()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_technical_service_requests_organizations_organization_id");
        builder.Entity<TechnicalServiceRequest>()
            .HasOne(r => r.Asset)
            .WithMany()
            .HasForeignKey(r => r.AssetId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("f_k_technical_service_requests_assets_asset_id");

        UseDateTimeOffsetPrecision(builder);
        builder.UseSnakeCaseNamingConvention();
    }

    private static void UseDateTimeOffsetPrecision(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes().SelectMany(entityType => entityType.GetProperties()))
        {
            if (property.ClrType == typeof(DateTimeOffset) ||
                property.ClrType == typeof(DateTimeOffset?))
                property.SetColumnType("datetime(6)");
        }
    }

    private static object PermissionSeed(
        int roleId,
        int id,
        string resource,
        string action,
        string description) =>
        new
        {
            RoleId = roleId,
            Id = id,
            Resource = resource,
            Action = action,
            Description = description
        };

}
