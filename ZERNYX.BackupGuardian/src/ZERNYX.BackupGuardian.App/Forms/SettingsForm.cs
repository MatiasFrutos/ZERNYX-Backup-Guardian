// Archivo: src\ZERNYX.BackupGuardian.App\Forms\SettingsForm.cs

using System.Drawing.Drawing2D;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Services;
using ZERNYX.BackupGuardian.App.Theme;

namespace ZERNYX.BackupGuardian.App.Forms;

public sealed class SettingsForm : Form
{
    private readonly SettingsService _settingsService;
    private AppSettings _settings = null!;

    private TextBox _txtClient = null!;
    private TextBox _txtTechnician = null!;
    private TextBox _txtDefaultDestination = null!;
    private NumericUpDown _numRetention = null!;

    private CheckBox _chkNotifications = null!;
    private CheckBox _chkScheduler = null!;
    private CheckBox _chkStartMinimized = null!;
    private CheckBox _chkStartWithWindows = null!;

    public SettingsForm()
    {
        _settingsService = new SettingsService();

        InitializeComponent();

        Load += (_, _) =>
        {
            _settings = _settingsService.GetSettings();
            LoadSettings();
        };
    }

    private void InitializeComponent()
    {
        AppTheme.ApplyForm(this);

        Text = "Configuración";
        Size = new Size(1480, 940);
        MinimumSize = new Size(1480, 940);
        MaximizeBox = false;
        MinimizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(42, 34, 42, 30),
            ColumnCount = 1,
            RowCount = 3,
            BackColor = AppTheme.Background
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));

        Controls.Add(root);

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildBody(), 0, 1);
        root.Controls.Add(BuildActions(), 0, 2);
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 0, 0, 18)
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Background,
            Padding = new Padding(6, 6, 0, 0)
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        titleBox.Controls.Add(new Label
        {
            Text = "Configuración",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = "Ajustes generales, automatización y preferencias operativas.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 1);

        var badgeHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background
        };

        var badge = new ModernPanel
        {
            Width = 250,
            Height = 36,
            Radius = 16,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(10),
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(74, 22)
        };

        badge.Controls.Add(new Label
        {
            Text = "ZERNYX Backup Guardian · v1.0.0",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 7.9F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface
        });

        badgeHost.Controls.Add(badge);

        header.Controls.Add(titleBox, 0, 0);
        header.Controls.Add(badgeHost, 1, 0);

        return header;
    }

    private Control BuildBody()
    {
        var body = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background
        };

        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 54));
        body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 46));

        body.Controls.Add(BuildLeftPanel(), 0, 0);
        body.Controls.Add(BuildRightPanel(), 1, 0);

        return body;
    }

    private Control BuildLeftPanel()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 24,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(30, 24, 30, 20),
            Margin = new Padding(0, 0, 18, 0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 10,
            BackColor = AppTheme.Surface
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 6));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleGeneral = BuildSectionTitle(
            "Datos generales",
            "Información base para identificar el cliente y el responsable técnico."
        );

        layout.Controls.Add(titleGeneral, 0, 0);
        layout.SetColumnSpan(titleGeneral, 2);

        layout.Controls.Add(BuildModernInput("Cliente", "Nombre del cliente", out _txtClient), 0, 1);
        layout.Controls.Add(BuildModernInput("Técnico / empresa", "Responsable técnico", out _txtTechnician), 1, 1);

        var spacer1 = BuildSpacer();
        layout.Controls.Add(spacer1, 0, 2);
        layout.SetColumnSpan(spacer1, 2);

        var titleBackup = BuildSectionTitle(
            "Configuración de respaldo",
            "Definí dónde se guardan las copias y cómo se organiza la operación."
        );

        layout.Controls.Add(titleBackup, 0, 3);
        layout.SetColumnSpan(titleBackup, 2);

        var destination = BuildDestinationInput();
        layout.Controls.Add(destination, 0, 4);
        layout.SetColumnSpan(destination, 2);

        var spacer2 = BuildSpacer();
        layout.Controls.Add(spacer2, 0, 5);
        layout.SetColumnSpan(spacer2, 2);

        var titleRetention = BuildSectionTitle(
            "Historial operativo",
            "Control local del período de conservación de registros."
        );

        layout.Controls.Add(titleRetention, 0, 6);
        layout.SetColumnSpan(titleRetention, 2);

        layout.Controls.Add(BuildRetentionInput(), 0, 7);

        var info = BuildMiniInfo(
            "Dato útil",
            "La base SQLite se crea localmente y mantiene historial, tareas, resultados y preferencias."
        );

        layout.Controls.Add(info, 1, 7);

        card.Controls.Add(layout);

        return card;
    }

    private Control BuildRightPanel()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 24,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(30, 26, 30, 26),
            Margin = new Padding(18, 0, 0, 0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 9,
            BackColor = AppTheme.Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 106));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var titleAutomation = BuildSectionTitle(
            "Automatización y preferencias",
            "Opciones de ejecución automática e inicio del sistema."
        );

        layout.Controls.Add(titleAutomation, 0, 0);

        _chkNotifications = BuildModernCheckBox(
            "Notificaciones",
            "Mostrar avisos cuando se ejecutan tareas o aparecen errores."
        );

        layout.Controls.Add(BuildOptionCard(_chkNotifications), 0, 1);
        layout.Controls.Add(BuildSpacer(), 0, 2);

        _chkScheduler = BuildModernCheckBox(
            "Programador automático",
            "Permite ejecutar backups según días y horarios configurados."
        );

        layout.Controls.Add(BuildOptionCard(_chkScheduler), 0, 3);
        layout.Controls.Add(BuildSpacer(), 0, 4);

        var dualRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 12)
        };

        dualRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        dualRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        _chkStartMinimized = BuildModernCheckBox(
            "Iniciar minimizado",
            "Abrir la app de forma discreta."
        );

        _chkStartWithWindows = BuildModernCheckBox(
            "Iniciar con Windows",
            "Preparar el inicio con el sistema."
        );

        dualRow.Controls.Add(BuildOptionCard(_chkStartMinimized, new Padding(0, 0, 14, 0)), 0, 0);
        dualRow.Controls.Add(BuildOptionCard(_chkStartWithWindows, new Padding(14, 0, 0, 0)), 1, 0);

        layout.Controls.Add(dualRow, 0, 5);
        layout.Controls.Add(BuildSpacer(), 0, 6);

        var note = BuildMiniInfo(
            "Nota operativa",
            "En esta versión, el programador automático funciona mientras la aplicación permanece abierta."
        );

        layout.Controls.Add(note, 0, 7);

        card.Controls.Add(layout);

        return card;
    }

    private Control BuildActions()
    {
        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 16, 0, 0)
        };

        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));

        actions.Controls.Add(new Label
        {
            Text = "Los cambios se guardan localmente y se aplican al comportamiento general del sistema.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.3F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 0);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 12, 0, 0),
            BackColor = AppTheme.Background
        };

        var btnSave = CreateModernButton(
            "Guardar",
            AppTheme.Primary,
            Color.White,
            155,
            44,
            AppTheme.PrimaryDark,
            Color.White,
            AppTheme.Primary
        );

        btnSave.Click += (_, _) => Save();

        var btnCancel = CreateModernButton(
            "Cancelar",
            AppTheme.Surface,
            AppTheme.Text,
            155,
            44,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnCancel.Click += (_, _) => Close();

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        actions.Controls.Add(buttons, 1, 0);

        return actions;
    }

    private static Control BuildSectionTitle(string title, string subtitle)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface,
            Margin = new Padding(0, 0, 0, 8)
        };

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        panel.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        panel.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        return panel;
    }

    private static Control BuildSpacer()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface
        };
    }

    private static Control BuildModernInput(string label, string placeholder, out TextBox textBox)
    {
        var wrapper = CreateFieldWrapper(label, new Padding(0, 4, 16, 10));

        var fieldHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 4)
        };

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Top,
            Height = 42,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 14,
            Padding = new Padding(13, 10, 13, 8)
        };

        textBox = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 8.9F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            PlaceholderText = placeholder,
            Dock = DockStyle.Fill
        };

        field.BindFocus(textBox);
        field.Controls.Add(textBox);
        fieldHost.Controls.Add(field);

        wrapper.Controls.Add(fieldHost, 0, 1);

        return wrapper;
    }

    private Control BuildDestinationInput()
    {
        var wrapper = CreateFieldWrapper("Destino por defecto", new Padding(0, 4, 0, 10));

        var row = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 4)
        };

        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 118));

        var fieldHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 16, 0)
        };

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Top,
            Height = 42,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 14,
            Padding = new Padding(13, 10, 13, 8)
        };

        _txtDefaultDestination = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 8.9F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            PlaceholderText = "Carpeta destino por defecto",
            Dock = DockStyle.Fill
        };

        field.BindFocus(_txtDefaultDestination);
        field.Controls.Add(_txtDefaultDestination);
        fieldHost.Controls.Add(field);

        var btnBrowse = CreateModernButton(
            "Buscar",
            AppTheme.Surface,
            AppTheme.Text,
            98,
            42,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnBrowse.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        btnBrowse.Margin = new Padding(0);
        btnBrowse.Click += (_, _) => SelectDestination();

        var btnHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0)
        };

        btnHost.Controls.Add(btnBrowse);

        row.Controls.Add(fieldHost, 0, 0);
        row.Controls.Add(btnHost, 1, 0);

        wrapper.Controls.Add(row, 0, 1);

        return wrapper;
    }

    private Control BuildRetentionInput()
    {
        var wrapper = CreateFieldWrapper("Retención de registros", new Padding(0, 4, 16, 8));

        var fieldHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface
        };

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 14,
            Padding = new Padding(13, 8, 13, 7)
        };

        _numRetention = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 3650,
            Value = 30,
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 8.9F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            Dock = DockStyle.Left,
            Width = 110
        };

        var help = new Label
        {
            Text = "días",
            Dock = DockStyle.Right,
            Width = 60,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.9F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = AppTheme.SurfaceSoft
        };

        field.BindFocus(_numRetention);
        field.Controls.Add(_numRetention);
        field.Controls.Add(help);
        fieldHost.Controls.Add(field);

        wrapper.Controls.Add(fieldHost, 0, 1);

        return wrapper;
    }

    private static TableLayoutPanel CreateFieldWrapper(string label, Padding margin)
    {
        var wrapper = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface,
            Margin = margin
        };

        wrapper.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        wrapper.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        wrapper.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        return wrapper;
    }

    private static CheckBox BuildModernCheckBox(string text, string tooltip)
    {
        return new CheckBox
        {
            Text = text,
            Tag = tooltip,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8.4F, FontStyle.Bold),
            ForeColor = AppTheme.Text,
            Margin = new Padding(0),
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Standard,
            BackColor = AppTheme.Surface
        };
    }

    private static Control BuildOptionCard(CheckBox checkBox)
    {
        return BuildOptionCard(checkBox, new Padding(0));
    }

    private static Control BuildOptionCard(CheckBox checkBox, Padding margin)
    {
        var card = new AnimatedOptionPanel
        {
            Dock = DockStyle.Fill,
            Radius = 16,
            BackColor = AppTheme.Surface,
            Margin = margin,
            Padding = new Padding(16, 12, 16, 12)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        checkBox.BackColor = AppTheme.Surface;

        layout.Controls.Add(checkBox, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = checkBox.Tag?.ToString() ?? "",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.6F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        card.BindHover(layout);
        card.Controls.Add(layout);

        return card;
    }

    private static Control BuildMiniInfo(string title, string text)
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 16,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(14, 10, 14, 10),
            Margin = new Padding(0, 4, 0, 8)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 8.1F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.3F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        card.Controls.Add(layout);

        return card;
    }

    private static Button CreateModernButton(
        string text,
        Color backColor,
        Color foreColor,
        int width,
        int height,
        Color hoverBackColor,
        Color hoverForeColor,
        Color borderColor)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Segoe UI", 8.6F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(8, 0, 0, 0),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = borderColor;
        button.FlatAppearance.MouseOverBackColor = hoverBackColor;
        button.FlatAppearance.MouseDownBackColor = hoverBackColor;

        button.MouseEnter += (_, _) =>
        {
            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
            button.FlatAppearance.BorderColor = borderColor;
        };

        button.MouseLeave += (_, _) =>
        {
            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatAppearance.BorderColor = borderColor;
        };

        button.MouseDown += (_, _) =>
        {
            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
        };

        button.MouseUp += (_, _) =>
        {
            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
        };

        return button;
    }

    private void LoadSettings()
    {
        _txtClient.Text = _settings.ClientName;
        _txtTechnician.Text = _settings.TechnicianName;
        _txtDefaultDestination.Text = _settings.DefaultBackupDestination;
        _numRetention.Value = Math.Clamp(_settings.RetentionDays, 1, 3650);

        _chkNotifications.Checked = _settings.EnableNotifications;
        _chkScheduler.Checked = _settings.EnableScheduler;
        _chkStartMinimized.Checked = _settings.StartMinimized;
        _chkStartWithWindows.Checked = _settings.StartWithWindows;
    }

    private void Save()
    {
        _settings.ClientName = _txtClient.Text.Trim();
        _settings.TechnicianName = _txtTechnician.Text.Trim();
        _settings.DefaultBackupDestination = _txtDefaultDestination.Text.Trim();
        _settings.RetentionDays = (int)_numRetention.Value;

        _settings.EnableNotifications = _chkNotifications.Checked;
        _settings.EnableScheduler = _chkScheduler.Checked;
        _settings.StartMinimized = _chkStartMinimized.Checked;
        _settings.StartWithWindows = _chkStartWithWindows.Checked;
        _settings.DarkMode = false;

        _settingsService.SaveSettings(_settings);

        MessageBox.Show(
            "Configuración guardada correctamente.",
            AppConfig.AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );

        DialogResult = DialogResult.OK;
        Close();
    }

    private void SelectDestination()
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Seleccionar carpeta destino por defecto",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        if (!string.IsNullOrWhiteSpace(_txtDefaultDestination.Text) &&
            Directory.Exists(_txtDefaultDestination.Text))
        {
            dialog.SelectedPath = _txtDefaultDestination.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _txtDefaultDestination.Text = dialog.SelectedPath;
        }
    }

    private sealed class ModernPanel : Panel
    {
        public int Radius { get; set; } = 20;
        public Color BorderColor { get; set; } = AppTheme.Border;

        public ModernPanel()
        {
            DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawRoundedPanel(e.Graphics, ClientRectangle, Radius, BackColor, BorderColor);
        }
    }

    private sealed class AnimatedInputPanel : Panel
    {
        private readonly System.Windows.Forms.Timer _timer;
        private Color _currentBorder;
        private Color _targetBorder;

        public int Radius { get; set; } = 14;

        public AnimatedInputPanel()
        {
            DoubleBuffered = true;

            _currentBorder = AppTheme.Border;
            _targetBorder = AppTheme.Border;

            _timer = new System.Windows.Forms.Timer
            {
                Interval = 15
            };

            _timer.Tick += (_, _) =>
            {
                _currentBorder = LerpColor(_currentBorder, _targetBorder, 0.22F);
                Invalidate();

                if (ColorDistance(_currentBorder, _targetBorder) < 4)
                {
                    _currentBorder = _targetBorder;
                    _timer.Stop();
                    Invalidate();
                }
            };
        }

        public void BindFocus(Control control)
        {
            control.GotFocus += (_, _) => AnimateTo(AppTheme.Primary);
            control.LostFocus += (_, _) => AnimateTo(AppTheme.Border);

            control.MouseEnter += (_, _) =>
            {
                if (!control.Focused)
                {
                    AnimateTo(Color.FromArgb(196, 181, 253));
                }
            };

            control.MouseLeave += (_, _) =>
            {
                if (!control.Focused)
                {
                    AnimateTo(AppTheme.Border);
                }
            };
        }

        private void AnimateTo(Color color)
        {
            _targetBorder = color;
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawRoundedPanel(e.Graphics, ClientRectangle, Radius, BackColor, _currentBorder);
        }
    }

    private sealed class AnimatedOptionPanel : Panel
    {
        private readonly System.Windows.Forms.Timer _timer;
        private Color _currentBorder;
        private Color _targetBorder;
        private Color _currentBack;
        private Color _targetBack;

        public int Radius { get; set; } = 16;

        public AnimatedOptionPanel()
        {
            DoubleBuffered = true;

            _currentBorder = AppTheme.Border;
            _targetBorder = AppTheme.Border;
            _currentBack = AppTheme.Surface;
            _targetBack = AppTheme.Surface;

            _timer = new System.Windows.Forms.Timer
            {
                Interval = 15
            };

            _timer.Tick += (_, _) =>
            {
                _currentBorder = LerpColor(_currentBorder, _targetBorder, 0.20F);
                _currentBack = LerpColor(_currentBack, _targetBack, 0.20F);
                BackColor = _currentBack;
                UpdateChildBackColors(this, _currentBack);
                Invalidate();

                if (ColorDistance(_currentBorder, _targetBorder) < 4 &&
                    ColorDistance(_currentBack, _targetBack) < 4)
                {
                    _currentBorder = _targetBorder;
                    _currentBack = _targetBack;
                    BackColor = _currentBack;
                    UpdateChildBackColors(this, _currentBack);
                    _timer.Stop();
                    Invalidate();
                }
            };
        }

        public void BindHover(Control child)
        {
            MouseEnter += (_, _) => Animate(true);
            MouseLeave += (_, _) => Animate(false);

            child.MouseEnter += (_, _) => Animate(true);
            child.MouseLeave += (_, _) => Animate(false);

            foreach (Control item in child.Controls)
            {
                item.MouseEnter += (_, _) => Animate(true);
                item.MouseLeave += (_, _) => Animate(false);
            }
        }

        private void Animate(bool hover)
        {
            _targetBorder = AppTheme.Border;
            _targetBack = hover ? AppTheme.SurfaceSoft : AppTheme.Surface;
            _timer.Start();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            BackColor = _currentBack;
            base.OnPaint(e);
            DrawRoundedPanel(e.Graphics, ClientRectangle, Radius, _currentBack, _currentBorder);
        }

        private static void UpdateChildBackColors(Control parent, Color color)
        {
            foreach (Control child in parent.Controls)
            {
                child.BackColor = color;
                UpdateChildBackColors(child, color);
            }
        }
    }

    private static void DrawRoundedPanel(Graphics graphics, Rectangle rect, int radius, Color backColor, Color borderColor)
    {
        graphics.SmoothingMode = SmoothingMode.AntiAlias;

        if (rect.Width <= 1 || rect.Height <= 1)
        {
            return;
        }

        rect.Width -= 1;
        rect.Height -= 1;

        using var path = CreateRoundedPath(rect, radius);
        using var brush = new SolidBrush(backColor);
        using var pen = new Pen(borderColor, 1);

        graphics.FillPath(brush, path);
        graphics.DrawPath(pen, path);
    }

    private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();

        var safeRadius = Math.Max(1, Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2));
        var diameter = safeRadius * 2;

        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();

        return path;
    }

    private static Color LerpColor(Color from, Color to, float amount)
    {
        amount = Math.Clamp(amount, 0F, 1F);

        var r = (int)(from.R + ((to.R - from.R) * amount));
        var g = (int)(from.G + ((to.G - from.G) * amount));
        var b = (int)(from.B + ((to.B - from.B) * amount));

        return Color.FromArgb(r, g, b);
    }

    private static int ColorDistance(Color a, Color b)
    {
        return Math.Abs(a.R - b.R) + Math.Abs(a.G - b.G) + Math.Abs(a.B - b.B);
    }
}