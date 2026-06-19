using ColdTrace.Platform.AssetManagement.Application.Internal.CommandServices;
using ColdTrace.Platform.AssetManagement.Application.Internal.QueryServices;
using ColdTrace.Platform.AssetManagement.Domain.Services;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.AssetManagement.Infrastructure.Persistence.EFC.Repositories;
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
builder.Services.AddScoped<ILocationCommandService, LocationCommandService>();
builder.Services.AddScoped<ILocationQueryService, LocationQueryService>();
builder.Services.AddScoped<IGatewayCommandService, GatewayCommandService>();
builder.Services.AddScoped<IGatewayQueryService, GatewayQueryService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();
builder.Services.AddScoped<IAssetCommandService, AssetCommandService>();
builder.Services.AddScoped<IAssetQueryService, AssetQueryService>(); //HU-48
builder.Services.AddScoped<IIotDeviceRepository, IotDeviceRepository>();
builder.Services.AddScoped<IIotDeviceCommandService, IotDeviceCommandService>();
builder.Services.AddScoped<IIotDeviceQueryService, IotDeviceQueryService>();

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

    context.Database.Migrate();
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
