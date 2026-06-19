using ColdTrace.Platform.Alerts.Application.Internal.CommandServices;
using ColdTrace.Platform.Alerts.Application.Internal.QueryServices;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.Alerts.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.Internal.CommandServices;
using ColdTrace.Platform.MaintenanceManagement.Application.Internal.QueryServices;
using ColdTrace.Platform.MaintenanceManagement.Domain.Services;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Resources;
using ColdTrace.Platform.IdentityAccess.Application.Internal.CommandServices;
using ColdTrace.Platform.IdentityAccess.Application.Internal.QueryServices;
using ColdTrace.Platform.IdentityAccess.Domain.Services;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Shared.Domain.Repositories;
using ColdTrace.Platform.Shared.Interfaces.ASP.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;
using ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Localization;
using MySql.Data.MySqlClient;


var builder = WebApplication.CreateBuilder(args);

// Configure Lower Case URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Localization Configuration
builder.Services.AddLocalization();

// Configure Kebab Case Route Naming Convention
builder.Services.AddControllers(options => options.Conventions.Add(new KebabCaseRouteNamingConvention()))
    .AddDataAnnotationsLocalization();

// Register RFC 7807 ProblemDetails payloads for centralized exception handling.
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status is null or >= 500)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            context.ProblemDetails.Title ??= localizer["UnexpectedServerError"].Value;
            context.ProblemDetails.Detail ??= localizer["UnexpectedErrorProcessingRequest"].Value;
        }
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// Configure Database Context and route EF logs through the app logger pipeline.
builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
{
    var connectionStringTemplate = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionStringTemplate))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    var connectionString = Environment.ExpandEnvironmentVariables(connectionStringTemplate);
    if (string.IsNullOrWhiteSpace(connectionString))
        throw new InvalidOperationException("Database connection string is not set in the configuration.");

    options.UseMySQL(connectionString)
        .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>())
        .EnableDetailedErrors();

    if (builder.Environment.IsDevelopment())
        options.EnableSensitiveDataLogging();
});

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Alerts Bounded Context Injection Configuration
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IIncidentCommandService, IncidentCommandService>();
builder.Services.AddScoped<IIncidentQueryService, IncidentQueryService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

// Identity Access Bounded Context Injection Configuration
builder.Services.AddScoped<IOrganizationRepository, OrganizationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOrganizationCommandService, OrganizationCommandService>();
builder.Services.AddScoped<IOrganizationQueryService, OrganizationQueryService>();
builder.Services.AddScoped<IOrganizationSignUpCommandService, OrganizationSignUpCommandService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IRoleQueryService, RoleQueryService>();

// Asset Management Bounded Context Injection Configuration
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IGatewayRepository, GatewayRepository>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<ILocationCommandService, LocationCommandService>();
builder.Services.AddScoped<ILocationQueryService, LocationQueryService>();
builder.Services.AddScoped<IGatewayCommandService, GatewayCommandService>();
builder.Services.AddScoped<IGatewayQueryService, GatewayQueryService>();
builder.Services.AddScoped<IAssetCommandService, AssetCommandService>();
builder.Services.AddScoped<IAssetQueryService, AssetQueryService>();

// Maintenance Management Bounded Context Injection Configuration
builder.Services.AddScoped<IMaintenanceScheduleRepository, MaintenanceScheduleRepository>();
builder.Services.AddScoped<IMaintenanceScheduleCommandService, MaintenanceScheduleCommandService>();
builder.Services.AddScoped<IMaintenanceScheduleQueryService, MaintenanceScheduleQueryService>();
builder.Services.AddScoped<ITechnicalServiceRequestRepository, TechnicalServiceRequestRepository>();
builder.Services.AddScoped<ITechnicalServiceRequestCommandService, TechnicalServiceRequestCommandService>();
builder.Services.AddScoped<ITechnicalServiceRequestQueryService, TechnicalServiceRequestQueryService>();

var app = builder.Build();

// Apply pending migrations on startup (safe to call even when schema is up to date).
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    ApplyPendingMigrations(context);
}

app.UseExceptionHandler();

// Swagger UI is enabled in all environments for course delivery and manual API smoke validation.
app.UseSwagger();
app.UseSwaggerUI();

// Localization Configuration
string[] supportedCultures = ["en", "en-US", "es", "es-PE"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ApplyPendingMigrations(DbContext context)
{
    EnsureMySqlDatabaseExists(context);

    var appliedMigrations = GetAppliedMigrations(context);
    var currentMigration = appliedMigrations.LastOrDefault() ?? Migration.InitialDatabase;
    var migrator = context.GetService<IMigrator>();

    foreach (var migration in context.Database.GetMigrations().Where(migration => !appliedMigrations.Contains(migration)))
    {
        var script = migrator.GenerateScript(currentMigration, migration);
        ExecuteMigrationScript(context, script);
        appliedMigrations.Add(migration);
        currentMigration = migration;
    }
}

static void EnsureMySqlDatabaseExists(DbContext context)
{
    var connectionStringBuilder = new MySqlConnectionStringBuilder(context.Database.GetConnectionString());
    var database = connectionStringBuilder.Database;
    if (string.IsNullOrWhiteSpace(database)) return;

    connectionStringBuilder.Database = string.Empty;
    using var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = $"CREATE DATABASE IF NOT EXISTS `{database.Replace("`", "``")}`;";
    command.ExecuteNonQuery();
}

static List<string> GetAppliedMigrations(DbContext context)
{
    using var connection = new MySqlConnection(context.Database.GetConnectionString());
    connection.Open();

    if (!MigrationHistoryTableExists(connection)) return [];

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT `MigrationId` FROM `__EFMigrationsHistory` ORDER BY `MigrationId`;";

    using var reader = command.ExecuteReader();
    var migrations = new List<string>();
    while (reader.Read())
    {
        migrations.Add(reader.GetString(0));
    }

    return migrations;
}

static bool MigrationHistoryTableExists(MySqlConnection connection)
{
    using var command = connection.CreateCommand();
    command.CommandText =
        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '__EFMigrationsHistory';";
    return Convert.ToInt32(command.ExecuteScalar()) > 0;
}

static void ExecuteMigrationScript(DbContext context, string script)
{
    foreach (var statement in script.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        context.Database.ExecuteSqlRaw(statement);
    }
}
