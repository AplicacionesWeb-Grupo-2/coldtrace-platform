using ColdTrace.Platform.IdentityAccess.Domain.Model.Aggregates;
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

        builder.UseSnakeCaseNamingConvention();
    }
}
