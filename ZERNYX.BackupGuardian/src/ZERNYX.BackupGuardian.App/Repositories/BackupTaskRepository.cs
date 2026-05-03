// Archivo: src\ZERNYX.BackupGuardian.App\Repositories\BackupTaskRepository.cs

using Microsoft.Data.Sqlite;
using ZERNYX.BackupGuardian.App.Data;
using ZERNYX.BackupGuardian.App.Models;

namespace ZERNYX.BackupGuardian.App.Repositories;

public sealed class BackupTaskRepository
{
    public List<BackupTask> GetAll()
    {
        var tasks = new List<BackupTask>();

        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            name,
            source_path,
            destination_path,
            is_active,
            frequency,
            scheduled_days,
            scheduled_time,
            include_subfolders,
            overwrite_existing_files,
            create_date_folder,
            last_run_at,
            last_success_at,
            last_error_at,
            last_scheduled_run_date,
            last_status,
            notes,
            created_at,
            updated_at
        FROM backup_tasks
        ORDER BY created_at DESC;
        """;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            tasks.Add(Map(reader));
        }

        return tasks;
    }

    public List<BackupTask> GetActive()
    {
        var tasks = new List<BackupTask>();

        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            name,
            source_path,
            destination_path,
            is_active,
            frequency,
            scheduled_days,
            scheduled_time,
            include_subfolders,
            overwrite_existing_files,
            create_date_folder,
            last_run_at,
            last_success_at,
            last_error_at,
            last_scheduled_run_date,
            last_status,
            notes,
            created_at,
            updated_at
        FROM backup_tasks
        WHERE is_active = 1
        ORDER BY name ASC;
        """;

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            tasks.Add(Map(reader));
        }

        return tasks;
    }

    public BackupTask? GetById(int id)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        SELECT
            id,
            name,
            source_path,
            destination_path,
            is_active,
            frequency,
            scheduled_days,
            scheduled_time,
            include_subfolders,
            overwrite_existing_files,
            create_date_folder,
            last_run_at,
            last_success_at,
            last_error_at,
            last_scheduled_run_date,
            last_status,
            notes,
            created_at,
            updated_at
        FROM backup_tasks
        WHERE id = $id
        LIMIT 1;
        """;

        command.Parameters.AddWithValue("$id", id);

        using var reader = command.ExecuteReader();

        return reader.Read() ? Map(reader) : null;
    }

    public int Create(BackupTask task)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        var now = DateTime.Now;

        command.CommandText = """
        INSERT INTO backup_tasks (
            name,
            source_path,
            destination_path,
            is_active,
            frequency,
            scheduled_days,
            scheduled_time,
            include_subfolders,
            overwrite_existing_files,
            create_date_folder,
            last_status,
            notes,
            created_at,
            updated_at
        )
        VALUES (
            $name,
            $source_path,
            $destination_path,
            $is_active,
            $frequency,
            $scheduled_days,
            $scheduled_time,
            $include_subfolders,
            $overwrite_existing_files,
            $create_date_folder,
            $last_status,
            $notes,
            $created_at,
            $updated_at
        );

        SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("$name", task.Name.Trim());
        command.Parameters.AddWithValue("$source_path", task.SourcePath.Trim());
        command.Parameters.AddWithValue("$destination_path", task.DestinationPath.Trim());
        command.Parameters.AddWithValue("$is_active", task.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$frequency", task.Frequency);
        command.Parameters.AddWithValue("$scheduled_days", task.ScheduledDays);
        command.Parameters.AddWithValue("$scheduled_time", task.ScheduledTime);
        command.Parameters.AddWithValue("$include_subfolders", task.IncludeSubfolders ? 1 : 0);
        command.Parameters.AddWithValue("$overwrite_existing_files", task.OverwriteExistingFiles ? 1 : 0);
        command.Parameters.AddWithValue("$create_date_folder", task.CreateDateFolder ? 1 : 0);
        command.Parameters.AddWithValue("$last_status", string.IsNullOrWhiteSpace(task.LastStatus) ? "Pendiente" : task.LastStatus);
        command.Parameters.AddWithValue("$notes", task.Notes ?? string.Empty);
        command.Parameters.AddWithValue("$created_at", ToDbDate(now));
        command.Parameters.AddWithValue("$updated_at", ToDbDate(now));

        var result = command.ExecuteScalar();
        return Convert.ToInt32(result);
    }

    public void Update(BackupTask task)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        UPDATE backup_tasks
        SET
            name = $name,
            source_path = $source_path,
            destination_path = $destination_path,
            is_active = $is_active,
            frequency = $frequency,
            scheduled_days = $scheduled_days,
            scheduled_time = $scheduled_time,
            include_subfolders = $include_subfolders,
            overwrite_existing_files = $overwrite_existing_files,
            create_date_folder = $create_date_folder,
            notes = $notes,
            updated_at = $updated_at
        WHERE id = $id;
        """;

        command.Parameters.AddWithValue("$id", task.Id);
        command.Parameters.AddWithValue("$name", task.Name.Trim());
        command.Parameters.AddWithValue("$source_path", task.SourcePath.Trim());
        command.Parameters.AddWithValue("$destination_path", task.DestinationPath.Trim());
        command.Parameters.AddWithValue("$is_active", task.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("$frequency", task.Frequency);
        command.Parameters.AddWithValue("$scheduled_days", task.ScheduledDays);
        command.Parameters.AddWithValue("$scheduled_time", task.ScheduledTime);
        command.Parameters.AddWithValue("$include_subfolders", task.IncludeSubfolders ? 1 : 0);
        command.Parameters.AddWithValue("$overwrite_existing_files", task.OverwriteExistingFiles ? 1 : 0);
        command.Parameters.AddWithValue("$create_date_folder", task.CreateDateFolder ? 1 : 0);
        command.Parameters.AddWithValue("$notes", task.Notes ?? string.Empty);
        command.Parameters.AddWithValue("$updated_at", ToDbDate(DateTime.Now));

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = "DELETE FROM backup_tasks WHERE id = $id;";
        command.Parameters.AddWithValue("$id", id);

        command.ExecuteNonQuery();
    }

    public void UpdateRunStatus(int taskId, string status, DateTime runAt, bool success)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        UPDATE backup_tasks
        SET
            last_run_at = $last_run_at,
            last_success_at = CASE WHEN $success = 1 THEN $last_run_at ELSE last_success_at END,
            last_error_at = CASE WHEN $success = 0 THEN $last_run_at ELSE last_error_at END,
            last_status = $last_status,
            updated_at = $updated_at
        WHERE id = $id;
        """;

        command.Parameters.AddWithValue("$id", taskId);
        command.Parameters.AddWithValue("$last_run_at", ToDbDate(runAt));
        command.Parameters.AddWithValue("$success", success ? 1 : 0);
        command.Parameters.AddWithValue("$last_status", status);
        command.Parameters.AddWithValue("$updated_at", ToDbDate(DateTime.Now));

        command.ExecuteNonQuery();
    }

    public void MarkScheduledRun(int taskId, DateTime date)
    {
        using var connection = AppDb.GetConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
        UPDATE backup_tasks
        SET
            last_scheduled_run_date = $date,
            updated_at = $updated_at
        WHERE id = $id;
        """;

        command.Parameters.AddWithValue("$id", taskId);
        command.Parameters.AddWithValue("$date", date.Date.ToString("yyyy-MM-dd"));
        command.Parameters.AddWithValue("$updated_at", ToDbDate(DateTime.Now));

        command.ExecuteNonQuery();
    }

    private static BackupTask Map(SqliteDataReader reader)
    {
        return new BackupTask
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            SourcePath = reader.GetString(2),
            DestinationPath = reader.GetString(3),
            IsActive = reader.GetInt32(4) == 1,
            Frequency = reader.GetString(5),
            ScheduledDays = reader.GetString(6),
            ScheduledTime = reader.GetString(7),
            IncludeSubfolders = reader.GetInt32(8) == 1,
            OverwriteExistingFiles = reader.GetInt32(9) == 1,
            CreateDateFolder = reader.GetInt32(10) == 1,
            LastRunAt = ReadNullableDate(reader, 11),
            LastSuccessAt = ReadNullableDate(reader, 12),
            LastErrorAt = ReadNullableDate(reader, 13),
            LastScheduledRunDate = ReadNullableDate(reader, 14),
            LastStatus = reader.GetString(15),
            Notes = reader.GetString(16),
            CreatedAt = ReadDate(reader, 17),
            UpdatedAt = ReadDate(reader, 18)
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