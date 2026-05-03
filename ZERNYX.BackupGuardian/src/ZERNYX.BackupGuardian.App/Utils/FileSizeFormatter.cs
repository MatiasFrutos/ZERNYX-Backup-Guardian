// Archivo: src\ZERNYX.BackupGuardian.App\Utils\FileSizeFormatter.cs

namespace ZERNYX.BackupGuardian.App.Utils;

public static class FileSizeFormatter
{
    public static string Format(long bytes)
    {
        if (bytes < 0)
        {
            return "0 B";
        }

        string[] sizes = ["B", "KB", "MB", "GB", "TB"];

        double len = bytes;
        var order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}