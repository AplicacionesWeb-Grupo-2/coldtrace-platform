using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
using ColdTrace.Platform.IdentityAccess.Domain.Model.ValueObjects;
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
