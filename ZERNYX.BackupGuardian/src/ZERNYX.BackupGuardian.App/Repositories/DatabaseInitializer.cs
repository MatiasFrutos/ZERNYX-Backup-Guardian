// Archivo: src\ZERNYX.BackupGuardian.App\Repositories\DatabaseInitializer.cs

using ZERNYX.BackupGuardian.App.Data;

namespace ZERNYX.BackupGuardian.App.Repositories;

public static class DatabaseInitializer
{
    public static void Initialize()
    {
        AppConfig.EnsureAppFolders();

        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        CREATE TABLE IF NOT EXISTS backup_tasks (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            name TEXT NOT NULL,
            source_path TEXT NOT NULL,
            destination_path TEXT NOT NULL,
            is_active INTEGER NOT NULL DEFAULT 1,
            frequency TEXT NOT NULL DEFAULT 'manual',
            scheduled_days TEXT NOT NULL DEFAULT '',
            scheduled_time TEXT NOT NULL DEFAULT '18:00',
            include_subfolders INTEGER NOT NULL DEFAULT 1,
            overwrite_existing_files INTEGER NOT NULL DEFAULT 1,
            create_date_folder INTEGER NOT NULL DEFAULT 1,
            last_run_at TEXT NULL,
            last_success_at TEXT NULL,
            last_error_at TEXT NULL,
            last_scheduled_run_date TEXT NULL,
            last_status TEXT NOT NULL DEFAULT 'Pendiente',
            notes TEXT NOT NULL DEFAULT '',
            created_at TEXT NOT NULL,
            updated_at TEXT NOT NULL
        );

        CREATE TABLE IF NOT EXISTS backup_history (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            backup_task_id INTEGER NOT NULL,
            task_name TEXT NOT NULL,
            source_path TEXT NOT NULL,
            destination_path TEXT NOT NULL,
            final_backup_path TEXT NOT NULL,
            status TEXT NOT NULL,
            files_copied INTEGER NOT NULL DEFAULT 0,
            files_skipped INTEGER NOT NULL DEFAULT 0,
            files_failed INTEGER NOT NULL DEFAULT 0,
            total_bytes_copied INTEGER NOT NULL DEFAULT 0,
            message TEXT NOT NULL DEFAULT '',
            error_details TEXT NOT NULL DEFAULT '',
            started_at TEXT NOT NULL,
            finished_at TEXT NULL,
            duration_seconds INTEGER NOT NULL DEFAULT 0,
            trigger_type TEXT NOT NULL DEFAULT 'manual',
            FOREIGN KEY (backup_task_id) REFERENCES backup_tasks(id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS app_settings (
            id INTEGER PRIMARY KEY CHECK (id = 1),
            client_name TEXT NOT NULL DEFAULT 'Cliente',
            technician_name TEXT NOT NULL DEFAULT 'ZERNYX Tech Studio',
            default_backup_destination TEXT NOT NULL DEFAULT '',
            start_minimized INTEGER NOT NULL DEFAULT 0,
            start_with_windows INTEGER NOT NULL DEFAULT 0,
            enable_notifications INTEGER NOT NULL DEFAULT 1,
            enable_scheduler INTEGER NOT NULL DEFAULT 1,
            dark_mode INTEGER NOT NULL DEFAULT 0,
            retention_days INTEGER NOT NULL DEFAULT 30,
            updated_at TEXT NOT NULL
        );

        INSERT OR IGNORE INTO app_settings (
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
            'Cliente',
            'ZERNYX Tech Studio',
            '',
            0,
            0,
            1,
            1,
            0,
            30,
            datetime('now')
        );

        CREATE INDEX IF NOT EXISTS idx_backup_tasks_is_active
        ON backup_tasks(is_active);

        CREATE INDEX IF NOT EXISTS idx_backup_history_task_id
        ON backup_history(backup_task_id);

        CREATE INDEX IF NOT EXISTS idx_backup_history_started_at
        ON backup_history(started_at);
        """;

        command.ExecuteNonQuery();
    }
}