// Archivo: src\ZERNYX.BackupGuardian.App\AppConfig.cs

namespace ZERNYX.BackupGuardian.App;

public static class AppConfig
{
    public const string AppName = "ZERNYX Backup Guardian";
    public const string AppVersion = "1.0.0";
    public const string CompanyName = "ZERNYX Tech Studio";
    public const string Slogan = "Protegé tus archivos antes de que sea tarde.";

    public static string BaseDirectory => AppDomain.CurrentDomain.BaseDirectory;

    public static string DataDirectory => Path.Combine(BaseDirectory, "data");

    public static string LogsDirectory => Path.Combine(BaseDirectory, "logs");

    public static string ReportsDirectory => Path.Combine(BaseDirectory, "reports");

    public static string StorageDirectory => Path.Combine(BaseDirectory, "storage");

    public static string DatabasePath => Path.Combine(DataDirectory, "zernyx_backup_guardian.db");

    public static string LogFilePath => Path.Combine(LogsDirectory, $"app_{DateTime.Now:yyyyMMdd}.log");

    public static string DefaultReportFilePath =>
        Path.Combine(ReportsDirectory, $"backup_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

    public static void EnsureAppFolders()
    {
        Directory.CreateDirectory(DataDirectory);
        Directory.CreateDirectory(LogsDirectory);
        Directory.CreateDirectory(ReportsDirectory);
        Directory.CreateDirectory(StorageDirectory);
    }
}