// Archivo: src\ZERNYX.BackupGuardian.App\Services\SettingsService.cs

using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;

namespace ZERNYX.BackupGuardian.App.Services;

public sealed class SettingsService
{
    private readonly SettingsRepository _settingsRepository;

    public SettingsService()
    {
        _settingsRepository = new SettingsRepository();
    }

    public AppSettings GetSettings()
    {
        return _settingsRepository.Get();
    }

    public void SaveSettings(AppSettings settings)
    {
        Validate(settings);
        _settingsRepository.Save(settings);
        LogService.Info("Configuración guardada.");
    }

    private static void Validate(AppSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.ClientName))
        {
            settings.ClientName = "Cliente";
        }

        if (string.IsNullOrWhiteSpace(settings.TechnicianName))
        {
            settings.TechnicianName = "ZERNYX Tech Studio";
        }

        if (settings.RetentionDays < 1)
        {
            settings.RetentionDays = 30;
        }

        if (settings.RetentionDays > 3650)
        {
            settings.RetentionDays = 3650;
        }

        settings.UpdatedAt = DateTime.Now;
    }
}