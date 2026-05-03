// Archivo: src\ZERNYX.BackupGuardian.App\Utils\DateTimeHelper.cs

namespace ZERNYX.BackupGuardian.App.Utils;

public static class DateTimeHelper
{
    public static string FormatDateTime(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("dd/MM/yyyy HH:mm") : "-";
    }

    public static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("dd/MM/yyyy") : "-";
    }

    public static string FormatTime(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("HH:mm") : "-";
    }

    public static string FormatDuration(int seconds)
    {
        if (seconds <= 0)
        {
            return "0s";
        }

        var ts = TimeSpan.FromSeconds(seconds);

        if (ts.TotalHours >= 1)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        }

        if (ts.TotalMinutes >= 1)
        {
            return $"{ts.Minutes}m {ts.Seconds}s";
        }

        return $"{ts.Seconds}s";
    }

    public static bool IsValidTime(string value)
    {
        return TimeSpan.TryParse(value, out _);
    }
}