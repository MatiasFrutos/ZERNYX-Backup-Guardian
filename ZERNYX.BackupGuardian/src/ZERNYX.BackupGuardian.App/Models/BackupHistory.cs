// Archivo: src\ZERNYX.BackupGuardian.App\Models\BackupHistory.cs

namespace ZERNYX.BackupGuardian.App.Models;

public sealed class BackupHistory
{
    public int Id { get; set; }

    public int BackupTaskId { get; set; }

    public string TaskName { get; set; } = string.Empty;

    public string SourcePath { get; set; } = string.Empty;

    public string DestinationPath { get; set; } = string.Empty;

    public string FinalBackupPath { get; set; } = string.Empty;

    public string Status { get; set; } = BackupStatus.Pending;

    public int FilesCopied { get; set; }

    public int FilesSkipped { get; set; }

    public int FilesFailed { get; set; }

    public long TotalBytesCopied { get; set; }

    public string Message { get; set; } = string.Empty;

    public string ErrorDetails { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.Now;

    public DateTime? FinishedAt { get; set; }

    public int DurationSeconds { get; set; }

    public string TriggerType { get; set; } = BackupTriggerType.Manual;

    public bool IsSuccess => Status == BackupStatus.Success;

    public bool IsError => Status == BackupStatus.Error;
}

public static class BackupStatus
{
    public const string Pending = "pending";
    public const string Running = "running";
    public const string Success = "success";
    public const string Error = "error";
    public const string Cancelled = "cancelled";

    public static string ToDisplayName(string status)
    {
        return status switch
        {
            Success => "Exitoso",
            Error => "Error",
            Running => "En proceso",
            Cancelled => "Cancelado",
            Pending => "Pendiente",
            _ => "Desconocido"
        };
    }
}

public static class BackupTriggerType
{
    public const string Manual = "manual";
    public const string Scheduled = "scheduled";

    public static string ToDisplayName(string trigger)
    {
        return trigger switch
        {
            Manual => "Manual",
            Scheduled => "Programado",
            _ => "Desconocido"
        };
    }
}