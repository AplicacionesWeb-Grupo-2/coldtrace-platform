using ColdTrace.Platform.AssetManagement.Domain.Model.Aggregates;
using ColdTrace.Platform.AssetManagement.Domain.Repositories;
using ColdTrace.Platform.IdentityAccess.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Application.Errors;
using ColdTrace.Platform.Monitoring.Domain.Model.Aggregates;
using ColdTrace.Platform.Monitoring.Domain.Model.Commands;
using ColdTrace.Platform.Monitoring.Domain.Repositories;
using ColdTrace.Platform.Monitoring.Domain.Services;
using ColdTrace.Platform.Shared.Application.Patterns;
using ColdTrace.Platform.Shared.Domain.Repositories;

namespace ColdTrace.Platform.Monitoring.Application.Internal.CommandServices;

/// <summary>
///     Application service for sensor reading command operations.
/// </summary>
public class SensorReadingCommandService(
    ISensorReadingRepository sensorReadingRepository,
    IOrganizationRepository organizationRepository,
    IAssetRepository assetRepository,
    IIotDeviceRepository iotDeviceRepository,
    IGatewayRepository gatewayRepository,
    IAssetSettingsRepository assetSettingsRepository,
    IUnitOfWork unitOfWork,
    ILogger<SensorReadingCommandService> logger)
    : ISensorReadingCommandService
{
    private const string OfflineStatus = "offline";
    private const int LowBatteryThreshold = 15;
    private const int LowSignalThreshold = 35;

    /// <inheritdoc />
    public async Task<Result<SensorReading, CreateSensorReadingError>> Handle(
        CreateSensorReadingCommand command,
        CancellationToken cancellationToken = default)
    {
        var contextResult = await ResolveContextAsync(
            command.OrganizationId,
            command.AssetId,
            command.IotDeviceId,
            cancellationToken);
        if (contextResult is Result<ReadingContext, CreateSensorReadingError>.Failure contextFailure)
            return new Result<SensorReading, CreateSensorReadingError>.Failure(contextFailure.Error);

        var context = ((Result<ReadingContext, CreateSensorReadingError>.Success)contextResult).Value;
        if (!SupportsRequestedMeasurements(
                context.Device,
                command.Temperature,
                command.Humidity,
                command.MotionDetected,
                command.ImageCaptured,
                command.BatteryLevel,
                command.SignalStrength))
            return new Result<SensorReading, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.UnsupportedMeasurement);

        try
        {
            var outOfRange = EvaluateOutOfRange(
                context.Settings,
                command.Temperature,
                command.Humidity,
                command.BatteryLevel,
                command.SignalStrength);
            var sensorReading = new SensorReading(
                command.OrganizationId,
                command.AssetId,
                command.IotDeviceId,
                context.Gateway.Id,
                context.Asset.LocationId,
                command.Temperature,
                command.Humidity,
                outOfRange,
                command.RecordedAt,
                command.MotionDetected,
                command.ImageCaptured,
                command.BatteryLevel,
                command.SignalStrength);
            await sensorReadingRepository.AddAsync(sensorReading, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<SensorReading, CreateSensorReadingError>.Success(sensorReading);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "Unexpected error creating sensor reading for organization {OrganizationId} and IoT device {IotDeviceId}",
                command.OrganizationId,
                command.IotDeviceId);
            return new Result<SensorReading, CreateSensorReadingError>.Failure(CreateSensorReadingError.UnexpectedError);
        }
    }

    /// <inheritdoc />
    public async Task<Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>> Handle(
        GenerateDemoSensorReadingsCommand command,
        CancellationToken cancellationToken = default)
    {
        var organization = await organizationRepository.FindByIdAsync(command.OrganizationId, cancellationToken);
        if (organization is null)
            return new Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Failure(
                GenerateDemoSensorReadingsError.OrganizationNotFound);

        if (command.AssetId is not null)
        {
            var asset = await assetRepository.FindByIdAndOrganizationIdAsync(
                command.AssetId.Value,
                command.OrganizationId,
                cancellationToken);
            if (asset is null)
                return new Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Failure(
                    GenerateDemoSensorReadingsError.AssetNotFound);
        }

        try
        {
            var devices = await iotDeviceRepository.FindAllByOrganizationIdAsync(
                command.OrganizationId,
                cancellationToken);
            var candidates = devices
                .Where(device => device.AssetId is not null)
                .Where(device => command.AssetId is null || device.AssetId == command.AssetId)
                .Where(device => !string.Equals(device.Status, OfflineStatus, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var generated = new List<SensorReading>();
            for (var index = 0; index < command.Count; index++)
            {
                var reading = await GenerateOneAsync(command.OrganizationId, candidates, index, cancellationToken);
                if (reading is not null) generated.Add(reading);
            }

            if (generated.Count == 0)
                return new Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Failure(
                    GenerateDemoSensorReadingsError.NoEligibleDevices);

            await unitOfWork.CompleteAsync(cancellationToken);
            return new Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Success(generated);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error generating demo sensor readings for organization {OrganizationId}",
                command.OrganizationId);
            return new Result<IEnumerable<SensorReading>, GenerateDemoSensorReadingsError>.Failure(
                GenerateDemoSensorReadingsError.UnexpectedError);
        }
    }

    private async Task<SensorReading?> GenerateOneAsync(
        int organizationId,
        IReadOnlyList<IotDevice> candidates,
        int offsetMinutes,
        CancellationToken cancellationToken)
    {
        if (candidates.Count == 0) return null;

        var start = Random.Shared.Next(candidates.Count);
        for (var attempt = 0; attempt < candidates.Count; attempt++)
        {
            var device = candidates[(start + attempt) % candidates.Count];
            if (device.AssetId is null) continue;

            var contextResult = await ResolveContextAsync(
                organizationId,
                device.AssetId.Value,
                device.Id,
                cancellationToken);
            if (contextResult is not Result<ReadingContext, CreateSensorReadingError>.Success contextSuccess) continue;

            var context = contextSuccess.Value;
            var parameters = context.Device.MeasurementParameters;
            double? temperature = ContainsParameter(parameters, "temperature")
                ? RandomTemperature(context.Settings.MinimumTemperature, context.Settings.MaximumTemperature)
                : null;
            double? humidity = ContainsParameter(parameters, "humidity")
                ? RandomHumidity(context.Settings.MinimumHumidity, context.Settings.MaximumHumidity)
                : null;
            bool? motionDetected = ContainsParameter(parameters, "motion") ? Random.Shared.NextDouble() < 0.18 : null;
            bool? imageCaptured = ContainsParameter(parameters, "image") ? Random.Shared.NextDouble() < 0.35 : null;
            int? batteryLevel = ContainsParameter(parameters, "battery") ? RandomPercentage(8, 100, 0.96, 20, 100) : null;
            int? signalStrength = ContainsParameter(parameters, "signal") ? RandomPercentage(28, 100, 0.96, 40, 100) : null;
            if (temperature is null && humidity is null && motionDetected is null && imageCaptured is null &&
                batteryLevel is null && signalStrength is null)
                continue;

            var outOfRange = EvaluateOutOfRange(
                context.Settings,
                temperature,
                humidity,
                batteryLevel,
                signalStrength);
            var reading = new SensorReading(
                organizationId,
                context.Asset.Id,
                context.Device.Id,
                context.Gateway.Id,
                context.Asset.LocationId,
                temperature,
                humidity,
                outOfRange,
                DateTimeOffset.UtcNow.AddMinutes(-offsetMinutes),
                motionDetected,
                imageCaptured,
                batteryLevel,
                signalStrength);
            await sensorReadingRepository.AddAsync(reading, cancellationToken);
            return reading;
        }

        return null;
    }

    private async Task<Result<ReadingContext, CreateSensorReadingError>> ResolveContextAsync(
        int organizationId,
        int assetId,
        int iotDeviceId,
        CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.FindByIdAsync(organizationId, cancellationToken);
        if (organization is null)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.OrganizationNotFound);

        var asset = await assetRepository.FindByIdAndOrganizationIdAsync(assetId, organizationId, cancellationToken);
        if (asset is null)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(CreateSensorReadingError.AssetNotFound);

        var device = await iotDeviceRepository.FindByIdAndOrganizationIdAsync(
            iotDeviceId,
            organizationId,
            cancellationToken);
        if (device is null)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.IotDeviceNotFound);

        if (device.AssetId != assetId)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.DeviceNotAssignedToAsset);

        if (string.Equals(device.Status, OfflineStatus, StringComparison.OrdinalIgnoreCase))
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(CreateSensorReadingError.DeviceOffline);

        var gateway = await gatewayRepository.FindByIdAndOrganizationIdAsync(
            device.GatewayId,
            organizationId,
            cancellationToken);
        if (gateway is null)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.GatewayNotFound);

        if (string.Equals(gateway.Status, OfflineStatus, StringComparison.OrdinalIgnoreCase))
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(CreateSensorReadingError.GatewayOffline);

        if (asset.LocationId != gateway.LocationId)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.IncompatibleLocation);

        var settings = await assetSettingsRepository.FindByOrganizationIdAndAssetIdAsync(
                           organizationId,
                           assetId,
                           cancellationToken)
                       ?? await assetSettingsRepository.FindDefaultByOrganizationIdAsync(
                           organizationId,
                           cancellationToken);
        if (settings is null)
            return new Result<ReadingContext, CreateSensorReadingError>.Failure(
                CreateSensorReadingError.AssetSettingsNotFound);

        return new Result<ReadingContext, CreateSensorReadingError>.Success(new ReadingContext(asset, device, gateway, settings));
    }

    private static bool SupportsRequestedMeasurements(
        IotDevice device,
        double? temperature,
        double? humidity,
        bool? motionDetected,
        bool? imageCaptured,
        int? batteryLevel,
        int? signalStrength)
    {
        var parameters = device.MeasurementParameters;
        return (temperature is null || ContainsParameter(parameters, "temperature")) &&
               (humidity is null || ContainsParameter(parameters, "humidity")) &&
               (motionDetected is null || ContainsParameter(parameters, "motion")) &&
               (imageCaptured is null || ContainsParameter(parameters, "image")) &&
               (batteryLevel is null || ContainsParameter(parameters, "battery")) &&
               (signalStrength is null || ContainsParameter(parameters, "signal"));
    }

    private static bool EvaluateOutOfRange(
        AssetSettings settings,
        double? temperature,
        double? humidity,
        int? batteryLevel,
        int? signalStrength)
    {
        var temperatureOutOfRange = temperature is not null &&
                                    (temperature < settings.MinimumTemperature ||
                                     temperature > settings.MaximumTemperature);
        var humidityOutOfRange = humidity is not null &&
                                 (humidity < settings.MinimumHumidity || humidity > settings.MaximumHumidity);
        var batteryOutOfRange = batteryLevel is not null && batteryLevel < LowBatteryThreshold;
        var signalOutOfRange = signalStrength is not null && signalStrength < LowSignalThreshold;
        return temperatureOutOfRange || humidityOutOfRange || batteryOutOfRange || signalOutOfRange;
    }

    private static bool ContainsParameter(IEnumerable<string> parameters, string expected) =>
        parameters.Any(parameter => string.Equals(parameter, expected, StringComparison.OrdinalIgnoreCase));

    private static double RandomTemperature(double minimum, double maximum)
    {
        var roll = Random.Shared.NextDouble();
        if (roll < 0.94) return RoundOne(RandomDouble(minimum, maximum));
        return roll < 0.97
            ? RoundOne(RandomDouble(minimum - 2, minimum - 0.2))
            : RoundOne(RandomDouble(maximum + 0.2, maximum + 3));
    }

    private static double RandomHumidity(double minimum, double maximum)
    {
        var roll = Random.Shared.NextDouble();
        return roll < 0.94
            ? RoundOne(RandomDouble(minimum, maximum))
            : RoundOne(RandomDouble(maximum + 1, maximum + 8));
    }

    private static int RandomPercentage(
        int abnormalMinimum,
        int abnormalMaximum,
        double normalChance,
        int normalMinimum,
        int normalMaximum)
    {
        return Random.Shared.NextDouble() < normalChance
            ? Random.Shared.Next(normalMinimum, normalMaximum + 1)
            : Random.Shared.Next(abnormalMinimum, abnormalMaximum + 1);
    }

    private static double RandomDouble(double minimum, double maximum) =>
        minimum + Random.Shared.NextDouble() * (maximum - minimum);

    private static double RoundOne(double value) => Math.Round(value, 1, MidpointRounding.AwayFromZero);

    private sealed record ReadingContext(
        Asset Asset,
        IotDevice Device,
        Gateway Gateway,
        AssetSettings Settings);
}
