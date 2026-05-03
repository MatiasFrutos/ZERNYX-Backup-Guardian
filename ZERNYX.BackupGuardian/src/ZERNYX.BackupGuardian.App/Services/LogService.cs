// Archivo: src\ZERNYX.BackupGuardian.App\Services\LogService.cs

namespace ZERNYX.BackupGuardian.App.Services;

public static class LogService
{
    private static readonly object LockObject = new();

    public static void Info(string message)
    {
        Write("INFO", message, null);
    }

    public static void Warning(string message)
    {
        Write("WARN", message, null);
    }

    public static void Error(string message, Exception? exception = null)
    {
        Write("ERROR", message, exception);
    }

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            AppConfig.EnsureAppFolders();

            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

            if (exception is not null)
            {
                line += Environment.NewLine;
                line += exception.ToString();
            }

            lock (LockObject)
            {
                File.AppendAllText(AppConfig.LogFilePath, line + Environment.NewLine);
            }
        }
        catch
        {
            // No hacemos nada para evitar romper la app por un error de log.
        }
    }
}