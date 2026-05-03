// Archivo: src\ZERNYX.BackupGuardian.App\Utils\PathHelper.cs

namespace ZERNYX.BackupGuardian.App.Utils;

public static class PathHelper
{
    public static bool IsValidDirectory(string path)
    {
        return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
    }

    public static string ShortenPath(string path, int maxLength = 70)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return "-";
        }

        if (path.Length <= maxLength)
        {
            return path;
        }

        var root = Path.GetPathRoot(path) ?? string.Empty;
        var fileName = Path.GetFileName(path);

        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).LastOrDefault() ?? "";
        }

        var result = $"{root}...\\{fileName}";

        if (result.Length <= maxLength)
        {
            return result;
        }

        return "..." + path[^Math.Min(maxLength - 3, path.Length)..];
    }

    public static string MakeSafeFileName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "archivo";
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());

        return string.IsNullOrWhiteSpace(safe) ? "archivo" : safe.Trim();
    }

    public static string EnsureTrailingSlash(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        if (path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar))
        {
            return path;
        }

        return path + Path.DirectorySeparatorChar;
    }
}