// Archivo: src\ZERNYX.BackupGuardian.App\Models\BackupSchedule.cs

namespace ZERNYX.BackupGuardian.App.Models;

public sealed class BackupSchedule
{
    public bool Monday { get; set; }

    public bool Tuesday { get; set; }

    public bool Wednesday { get; set; }

    public bool Thursday { get; set; }

    public bool Friday { get; set; }

    public bool Saturday { get; set; }

    public bool Sunday { get; set; }

    public string Time { get; set; } = "18:00";

    public string ToDatabaseValue()
    {
        var days = new List<string>();

        if (Monday) days.Add("Monday");
        if (Tuesday) days.Add("Tuesday");
        if (Wednesday) days.Add("Wednesday");
        if (Thursday) days.Add("Thursday");
        if (Friday) days.Add("Friday");
        if (Saturday) days.Add("Saturday");
        if (Sunday) days.Add("Sunday");

        return string.Join(",", days);
    }

    public static BackupSchedule FromDatabaseValue(string? days, string? time)
    {
        var schedule = new BackupSchedule
        {
            Time = string.IsNullOrWhiteSpace(time) ? "18:00" : time.Trim()
        };

        if (string.IsNullOrWhiteSpace(days))
        {
            return schedule;
        }

        var parts = days
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet();

        schedule.Monday = parts.Contains("monday");
        schedule.Tuesday = parts.Contains("tuesday");
        schedule.Wednesday = parts.Contains("wednesday");
        schedule.Thursday = parts.Contains("thursday");
        schedule.Friday = parts.Contains("friday");
        schedule.Saturday = parts.Contains("saturday");
        schedule.Sunday = parts.Contains("sunday");

        return schedule;
    }

    public bool ContainsDay(DayOfWeek day)
    {
        return day switch
        {
            DayOfWeek.Monday => Monday,
            DayOfWeek.Tuesday => Tuesday,
            DayOfWeek.Wednesday => Wednesday,
            DayOfWeek.Thursday => Thursday,
            DayOfWeek.Friday => Friday,
            DayOfWeek.Saturday => Saturday,
            DayOfWeek.Sunday => Sunday,
            _ => false
        };
    }

    public string ToDisplayText()
    {
        var days = new List<string>();

        if (Monday) days.Add("Lunes");
        if (Tuesday) days.Add("Martes");
        if (Wednesday) days.Add("Miércoles");
        if (Thursday) days.Add("Jueves");
        if (Friday) days.Add("Viernes");
        if (Saturday) days.Add("Sábado");
        if (Sunday) days.Add("Domingo");

        if (days.Count == 0)
        {
            return $"Sin días definidos · {Time}";
        }

        return $"{string.Join(", ", days)} · {Time}";
    }
}