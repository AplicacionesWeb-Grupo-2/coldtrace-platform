using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
using ColdTrace.Platform.MaintenanceManagement.Domain.Model.Aggregates;
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
        
        builder.Entity<MaintenanceSchedule>().HasKey(s => s.Id);
        builder.Entity<MaintenanceSchedule>().Property(s => s.Id).IsRequired().ValueGeneratedOnAdd();
        builder.Entity<MaintenanceSchedule>().Property(s => s.Uuid).IsRequired().HasMaxLength(32);
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
        builder.Entity<TechnicalServiceRequest>().Property(r => r.Code).IsRequired().HasMaxLength(16);
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

        builder.UseSnakeCaseNamingConvention();
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
