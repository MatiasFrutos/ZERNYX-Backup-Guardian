// Archivo: src\ZERNYX.BackupGuardian.App\Models\BackupTask.cs

namespace ZERNYX.BackupGuardian.App.Models;

public sealed class BackupTask
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SourcePath { get; set; } = string.Empty;

    public string DestinationPath { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Frequency { get; set; } = BackupFrequency.Manual;

    public string ScheduledDays { get; set; } = string.Empty;

    public string ScheduledTime { get; set; } = "18:00";

    public bool IncludeSubfolders { get; set; } = true;

    public bool OverwriteExistingFiles { get; set; } = true;

    public bool CreateDateFolder { get; set; } = true;

    public DateTime? LastRunAt { get; set; }

    public DateTime? LastSuccessAt { get; set; }

    public DateTime? LastErrorAt { get; set; }

    public DateTime? LastScheduledRunDate { get; set; }

    public string LastStatus { get; set; } = "Pendiente";

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public bool IsManualOnly => Frequency == BackupFrequency.Manual;

    public string StatusLabel => IsActive ? "Activo" : "Inactivo";

    public string GetFrequencyLabel()
    {
        return Frequency switch
        {
            BackupFrequency.Manual => "Manual solamente",
            BackupFrequency.Daily => "Todos los días",
            BackupFrequency.Weekly => "Semanal",
            BackupFrequency.SpecificDays => "Días específicos",
            BackupFrequency.Monthly => "Mensual",
            _ => "Sin definir"
        };
    }
}

public static class BackupFrequency
{
    public const string Manual = "manual";
    public const string Daily = "daily";
    public const string Weekly = "weekly";
    public const string SpecificDays = "specific_days";
    public const string Monthly = "monthly";

    public static IReadOnlyList<string> All =>
    [
        Manual,
        Daily,
        Weekly,
        SpecificDays,
        Monthly
    ];
}