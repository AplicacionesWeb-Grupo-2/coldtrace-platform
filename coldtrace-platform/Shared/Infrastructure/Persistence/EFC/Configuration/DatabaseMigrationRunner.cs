using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.MySqlClient;

namespace ColdTrace.Platform.Shared.Infrastructure.Persistence.EFC.Configuration;

/// <summary>
///     Applies Entity Framework Core migrations for MySQL in an idempotent way.
/// </summary>
public static class DatabaseMigrationRunner
{
    /// <summary>
    ///     Ensures the configured database exists and applies pending EF Core migrations.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public static void ApplyPendingMigrations(AppDbContext context)
    {
        var connectionString = context.Database.GetDbConnection().ConnectionString;
        var connectionStringBuilder = new MySqlConnectionStringBuilder(connectionString);
        var databaseName = connectionStringBuilder.Database;

        if (string.IsNullOrWhiteSpace(databaseName))
            throw new InvalidOperationException("Database name is not set in the connection string.");

        EnsureDatabaseExists(connectionStringBuilder, databaseName);
        EnsureMigrationsHistoryTable(connectionStringBuilder);

        var appliedMigrations = GetAppliedMigrations(connectionStringBuilder).ToHashSet(StringComparer.Ordinal);
        var availableMigrations = context.Database.GetMigrations().ToArray();
        var pendingMigrations = availableMigrations
            .Where(migration => !appliedMigrations.Contains(migration))
            .ToArray();

        if (pendingMigrations.Length == 0) return;

        var lastAppliedMigration = availableMigrations.LastOrDefault(appliedMigrations.Contains);
        var migrator = context.GetService<IMigrator>();
        var migrationScript = migrator.GenerateScript(lastAppliedMigration, null);

        ExecuteDatabaseCommand(connectionStringBuilder, migrationScript);
    }

    private static void EnsureDatabaseExists(MySqlConnectionStringBuilder connectionStringBuilder, string databaseName)
    {
        var serverConnectionStringBuilder = new MySqlConnectionStringBuilder(connectionStringBuilder.ConnectionString)
        {
            Database = string.Empty
        };

        ExecuteDatabaseCommand(
            serverConnectionStringBuilder,
            $"CREATE DATABASE IF NOT EXISTS {QuoteMySqlIdentifier(databaseName)};");
    }

    private static void EnsureMigrationsHistoryTable(MySqlConnectionStringBuilder connectionStringBuilder)
    {
        const string commandText = """
                                   CREATE TABLE IF NOT EXISTS `__EFMigrationsHistory` (
                                       `MigrationId` varchar(150) NOT NULL,
                                       `ProductVersion` varchar(32) NOT NULL,
                                       PRIMARY KEY (`MigrationId`)
                                   );
                                   """;

        ExecuteDatabaseCommand(connectionStringBuilder, commandText);
    }

    private static IEnumerable<string> GetAppliedMigrations(MySqlConnectionStringBuilder connectionStringBuilder)
    {
        using var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT `MigrationId`
                              FROM `__EFMigrationsHistory`
                              ORDER BY `MigrationId`;
                              """;

        using var reader = command.ExecuteReader();
        while (reader.Read())
            yield return reader.GetString(0);
    }

    private static void ExecuteDatabaseCommand(
        MySqlConnectionStringBuilder connectionStringBuilder,
        string commandText)
    {
        if (string.IsNullOrWhiteSpace(commandText)) return;

        using var connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.ExecuteNonQuery();
    }

    private static string QuoteMySqlIdentifier(string identifier)
    {
        return $"`{identifier.Replace("`", "``", StringComparison.Ordinal)}`";
    }
}
