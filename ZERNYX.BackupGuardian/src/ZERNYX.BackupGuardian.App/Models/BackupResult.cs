// Archivo: src\ZERNYX.BackupGuardian.App\Models\BackupResult.cs

namespace ZERNYX.BackupGuardian.App.Models;

public sealed class BackupResult
{
    public bool Success { get; set; }

    public int BackupTaskId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string SourcePath { get; set; } = string.Empty;

    public string DestinationPath { get; set; } = string.Empty;

    public string FinalBackupPath { get; set; } = string.Empty;

    public int FilesCopied { get; set; }

    public int FilesSkipped { get; set; }

    public int FilesFailed { get; set; }

    public long TotalBytesCopied { get; set; }

    public string Message { get; set; } = string.Empty;

    public string ErrorDetails { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.Now;

    public DateTime FinishedAt { get; set; } = DateTime.Now;

    public int DurationSeconds => Math.Max(0, (int)(FinishedAt - StartedAt).TotalSeconds);

    public string Status => Success ? BackupStatus.Success : BackupStatus.Error;

    public BackupHistory ToHistory(string triggerType)
    {
        return new BackupHistory
        {
            BackupTaskId = BackupTaskId,
            TaskName = TaskName,
            SourcePath = SourcePath,
            DestinationPath = DestinationPath,
            FinalBackupPath = FinalBackupPath,
            Status = Status,
            FilesCopied = FilesCopied,
            FilesSkipped = FilesSkipped,
            FilesFailed = FilesFailed,
            TotalBytesCopied = TotalBytesCopied,
            Message = Message,
            ErrorDetails = ErrorDetails,
            StartedAt = StartedAt,
            FinishedAt = FinishedAt,
            DurationSeconds = DurationSeconds,
            TriggerType = triggerType
        };
    }
}