// Archivo: src\ZERNYX.BackupGuardian.App\Services\ReportService.cs

using System.Text;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;
using ZERNYX.BackupGuardian.App.Utils;

namespace ZERNYX.BackupGuardian.App.Services;

public sealed class ReportService
{
    private readonly BackupHistoryRepository _historyRepository;

    public ReportService()
    {
        _historyRepository = new BackupHistoryRepository();
    }

    public string ExportHistoryToTxt(string? outputPath = null, int limit = 500)
    {
        AppConfig.EnsureAppFolders();

        var histories = _historyRepository.GetLatest(limit);
        var path = string.IsNullOrWhiteSpace(outputPath)
            ? AppConfig.DefaultReportFilePath
            : outputPath;

        var sb = new StringBuilder();

        sb.AppendLine("============================================================");
        sb.AppendLine(" ZERNYX BACKUP GUARDIAN - REPORTE DE HISTORIAL");
        sb.AppendLine("============================================================");
        sb.AppendLine($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
        sb.AppendLine($"Cantidad de registros: {histories.Count}");
        sb.AppendLine("============================================================");
        sb.AppendLine();

        foreach (var item in histories)
        {
            sb.AppendLine($"ID: {item.Id}");
            sb.AppendLine($"Tarea: {item.TaskName}");
            sb.AppendLine($"Tipo: {BackupTriggerType.ToDisplayName(item.TriggerType)}");
            sb.AppendLine($"Estado: {BackupStatus.ToDisplayName(item.Status)}");
            sb.AppendLine($"Inicio: {item.StartedAt:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Fin: {(item.FinishedAt.HasValue ? item.FinishedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : "-")}");
            sb.AppendLine($"Duración: {item.DurationSeconds} segundos");
            sb.AppendLine($"Origen: {item.SourcePath}");
            sb.AppendLine($"Destino base: {item.DestinationPath}");
            sb.AppendLine($"Destino final: {item.FinalBackupPath}");
            sb.AppendLine($"Archivos copiados: {item.FilesCopied}");
            sb.AppendLine($"Archivos omitidos: {item.FilesSkipped}");
            sb.AppendLine($"Archivos fallidos: {item.FilesFailed}");
            sb.AppendLine($"Tamaño copiado: {FileSizeFormatter.Format(item.TotalBytesCopied)}");
            sb.AppendLine($"Mensaje: {item.Message}");

            if (!string.IsNullOrWhiteSpace(item.ErrorDetails))
            {
                sb.AppendLine("Errores:");
                sb.AppendLine(item.ErrorDetails);
            }

            sb.AppendLine("------------------------------------------------------------");
        }

        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);

        LogService.Info($"Reporte exportado: {path}");

        return path;
    }
}