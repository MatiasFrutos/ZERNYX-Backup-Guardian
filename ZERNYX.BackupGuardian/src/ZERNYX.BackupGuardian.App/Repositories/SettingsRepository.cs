// Archivo: src\ZERNYX.BackupGuardian.App\Repositories\SettingsRepository.cs

using Microsoft.Data.Sqlite;
using ZERNYX.BackupGuardian.App.Data;
using ZERNYX.BackupGuardian.App.Models;

namespace ZERNYX.BackupGuardian.App.Repositories;

public sealed class SettingsRepository
{
    public AppSettings Get()
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            client_name,
            technician_name,
            default_backup_destination,
            start_minimized,
            start_with_windows,
            enable_notifications,
            enable_scheduler,
            dark_mode,
            retention_days,
            updated_at
        FROM app_settings
        WHERE id = 1
        LIMIT 1;
        """;

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            return Map(reader);
        }

        var settings = new AppSettings();
        Save(settings);

        return settings;
    }

    public void Save(AppSettings settings)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO app_settings (
            id,
            client_name,
            technician_name,
            default_backup_destination,
            start_minimized,
            start_with_windows,
            enable_notifications,
            enable_scheduler,
            dark_mode,
            retention_days,
            updated_at
        )
        VALUES (
            1,
            $client_name,
            $technician_name,
            $default_backup_destination,
            $start_minimized,
            $start_with_windows,
            $enable_notifications,
            $enable_scheduler,
            $dark_mode,
            $retention_days,
            $updated_at
        )
        ON CONFLICT(id) DO UPDATE SET
            client_name = excluded.client_name,
            technician_name = excluded.technician_name,
            default_backup_destination = excluded.default_backup_destination,
            start_minimized = excluded.start_minimized,
            start_with_windows = excluded.start_with_windows,
            enable_notifications = excluded.enable_notifications,
            enable_scheduler = excluded.enable_scheduler,
            dark_mode = excluded.dark_mode,
            retention_days = excluded.retention_days,
            updated_at = excluded.updated_at;
        """;

        command.Parameters.AddWithValue("$client_name", settings.ClientName.Trim());
        command.Parameters.AddWithValue("$technician_name", settings.TechnicianName.Trim());
        command.Parameters.AddWithValue("$default_backup_destination", settings.DefaultBackupDestination.Trim());
        command.Parameters.AddWithValue("$start_minimized", settings.StartMinimized ? 1 : 0);
        command.Parameters.AddWithValue("$start_with_windows", settings.StartWithWindows ? 1 : 0);
        command.Parameters.AddWithValue("$enable_notifications", settings.EnableNotifications ? 1 : 0);
        command.Parameters.AddWithValue("$enable_scheduler", settings.EnableScheduler ? 1 : 0);
        command.Parameters.AddWithValue("$dark_mode", settings.DarkMode ? 1 : 0);
        command.Parameters.AddWithValue("$retention_days", settings.RetentionDays);
        command.Parameters.AddWithValue("$updated_at", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        command.ExecuteNonQuery();
    }

    private static AppSettings Map(SqliteDataReader reader)
    {
        return new AppSettings
        {
            Id = reader.GetInt32(0),
            ClientName = reader.GetString(1),
            TechnicianName = reader.GetString(2),
            DefaultBackupDestination = reader.GetString(3),
            StartMinimized = reader.GetInt32(4) == 1,
            StartWithWindows = reader.GetInt32(5) == 1,
            EnableNotifications = reader.GetInt32(6) == 1,
            EnableScheduler = reader.GetInt32(7) == 1,
            DarkMode = reader.GetInt32(8) == 1,
            RetentionDays = reader.GetInt32(9),
            UpdatedAt = ReadDate(reader, 10)
        };
    }

    private static DateTime ReadDate(SqliteDataReader reader, int index)
    {
        var value = reader.GetString(index);
        return DateTime.TryParse(value, out var date) ? date : DateTime.Now;
    }
}