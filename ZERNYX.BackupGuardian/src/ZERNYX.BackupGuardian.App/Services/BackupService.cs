// Archivo: src\ZERNYX.BackupGuardian.App\Services\BackupService.cs

using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;

namespace ZERNYX.BackupGuardian.App.Services;

public sealed class BackupService
{
    private readonly BackupHistoryRepository _historyRepository;
    private readonly BackupTaskRepository _taskRepository;

    public BackupService()
    {
        _historyRepository = new BackupHistoryRepository();
        _taskRepository = new BackupTaskRepository();
    }

    public BackupResult ExecuteBackup(BackupTask task, string triggerType)
    {
        var result = new BackupResult
        {
            BackupTaskId = task.Id,
            TaskName = task.Name,
            SourcePath = task.SourcePath,
            DestinationPath = task.DestinationPath,
            StartedAt = DateTime.Now
        };

        try
        {
            LogService.Info($"Iniciando backup: {task.Name}");

            ValidateTask(task);

            var finalDestination = BuildFinalDestination(task);
            result.FinalBackupPath = finalDestination;

            Directory.CreateDirectory(finalDestination);

            CopyDirectory(
                task.SourcePath,
                finalDestination,
                task.IncludeSubfolders,
                task.OverwriteExistingFiles,
                result
            );

            result.Success = result.FilesFailed == 0;
            result.FinishedAt = DateTime.Now;

            result.Message = result.Success
                ? $"Backup finalizado correctamente. Archivos copiados: {result.FilesCopied}."
                : $"Backup finalizado con errores. Copiados: {result.FilesCopied}. Fallidos: {result.FilesFailed}.";

            _historyRepository.Create(result.ToHistory(triggerType));

            _taskRepository.UpdateRunStatus(
                task.Id,
                result.Success ? "Exitoso" : "Error",
                result.FinishedAt,
                result.Success
            );

            LogService.Info(result.Message);

            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.FinishedAt = DateTime.Now;
            result.Message = "No se pudo completar el backup.";
            result.ErrorDetails = ex.Message;

            try
            {
                _historyRepository.Create(result.ToHistory(triggerType));

                _taskRepository.UpdateRunStatus(
                    task.Id,
                    "Error",
                    result.FinishedAt,
                    false
                );
            }
            catch (Exception dbEx)
            {
                LogService.Error("Error registrando historial de backup.", dbEx);
            }

            LogService.Error($"Error ejecutando backup: {task.Name}", ex);

            return result;
        }
    }

    private static void ValidateTask(BackupTask task)
    {
        if (task.Id <= 0)
        {
            throw new InvalidOperationException("La tarea no tiene un ID válido.");
        }

        if (string.IsNullOrWhiteSpace(task.Name))
        {
            throw new InvalidOperationException("La tarea no tiene nombre.");
        }

        if (string.IsNullOrWhiteSpace(task.SourcePath))
        {
            throw new InvalidOperationException("La carpeta origen no está definida.");
        }

        if (string.IsNullOrWhiteSpace(task.DestinationPath))
        {
            throw new InvalidOperationException("La carpeta destino no está definida.");
        }

        if (!Directory.Exists(task.SourcePath))
        {
            throw new DirectoryNotFoundException($"No existe la carpeta origen: {task.SourcePath}");
        }

        if (!Directory.Exists(task.DestinationPath))
        {
            Directory.CreateDirectory(task.DestinationPath);
        }
    }

    private static string BuildFinalDestination(BackupTask task)
    {
        var safeName = MakeSafeFolderName(task.Name);

        if (!task.CreateDateFolder)
        {
            return Path.Combine(task.DestinationPath, safeName);
        }

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        return Path.Combine(task.DestinationPath, $"{safeName}_{stamp}");
    }

    private static void CopyDirectory(
        string sourceDir,
        string destinationDir,
        bool includeSubfolders,
        bool overwrite,
        BackupResult result
    )
    {
        var source = new DirectoryInfo(sourceDir);

        if (!source.Exists)
        {
            throw new DirectoryNotFoundException($"No existe la carpeta origen: {source.FullName}");
        }

        Directory.CreateDirectory(destinationDir);

        foreach (var file in source.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);

            try
            {
                if (File.Exists(targetFilePath) && !overwrite)
                {
                    result.FilesSkipped++;
                    continue;
                }

                file.CopyTo(targetFilePath, overwrite);
                result.FilesCopied++;
                result.TotalBytesCopied += file.Length;
            }
            catch (Exception ex)
            {
                result.FilesFailed++;
                result.ErrorDetails += $"Archivo: {file.FullName} | Error: {ex.Message}{Environment.NewLine}";
                LogService.Error($"Error copiando archivo: {file.FullName}", ex);
            }
        }

        if (!includeSubfolders)
        {
            return;
        }

        foreach (var subDir in source.GetDirectories())
        {
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);

            try
            {
                CopyDirectory(
                    subDir.FullName,
                    newDestinationDir,
                    includeSubfolders,
                    overwrite,
                    result
                );
            }
            catch (Exception ex)
            {
                result.FilesFailed++;
                result.ErrorDetails += $"Carpeta: {subDir.FullName} | Error: {ex.Message}{Environment.NewLine}";
                LogService.Error($"Error copiando carpeta: {subDir.FullName}", ex);
            }
        }
    }

    private static string MakeSafeFolderName(string value)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safe = new string(value.Select(ch => invalidChars.Contains(ch) ? '_' : ch).ToArray());

        safe = safe.Trim();

        if (string.IsNullOrWhiteSpace(safe))
        {
            return "Backup";
        }

        return safe;
    }
}