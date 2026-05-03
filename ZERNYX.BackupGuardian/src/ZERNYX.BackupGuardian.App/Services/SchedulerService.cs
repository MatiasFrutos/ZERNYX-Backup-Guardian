// Archivo: src\ZERNYX.BackupGuardian.App\Services\SchedulerService.cs

using System.Windows.Forms;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;

namespace ZERNYX.BackupGuardian.App.Services;

public sealed class SchedulerService : IDisposable
{
    private readonly BackupTaskRepository _taskRepository;
    private readonly BackupService _backupService;
    private readonly System.Windows.Forms.Timer _timer;

    private bool _isRunning;
    private bool _disposed;

    public event EventHandler<string>? StatusChanged;
    public event EventHandler<BackupResult>? BackupExecuted;

    public SchedulerService()
    {
        _taskRepository = new BackupTaskRepository();
        _backupService = new BackupService();

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 60_000
        };

        _timer.Tick += (_, _) => CheckScheduledTasks();
    }

    public void Start()
    {
        if (_disposed)
        {
            return;
        }

        _timer.Start();
        StatusChanged?.Invoke(this, "Programador activo");
        LogService.Info("SchedulerService iniciado.");
        CheckScheduledTasks();
    }

    public void Stop()
    {
        if (_disposed)
        {
            return;
        }

        _timer.Stop();
        StatusChanged?.Invoke(this, "Programador detenido");
        LogService.Info("SchedulerService detenido.");
    }

    public void CheckScheduledTasks()
    {
        if (_isRunning || _disposed)
        {
            return;
        }

        try
        {
            _isRunning = true;

            var now = DateTime.Now;
            var tasks = _taskRepository.GetActive();

            foreach (var task in tasks)
            {
                if (!ShouldRun(task, now))
                {
                    continue;
                }

                StatusChanged?.Invoke(this, $"Ejecutando backup programado: {task.Name}");

                var result = _backupService.ExecuteBackup(task, BackupTriggerType.Scheduled);

                _taskRepository.MarkScheduledRun(task.Id, now.Date);

                BackupExecuted?.Invoke(this, result);

                StatusChanged?.Invoke(
                    this,
                    result.Success
                        ? $"Backup programado exitoso: {task.Name}"
                        : $"Backup programado con error: {task.Name}"
                );
            }
        }
        catch (Exception ex)
        {
            LogService.Error("Error revisando tareas programadas.", ex);
            StatusChanged?.Invoke(this, "Error en programador de backups");
        }
        finally
        {
            _isRunning = false;
        }
    }

    private static bool ShouldRun(BackupTask task, DateTime now)
    {
        if (!task.IsActive)
        {
            return false;
        }

        if (task.Frequency == BackupFrequency.Manual)
        {
            return false;
        }

        if (task.LastScheduledRunDate.HasValue &&
            task.LastScheduledRunDate.Value.Date == now.Date)
        {
            return false;
        }

        if (!TimeMatches(task.ScheduledTime, now))
        {
            return false;
        }

        return task.Frequency switch
        {
            BackupFrequency.Daily => true,
            BackupFrequency.SpecificDays => ShouldRunSpecificDays(task, now),
            BackupFrequency.Weekly => ShouldRunWeekly(task, now),
            BackupFrequency.Monthly => now.Day == 1,
            _ => false
        };
    }

    private static bool TimeMatches(string scheduledTime, DateTime now)
    {
        if (!TimeSpan.TryParse(scheduledTime, out var time))
        {
            return false;
        }

        var scheduled = now.Date.Add(time);
        var difference = Math.Abs((now - scheduled).TotalMinutes);

        return difference <= 1;
    }

    private static bool ShouldRunSpecificDays(BackupTask task, DateTime now)
    {
        var schedule = BackupSchedule.FromDatabaseValue(task.ScheduledDays, task.ScheduledTime);
        return schedule.ContainsDay(now.DayOfWeek);
    }

    private static bool ShouldRunWeekly(BackupTask task, DateTime now)
    {
        if (string.IsNullOrWhiteSpace(task.ScheduledDays))
        {
            return now.DayOfWeek == DayOfWeek.Monday;
        }

        var schedule = BackupSchedule.FromDatabaseValue(task.ScheduledDays, task.ScheduledTime);
        return schedule.ContainsDay(now.DayOfWeek);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _timer.Stop();
        _timer.Dispose();
        _disposed = true;
    }
}