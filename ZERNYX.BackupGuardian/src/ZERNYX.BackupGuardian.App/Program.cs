// Archivo: src\ZERNYX.BackupGuardian.App\Program.cs

using ZERNYX.BackupGuardian.App.Forms;
using ZERNYX.BackupGuardian.App.Repositories;

namespace ZERNYX.BackupGuardian.App;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            AppConfig.EnsureAppFolders();
            DatabaseInitializer.Initialize();
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo iniciar ZERNYX Backup Guardian.\n\nDetalle técnico:\n{ex.Message}",
                "ZERNYX Backup Guardian",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}