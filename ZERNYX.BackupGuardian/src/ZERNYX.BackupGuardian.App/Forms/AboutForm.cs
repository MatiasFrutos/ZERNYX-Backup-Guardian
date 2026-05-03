// Archivo: src\ZERNYX.BackupGuardian.App\Forms\AboutForm.cs

using System.Diagnostics;
using ZERNYX.BackupGuardian.App.Theme;

namespace ZERNYX.BackupGuardian.App.Forms;

public sealed class AboutForm : Form
{
    private const string GitHubUrl = "https://github.com/MatiasFrutos";

    public AboutForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AppTheme.ApplyForm(this);

        Text = "Acerca de";
        Size = new Size(1000, 860);
        MinimumSize = new Size(1000, 860);
        MaximizeBox = false;
        MinimizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(34),
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Background
        };

        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));

        Controls.Add(root);

        var card = UiFactory.Card();
        card.Dock = DockStyle.Fill;
        card.Padding = new Padding(52, 44, 52, 44);
        card.Margin = new Padding(0);
        card.BackColor = AppTheme.Surface;

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 12,
            BackColor = AppTheme.Surface
        };

        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));   // título
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));   // slogan
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // versión
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // producto
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // empresa
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // desarrollo
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // tipo
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));   // tecnología
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));   // separador
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // descripción
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));   // github
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));   // cierre

        var title = new Label
        {
            Text = AppConfig.AppName,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var slogan = new Label
        {
            Text = "Software para automatizar copias de seguridad de forma simple, programada y controlada.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        };

        content.Controls.Add(title, 0, 0);
        content.Controls.Add(slogan, 0, 1);

        content.Controls.Add(BuildLine("Versión", AppConfig.AppVersion), 0, 2);
        content.Controls.Add(BuildLine("Producto", "ZERNYX Backup Guardian"), 0, 3);
        content.Controls.Add(BuildLine("Empresa", "ZERNYX Tech Studio"), 0, 4);
        content.Controls.Add(BuildLine("Desarrollo", "Matías Isaac Frutos Gonzalez · ZERNYX Tech Studio"), 0, 5);
        content.Controls.Add(BuildLine("Tipo", "Aplicación de escritorio para Windows"), 0, 6);
        content.Controls.Add(BuildLine("Tecnología", "C# · .NET 8 · Windows Forms · SQLite"), 0, 7);

        var separator = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 18, 0, 14)
        };

        var line = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 1,
            BackColor = AppTheme.Border
        };

        separator.Controls.Add(line);
        content.Controls.Add(separator, 0, 8);

        var descriptionPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 16, 0, 12)
        };

        var description = new Label
        {
            Text =
                "ZERNYX Backup Guardian es una herramienta creada para automatizar una tarea clave: " +
                "realizar copias de seguridad de carpetas importantes de forma ordenada, rápida y controlada.\n\n" +

                "El sistema permite configurar qué carpetas se deben respaldar, dónde guardar la copia, " +
                "qué días ejecutarla y a qué horario. De esta manera, el usuario evita realizar el proceso manualmente " +
                "y mantiene una rutina de respaldo más organizada.\n\n" +

                "Cada ejecución queda registrada en un historial operativo con fecha, estado, cantidad de archivos copiados, " +
                "tamaño respaldado y mensaje técnico. Esto permite revisar rápidamente si los backups se realizaron correctamente " +
                "o si hubo algún error durante el proceso.\n\n" +

                "Su objetivo es ahorrar tiempo, reducir errores operativos y brindar una solución simple para mantener " +
                "control sobre las copias de seguridad.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft
        };

        descriptionPanel.Controls.Add(description);
        content.Controls.Add(descriptionPanel, 0, 9);

        var githubPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.PrimarySoft,
            Padding = new Padding(18, 12, 18, 12),
            Margin = new Padding(0, 8, 0, 8)
        };

        var github = new LinkLabel
        {
            Text = $"GitHub: {GitHubUrl}",
            Dock = DockStyle.Fill,
            LinkColor = AppTheme.Primary,
            ActiveLinkColor = AppTheme.PrimaryDark,
            VisitedLinkColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        github.LinkClicked += (_, _) => OpenGitHub();

        githubPanel.Controls.Add(github);
        content.Controls.Add(githubPanel, 0, 10);

        var footer = new Label
        {
            Text = "Desarrollado por ZERNYX Tech Studio.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.6F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft
        };

        content.Controls.Add(footer, 0, 11);

        card.Controls.Add(content);
        root.Controls.Add(card, 0, 0);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 22, 0, 0),
            BackColor = AppTheme.Background
        };

        var btnClose = UiFactory.PrimaryButton("Cerrar");
        btnClose.Width = 180;
        btnClose.Height = 44;
        btnClose.Click += (_, _) => Close();

        actions.Controls.Add(btnClose);
        root.Controls.Add(actions, 0, 1);
    }

    private static Control BuildLine(string label, string value)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Margin = new Padding(0, 2, 0, 2)
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        var lblLabel = new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var lblValue = new Label
        {
            Text = value,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 8.9F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true
        };

        panel.Controls.Add(lblLabel, 0, 0);
        panel.Controls.Add(lblValue, 1, 0);

        return panel;
    }

    private static void OpenGitHub()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GitHubUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            Clipboard.SetText(GitHubUrl);

            MessageBox.Show(
                "No se pudo abrir el navegador. El enlace fue copiado al portapapeles.",
                AppConfig.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}