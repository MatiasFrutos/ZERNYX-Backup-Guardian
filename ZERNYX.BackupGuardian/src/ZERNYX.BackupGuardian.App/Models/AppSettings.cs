// Archivo: src\ZERNYX.BackupGuardian.App\Models\AppSettings.cs

namespace ZERNYX.BackupGuardian.App.Models;

public sealed class AppSettings
{
    public int Id { get; set; } = 1;

    public string ClientName { get; set; } = "Cliente";

    public string TechnicianName { get; set; } = "ZERNYX Tech Studio";

    public string DefaultBackupDestination { get; set; } = string.Empty;

    public bool StartMinimized { get; set; }

    public bool StartWithWindows { get; set; }

    public bool EnableNotifications { get; set; } = true;

    public bool EnableScheduler { get; set; } = true;

    public bool DarkMode { get; set; }

    public int RetentionDays { get; set; } = 30;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}