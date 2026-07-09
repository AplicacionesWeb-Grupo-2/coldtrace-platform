using ColdTrace.Platform.AiAssistance.Application.Internal.CommandServices;
using ColdTrace.Platform.AiAssistance.Application.Internal.QueryServices;
using ColdTrace.Platform.AiAssistance.Domain.Services;
using ColdTrace.Platform.AiAssistance.Infrastructure.Configuration;
using ColdTrace.Platform.AiAssistance.Infrastructure.Providers;
using ColdTrace.Platform.Alerts.Application.Internal.CommandServices;
using ColdTrace.Platform.Alerts.Application.Internal.QueryServices;
using ColdTrace.Platform.Alerts.Domain.Repositories;
using ColdTrace.Platform.Alerts.Domain.Services;
using ColdTrace.Platform.Alerts.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Billing.Application.ACL;
using ColdTrace.Platform.Billing.Application.Internal.CommandServices;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Checkout;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Portal;
using ColdTrace.Platform.Billing.Application.Internal.OutboundServices.Webhook;
using ColdTrace.Platform.Billing.Application.Internal.QueryServices;
using ColdTrace.Platform.Billing.Application.Internal.Services;
using ColdTrace.Platform.Billing.Domain.Model.Commands;
using ColdTrace.Platform.Billing.Domain.Repositories;
using ColdTrace.Platform.Billing.Domain.Services;
using ColdTrace.Platform.Billing.Infrastructure.Configuration;
using ColdTrace.Platform.Billing.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Billing.Infrastructure.Stripe;
using ColdTrace.Platform.Billing.Interfaces.ACL;
using ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Monitoring.Application.Internal.CommandServices;
using ColdTrace.Platform.Monitoring.Application.Internal.QueryServices;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Services;
using ColdTrace.Platform.Monitoring.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Application.Internal.CommandServices;
using ColdTrace.Platform.MaintenanceManagement.Application.Internal.QueryServices;
using ColdTrace.Platform.MaintenanceManagement.Domain.Services;
using ColdTrace.Platform.MaintenanceManagement.Domain.Repositories;
using ColdTrace.Platform.MaintenanceManagement.Infrastructure.Persistence.EFC.Repositories;
using ColdTrace.Platform.Reports.Application.Internal.CommandServices;
using ColdTrace.Platform.Reports.Application.Internal.QueryServices;
using ColdTrace.Platform.Reports.Domain.Repositories;
using ColdTrace.Platform.Reports.Domain.Services;
using ColdTrace.Platform.Reports.Infrastructure.Persistence.EFC.Repositories;
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
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;


var builder = WebApplication.CreateBuilder(args);
const string corsPolicyName = "ColdTraceCorsPolicy";

// Configure Lower Case URLs
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure CORS for browser-based clients.
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, policy =>
    {
        var allowedOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
            ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (allowedOrigins is { Length: > 0 })
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
            return;
        }

        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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
        if (context.Exception is PlanLimitExceededException planLimitExceededException)
        {
            var localizer = context.HttpContext.RequestServices.GetRequiredService<IStringLocalizer<SharedResource>>();
            var message = localizer[planLimitExceededException.MessageResourceKey].Value;

            context.HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
            context.ProblemDetails.Status = StatusCodes.Status409Conflict;
            context.ProblemDetails.Title = message;
            context.ProblemDetails.Detail = message;
            context.ProblemDetails.AppendPlanEntitlementProperties(planLimitExceededException.Entitlement);
            return;
        }

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

// Billing Bounded Context Injection Configuration
builder.Services.AddOptions<BillingOptions>()
    .Bind(builder.Configuration.GetSection(BillingOptions.SectionName))
    .PostConfigure(options => options.ExpandEnvironmentVariables());
builder.Services.AddScoped<IOrganizationSubscriptionRepository, OrganizationSubscriptionRepository>();
builder.Services.AddScoped<IBillingWebhookEventRepository, BillingWebhookEventRepository>();
builder.Services.AddScoped<ISubscriptionPlanQueryService, SubscriptionPlanQueryService>();
builder.Services.AddScoped<OrganizationSubscriptionUsageService>();
builder.Services.AddScoped<EntitlementPolicyService>();
builder.Services.AddScoped<ICheckoutSessionProviderService, StripeCheckoutSessionProviderService>();
builder.Services.AddScoped<IPortalSessionProviderService, StripePortalSessionProviderService>();
builder.Services.AddScoped<IBillingWebhookProviderService, StripeBillingWebhookProviderService>();
builder.Services.AddScoped<IOrganizationSubscriptionCommandService, OrganizationSubscriptionCommandService>();
builder.Services.AddScoped<IOrganizationSubscriptionQueryService, OrganizationSubscriptionQueryService>();
builder.Services.AddScoped<IBillingCheckoutSessionCommandService, BillingCheckoutSessionCommandService>();
builder.Services.AddScoped<IBillingPortalSessionCommandService, BillingPortalSessionCommandService>();
builder.Services.AddScoped<IBillingWebhookCommandService, BillingWebhookCommandService>();
builder.Services.AddScoped<ISubscriptionBillingContextFacade, SubscriptionBillingContextFacade>();

// AI Assistance Bounded Context Injection Configuration
builder.Services.AddOptions<AiOptions>()
    .Bind(builder.Configuration.GetSection(AiOptions.SectionName))
    .PostConfigure(options => options.ExpandEnvironmentVariables());
builder.Services.AddSingleton<IChatClient>(serviceProvider =>
    AiChatClientFactory.Create(serviceProvider.GetRequiredService<IOptions<AiOptions>>().Value));
builder.Services.AddScoped<IAiChatClientAdapter, ServiceProviderAiChatClientAdapter>();
builder.Services.AddScoped<IAiProviderStatusQueryService, AiProviderStatusQueryService>();
builder.Services.AddScoped<IAiStructuredOutputService, MicrosoftExtensionsAiStructuredOutputService>();

// Alerts Bounded Context Injection Configuration
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IAiResolutionPlanRepository, AiResolutionPlanRepository>();
builder.Services.AddScoped<IIncidentCommandService, IncidentCommandService>();
builder.Services.AddScoped<IAiResolutionPlanCommandService, AiResolutionPlanCommandService>();
builder.Services.AddScoped<IIncidentQueryService, IncidentQueryService>();
builder.Services.AddScoped<IAiResolutionPlanQueryService, AiResolutionPlanQueryService>();
builder.Services.AddScoped<INotificationQueryService, NotificationQueryService>();

// Reports Bounded Context Injection Configuration
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportCommandService, ReportCommandService>();
builder.Services.AddScoped<IReportAiSummaryCommandService, ReportAiSummaryCommandService>();
builder.Services.AddScoped<IReportQueryService, ReportQueryService>();

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
builder.Services.AddScoped<IAssetSettingsRepository, AssetSettingsRepository>();
builder.Services.AddScoped<ILocationCommandService, LocationCommandService>();
builder.Services.AddScoped<ILocationQueryService, LocationQueryService>();
builder.Services.AddScoped<IGatewayCommandService, GatewayCommandService>();
builder.Services.AddScoped<IGatewayQueryService, GatewayQueryService>();
builder.Services.AddScoped<IAssetCommandService, AssetCommandService>();
builder.Services.AddScoped<IAssetQueryService, AssetQueryService>(); //HU-48
builder.Services.AddScoped<IAssetSettingsCommandService, AssetSettingsCommandService>();
builder.Services.AddScoped<IAssetSettingsQueryService, AssetSettingsQueryService>();
builder.Services.AddScoped<IIotDeviceRepository, IotDeviceRepository>();
builder.Services.AddScoped<IIotDeviceCommandService, IotDeviceCommandService>();
builder.Services.AddScoped<IIotDeviceQueryService, IotDeviceQueryService>();

// Monitoring Bounded Context Injection Configuration
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<ISensorReadingCommandService, SensorReadingCommandService>();
builder.Services.AddScoped<ISensorReadingQueryService, SensorReadingQueryService>();
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
    var organizationSubscriptionCommandService =
        services.GetRequiredService<IOrganizationSubscriptionCommandService>();
    await organizationSubscriptionCommandService.Handle(new SeedBaseOrganizationSubscriptionsCommand());
}

app.UseExceptionHandler();

// Swagger UI is enabled in all environments for course delivery and manual API smoke validation.
app.UseSwagger();
app.UseSwaggerUI();

// Localization Configuration
string[] supportedCultures = ["en", "es"];
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
localizationOptions.ApplyCurrentCultureToResponseHeaders = true;
app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(corsPolicyName);

app.UseAuthorization();

app.MapMethods("/{*path}", ["OPTIONS"], () => Results.NoContent())
    .RequireCors(corsPolicyName);

app.MapControllers()
    .RequireCors(corsPolicyName);

app.Run();

static void ApplyPendingMigrations(DbContext context)
{
    EnsureMySqlDatabaseExists(context);
    EnsureMySqlMigrationsHistoryTableExists(context);

    var migrations = context.Database.GetMigrations().ToList();
    var appliedMigrations = GetAppliedMigrationIds(context);
    if (migrations.Count == 0 || migrations.All(appliedMigrations.Contains)) return;

    if (appliedMigrations.Count == 0)
        context.Database.Migrate();
    else
        ApplyPendingMigrationsWithScript(context, migrations, appliedMigrations);
}

static void ApplyPendingMigrationsWithScript(
    DbContext context,
    IReadOnlyList<string> migrations,
    HashSet<string> appliedMigrations)
{
    var fromMigration = migrations.LastOrDefault(appliedMigrations.Contains) ?? Migration.InitialDatabase;
    var script = context.GetService<IMigrator>()
        .GenerateScript(fromMigration, toMigration: null, MigrationsSqlGenerationOptions.Default);
    if (string.IsNullOrWhiteSpace(script)) return;

    var connectionString = context.Database.GetConnectionString();
    if (string.IsNullOrWhiteSpace(connectionString)) return;

    using var connection = new MySqlConnection(connectionString);
    connection.Open();
    new MySqlScript(connection, script).Execute();
}

static HashSet<string> GetAppliedMigrationIds(DbContext context)
{
    var connectionString = context.Database.GetConnectionString();
    if (string.IsNullOrWhiteSpace(connectionString)) return [];

    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT `MigrationId` FROM `__EFMigrationsHistory`;";

    using var reader = command.ExecuteReader();
    var appliedMigrations = new HashSet<string>(StringComparer.Ordinal);
    while (reader.Read()) appliedMigrations.Add(reader.GetString(0));

    return appliedMigrations;
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

static void EnsureMySqlMigrationsHistoryTableExists(DbContext context)
{
    var connectionString = context.Database.GetConnectionString();
    if (string.IsNullOrWhiteSpace(connectionString)) return;

    using var connection = new MySqlConnection(connectionString);
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = """
                          CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                              `MigrationId` varchar(150) NOT NULL,
                              `ProductVersion` varchar(32) NOT NULL,
                              PRIMARY KEY (`MigrationId`)
                          ) CHARACTER SET=utf8mb4;
                          """;
    command.ExecuteNonQuery();
}
