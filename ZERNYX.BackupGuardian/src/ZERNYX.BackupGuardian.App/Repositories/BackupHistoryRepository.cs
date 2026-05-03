// Archivo: src\ZERNYX.BackupGuardian.App\Repositories\BackupHistoryRepository.cs

using Microsoft.Data.Sqlite;
using ZERNYX.BackupGuardian.App.Data;
using ZERNYX.BackupGuardian.App.Models;

namespace ZERNYX.BackupGuardian.App.Repositories;

public sealed class BackupHistoryRepository
{
    public int Create(BackupHistory history)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        INSERT INTO backup_history (
            backup_task_id,
            task_name,
            source_path,
            destination_path,
            final_backup_path,
            status,
            files_copied,
            files_skipped,
            files_failed,
            total_bytes_copied,
            message,
            error_details,
            started_at,
            finished_at,
            duration_seconds,
            trigger_type
        )
        VALUES (
            $backup_task_id,
            $task_name,
            $source_path,
            $destination_path,
            $final_backup_path,
            $status,
            $files_copied,
            $files_skipped,
            $files_failed,
            $total_bytes_copied,
            $message,
            $error_details,
            $started_at,
            $finished_at,
            $duration_seconds,
            $trigger_type
        );

        SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("$backup_task_id", history.BackupTaskId);
        command.Parameters.AddWithValue("$task_name", history.TaskName);
        command.Parameters.AddWithValue("$source_path", history.SourcePath);
        command.Parameters.AddWithValue("$destination_path", history.DestinationPath);
        command.Parameters.AddWithValue("$final_backup_path", history.FinalBackupPath);
        command.Parameters.AddWithValue("$status", history.Status);
        command.Parameters.AddWithValue("$files_copied", history.FilesCopied);
        command.Parameters.AddWithValue("$files_skipped", history.FilesSkipped);
        command.Parameters.AddWithValue("$files_failed", history.FilesFailed);
        command.Parameters.AddWithValue("$total_bytes_copied", history.TotalBytesCopied);
        command.Parameters.AddWithValue("$message", history.Message ?? string.Empty);
        command.Parameters.AddWithValue("$error_details", history.ErrorDetails ?? string.Empty);
        command.Parameters.AddWithValue("$started_at", ToDbDate(history.StartedAt));
        command.Parameters.AddWithValue("$finished_at", history.FinishedAt.HasValue ? ToDbDate(history.FinishedAt.Value) : DBNull.Value);
        command.Parameters.AddWithValue("$duration_seconds", history.DurationSeconds);
        command.Parameters.AddWithValue("$trigger_type", history.TriggerType);

        var result = command.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public List<BackupHistory> GetLatest(int limit = 100)
    {
        var items = new List<BackupHistory>();

        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            backup_task_id,
            task_name,
            source_path,
            destination_path,
            final_backup_path,
            status,
            files_copied,
            files_skipped,
            files_failed,
            total_bytes_copied,
            message,
            error_details,
            started_at,
            finished_at,
            duration_seconds,
            trigger_type
        FROM backup_history
        ORDER BY started_at DESC
        LIMIT $limit;
        """;

        command.Parameters.AddWithValue("$limit", Math.Clamp(limit, 1, 1000));

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(Map(reader));
        }

        return items;
    }

    public List<BackupHistory> GetByTaskId(int taskId)
    {
        var items = new List<BackupHistory>();

        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            backup_task_id,
            task_name,
            source_path,
            destination_path,
            final_backup_path,
            status,
            files_copied,
            files_skipped,
            files_failed,
            total_bytes_copied,
            message,
            error_details,
            started_at,
            finished_at,
            duration_seconds,
            trigger_type
        FROM backup_history
        WHERE backup_task_id = $task_id
        ORDER BY started_at DESC;
        """;

        command.Parameters.AddWithValue("$task_id", taskId);

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            items.Add(Map(reader));
        }

        return items;
    }

    public int CountAll()
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "SELECT COUNT(*) FROM backup_history;";
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int CountByStatus(string status)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "SELECT COUNT(*) FROM backup_history WHERE status = $status;";
        command.Parameters.AddWithValue("$status", status);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public BackupHistory? GetLast()
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            backup_task_id,
            task_name,
            source_path,
            destination_path,
            final_backup_path,
            status,
            files_copied,
            files_skipped,
            files_failed,
            total_bytes_copied,
            message,
            error_details,
            started_at,
            finished_at,
            duration_seconds,
            trigger_type
        FROM backup_history
        ORDER BY started_at DESC
        LIMIT 1;
        """;

        using var reader = command.ExecuteReader();

        return reader.Read() ? Map(reader) : null;
    }

    public void DeleteAll()
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM backup_history;";
        command.ExecuteNonQuery();
    }

    private static BackupHistory Map(SqliteDataReader reader)
    {
        return new BackupHistory
        {
            Id = reader.GetInt32(0),
            BackupTaskId = reader.GetInt32(1),
            TaskName = reader.GetString(2),
            SourcePath = reader.GetString(3),
            DestinationPath = reader.GetString(4),
            FinalBackupPath = reader.GetString(5),
            Status = reader.GetString(6),
            FilesCopied = reader.GetInt32(7),
            FilesSkipped = reader.GetInt32(8),
            FilesFailed = reader.GetInt32(9),
            TotalBytesCopied = reader.GetInt64(10),
            Message = reader.GetString(11),
            ErrorDetails = reader.GetString(12),
            StartedAt = ReadDate(reader, 13),
            FinishedAt = ReadNullableDate(reader, 14),
            DurationSeconds = reader.GetInt32(15),
            TriggerType = reader.GetString(16)
        };
    }

    private static string ToDbDate(DateTime value)
    {
        return value.ToString("yyyy-MM-dd HH:mm:ss");
    }

    private static DateTime ReadDate(SqliteDataReader reader, int index)
    {
        var value = reader.GetString(index);
        return DateTime.TryParse(value, out var date) ? date : DateTime.Now;
    }

    private static DateTime? ReadNullableDate(SqliteDataReader reader, int index)
    {
        if (reader.IsDBNull(index))
        {
            return null;
        }

        var value = reader.GetString(index);
        return DateTime.TryParse(value, out var date) ? date : null;
    }
}