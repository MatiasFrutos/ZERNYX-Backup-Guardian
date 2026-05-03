// Archivo: src\ZERNYX.BackupGuardian.App\Forms\BackupTaskForm.cs

using System.Drawing.Drawing2D;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;
using ZERNYX.BackupGuardian.App.Theme;
using ZERNYX.BackupGuardian.App.Utils;

namespace ZERNYX.BackupGuardian.App.Forms;

public sealed class BackupTaskForm : Form
{
    private readonly BackupTaskRepository _repository;
    private readonly BackupTask? _editingTask;

    private TextBox _txtName = null!;
    private TextBox _txtSource = null!;
    private TextBox _txtDestination = null!;
    private TextBox _txtNotes = null!;

    private ComboBox _cmbFrequency = null!;
    private ComboBox _cmbTime = null!;

    private CheckBox _chkActive = null!;
    private CheckBox _chkSubfolders = null!;
    private CheckBox _chkOverwrite = null!;
    private CheckBox _chkDateFolder = null!;

    private CheckBox _chkMonday = null!;
    private CheckBox _chkTuesday = null!;
    private CheckBox _chkWednesday = null!;
    private CheckBox _chkThursday = null!;
    private CheckBox _chkFriday = null!;
    private CheckBox _chkSaturday = null!;
    private CheckBox _chkSunday = null!;

    private TabControl _tabs = null!;

    public BackupTaskForm(BackupTask? task = null)
    {
        _repository = new BackupTaskRepository();
        _editingTask = task;

        InitializeComponent();

        if (_editingTask is not null)
        {
            LoadTask(_editingTask);
        }
        else
        {
            _chkActive.Checked = true;
            _chkSubfolders.Checked = true;
            _chkOverwrite.Checked = true;
            _chkDateFolder.Checked = true;

            _cmbFrequency.SelectedIndex = 0;
            _cmbTime.SelectedItem = "18:00";

            UpdateDaysEnabled();
        }
    }

    private void InitializeComponent()
    {
        AppTheme.ApplyForm(this);

        Text = _editingTask is null ? "Nueva tarea de backup" : "Editar tarea de backup";
        Size = new Size(1480, 980);
        MinimumSize = new Size(1480, 980);
        MaximizeBox = false;
        MinimizeBox = false;
        FormBorderStyle = FormBorderStyle.FixedDialog;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(34, 26, 34, 26),
            ColumnCount = 1,
            RowCount = 3,
            BackColor = AppTheme.Background
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 116));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 84));

        Controls.Add(root);

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildMainCard(), 0, 1);
        root.Controls.Add(BuildActions(), 0, 2);
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 560));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Background,
            Padding = new Padding(10, 8, 0, 0)
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        titleBox.Controls.Add(new Label
        {
            Text = _editingTask is null ? "Nueva tarea" : "Editar tarea",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 22F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = "Configurá la tarea por secciones para mantener orden operativo y evitar campos cortados.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.8F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        var badgeHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background
        };

        var badge = new ModernPanel
        {
            Width = 260,
            Height = 44,
            Radius = 18,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(12),
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(92, 18)
        };

        badge.Controls.Add(new Label
        {
            Text = "Backup Guardian · Tarea",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 8.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface
        });

        badgeHost.Controls.Add(badge);

        header.Controls.Add(titleBox, 0, 0);
        header.Controls.Add(badgeHost, 1, 0);

        return header;
    }

    private Control BuildMainCard()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 24,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(24),
            Margin = new Padding(0)
        };

        _tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9.2F, FontStyle.Bold),
            Appearance = TabAppearance.Normal,
            SizeMode = TabSizeMode.Fixed,
            ItemSize = new Size(190, 40)
        };

        var tabGeneral = new TabPage
        {
            Text = "General",
            BackColor = AppTheme.Surface
        };

        var tabSchedule = new TabPage
        {
            Text = "Programación",
            BackColor = AppTheme.Surface
        };

        tabGeneral.Controls.Add(BuildGeneralTab());
        tabSchedule.Controls.Add(BuildScheduleTab());

        _tabs.TabPages.Add(tabGeneral);
        _tabs.TabPages.Add(tabSchedule);

        card.Controls.Add(_tabs);

        return card;
    }

    private Control BuildGeneralTab()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(26, 22, 26, 24),
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));

        root.Controls.Add(BuildGeneralLeft(), 0, 0);
        root.Controls.Add(BuildGeneralRight(), 1, 0);

        return root;
    }

    private Control BuildGeneralLeft()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 22, 0)
        };

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(BuildSectionTitle(
            "Datos de la tarea",
            "Definí un nombre claro para identificar rápidamente este respaldo."
        ), 0, 0);

        panel.Controls.Add(BuildTextInput(
            "Nombre de la tarea",
            "Ej: Backup Documentos Oficina",
            out _txtName
        ), 0, 1);

        panel.Controls.Add(BuildSpacer(), 0, 2);

        panel.Controls.Add(BuildSectionTitle(
            "Notas internas",
            "Campo opcional para documentar el objetivo o alcance de la tarea."
        ), 0, 3);

        panel.Controls.Add(BuildNotesArea(), 0, 4);

        return panel;
    }

    private Control BuildGeneralRight()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = AppTheme.Surface,
            Padding = new Padding(22, 0, 0, 0)
        };

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 74));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(BuildSectionTitle(
            "Rutas de respaldo",
            "Seleccioná la carpeta origen y la carpeta destino del backup."
        ), 0, 0);

        panel.Controls.Add(BuildPathInput(
            "Carpeta origen",
            "Carpeta que se va a respaldar",
            out _txtSource
        ), 0, 1);

        panel.Controls.Add(BuildSpacer(), 0, 2);

        panel.Controls.Add(BuildPathInput(
            "Carpeta destino",
            "Carpeta donde se guardará la copia",
            out _txtDestination
        ), 0, 3);

        panel.Controls.Add(BuildInfoBox(
            "Tip operativo",
            "Usá una carpeta destino distinta a la carpeta origen para evitar copias innecesarias o confusas."
        ), 0, 4);

        return panel;
    }

    private Control BuildScheduleTab()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(26, 22, 26, 24),
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        root.Controls.Add(BuildScheduleLeft(), 0, 0);
        root.Controls.Add(BuildScheduleRight(), 1, 0);

        return root;
    }

    private Control BuildScheduleLeft()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 22, 0)
        };

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(BuildSectionTitle(
            "Programación",
            "Configurá la frecuencia y el horario de ejecución."
        ), 0, 0);

        panel.Controls.Add(BuildFrequencyInput(), 0, 1);
        panel.Controls.Add(BuildTimeInput(), 0, 2);

        panel.Controls.Add(BuildSpacer(), 0, 3);

        panel.Controls.Add(BuildSectionTitle(
            "Días de ejecución",
            "Se utilizan cuando la frecuencia es semanal o por días específicos."
        ), 0, 4);

        panel.Controls.Add(BuildDaysPanel(), 0, 5);

        return panel;
    }

    private Control BuildScheduleRight()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            BackColor = AppTheme.Surface,
            Padding = new Padding(22, 0, 0, 0)
        };

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
        panel.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panel.Controls.Add(BuildSectionTitle(
            "Opciones de ejecución",
            "Controlá cómo se copian y organizan los archivos."
        ), 0, 0);

        panel.Controls.Add(BuildOptionsPanel(), 0, 1);
        panel.Controls.Add(BuildSpacer(), 0, 2);

        panel.Controls.Add(BuildSectionTitle(
            "Detalle operativo",
            "La configuración queda disponible para ejecución manual o programada."
        ), 0, 3);

        panel.Controls.Add(BuildInfoBox(
            "Importante",
            "El programador automático funciona mientras la aplicación permanece abierta. Para ejecución permanente, más adelante se puede sumar servicio de Windows."
        ), 0, 4);

        return panel;
    }

    private Control BuildActions()
    {
        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background
        };

        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 390));

        var hint = new Label
        {
            Text = "La tarea se guardará localmente y podrá ejecutarse manualmente o mediante el programador.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.3F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 20, 0, 0),
            BackColor = AppTheme.Background
        };

        var btnSave = CreateModernButton("Guardar", AppTheme.Primary, Color.White, 170, 48);
        btnSave.Click += (_, _) => Save();

        var btnCancel = CreateModernButton("Cancelar", AppTheme.Surface, AppTheme.Text, 170, 48);
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = AppTheme.Border;
        btnCancel.Click += (_, _) =>
        {
            DialogResult = DialogResult.Cancel;
            Close();
        };

        buttons.Controls.Add(btnSave);
        buttons.Controls.Add(btnCancel);

        actions.Controls.Add(hint, 0, 0);
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

        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 26));

        panel.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        panel.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
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
            Font = new Font("Segoe UI", 8.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        return wrapper;
    }

    private static Control BuildTextInput(string label, string placeholder, out TextBox textBox)
    {
        var wrapper = CreateFieldWrapper(label, new Padding(0, 6, 0, 10));

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 16,
            Padding = new Padding(16, 12, 16, 9)
        };

        textBox = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.4F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            PlaceholderText = placeholder,
            Dock = DockStyle.Fill
        };

        field.BindFocus(textBox);
        field.Controls.Add(textBox);

        wrapper.Controls.Add(field, 0, 1);

        return wrapper;
    }

    private Control BuildPathInput(string label, string placeholder, out TextBox textBox)
    {
        var wrapper = CreateFieldWrapper(label, new Padding(0, 0, 0, 0));

        var row = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 2, 0, 0)
        };

        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));

        var fieldHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 18, 0)
        };

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Top,
            Height = 34,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 14,
            Padding = new Padding(14, 7, 14, 6),
            Margin = new Padding(0)
        };

        var textBoxLocal = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.1F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            PlaceholderText = placeholder,
            Dock = DockStyle.Fill
        };

        textBox = textBoxLocal;

        field.BindFocus(textBoxLocal);
        field.Controls.Add(textBoxLocal);
        fieldHost.Controls.Add(field);

        var btnBrowse = CreateModernButton("Buscar", AppTheme.Surface, AppTheme.Text, 110, 34);
        btnBrowse.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        btnBrowse.Margin = new Padding(0);
        btnBrowse.FlatAppearance.BorderSize = 1;
        btnBrowse.FlatAppearance.BorderColor = AppTheme.Border;
        btnBrowse.Click += (_, _) => SelectFolder(textBoxLocal);

        var buttonHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 0)
        };

        buttonHost.Controls.Add(btnBrowse);

        row.Controls.Add(fieldHost, 0, 0);
        row.Controls.Add(buttonHost, 1, 0);

        wrapper.Controls.Add(row, 0, 1);

        return wrapper;
    }

    private Control BuildNotesArea()
    {
        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 16,
            Padding = new Padding(16, 14, 16, 12),
            Margin = new Padding(0, 8, 0, 0)
        };

        _txtNotes = new TextBox
        {
            BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9.2F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            PlaceholderText = "Notas internas opcionales",
            Multiline = true,
            Dock = DockStyle.Fill,
            ScrollBars = ScrollBars.Vertical
        };

        field.BindFocus(_txtNotes);
        field.Controls.Add(_txtNotes);

        return field;
    }

    private Control BuildFrequencyInput()
    {
        var wrapper = CreateFieldWrapper("Frecuencia", new Padding(0, 6, 0, 10));

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 16,
            Padding = new Padding(14, 11, 14, 9)
        };

        _cmbFrequency = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.4F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            Dock = DockStyle.Fill,
            FlatStyle = FlatStyle.Flat
        };

        _cmbFrequency.Items.Add("Manual solamente");
        _cmbFrequency.Items.Add("Todos los días");
        _cmbFrequency.Items.Add("Días específicos");
        _cmbFrequency.Items.Add("Semanal");
        _cmbFrequency.Items.Add("Mensual");
        _cmbFrequency.SelectedIndexChanged += (_, _) => UpdateDaysEnabled();

        field.BindFocus(_cmbFrequency);
        field.Controls.Add(_cmbFrequency);

        wrapper.Controls.Add(field, 0, 1);

        return wrapper;
    }

    private Control BuildTimeInput()
    {
        var wrapper = CreateFieldWrapper("Hora programada", new Padding(0, 6, 0, 10));

        var field = new AnimatedInputPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.SurfaceSoft,
            Radius = 16,
            Padding = new Padding(14, 11, 14, 9)
        };

        _cmbTime = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 9.4F, FontStyle.Regular),
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.SurfaceSoft,
            Dock = DockStyle.Left,
            Width = 170,
            FlatStyle = FlatStyle.Flat
        };

        LoadTimeOptions();

        field.BindFocus(_cmbTime);
        field.Controls.Add(_cmbTime);

        wrapper.Controls.Add(field, 0, 1);

        return wrapper;
    }

    private void LoadTimeOptions()
    {
        _cmbTime.Items.Clear();

        for (var hour = 0; hour < 24; hour++)
        {
            _cmbTime.Items.Add($"{hour:00}:00");
            _cmbTime.Items.Add($"{hour:00}:15");
            _cmbTime.Items.Add($"{hour:00}:30");
            _cmbTime.Items.Add($"{hour:00}:45");
        }
    }

    private Control BuildDaysPanel()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 18,
            BackColor = AppTheme.SurfaceSoft,
            BorderColor = AppTheme.Border,
            Padding = new Padding(22, 18, 22, 18),
            Margin = new Padding(0, 8, 0, 0)
        };

        var days = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 2,
            BackColor = AppTheme.SurfaceSoft
        };

        days.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        days.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        days.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        days.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        days.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        days.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        _chkMonday = BuildModernCheckBox("Lunes");
        _chkTuesday = BuildModernCheckBox("Martes");
        _chkWednesday = BuildModernCheckBox("Miércoles");
        _chkThursday = BuildModernCheckBox("Jueves");
        _chkFriday = BuildModernCheckBox("Viernes");
        _chkSaturday = BuildModernCheckBox("Sábado");
        _chkSunday = BuildModernCheckBox("Domingo");

        days.Controls.Add(_chkMonday, 0, 0);
        days.Controls.Add(_chkTuesday, 1, 0);
        days.Controls.Add(_chkWednesday, 2, 0);
        days.Controls.Add(_chkThursday, 3, 0);
        days.Controls.Add(_chkFriday, 0, 1);
        days.Controls.Add(_chkSaturday, 1, 1);
        days.Controls.Add(_chkSunday, 2, 1);

        card.Controls.Add(days);

        return card;
    }

    private Control BuildOptionsPanel()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 18,
            BackColor = AppTheme.SurfaceSoft,
            BorderColor = AppTheme.Border,
            Padding = new Padding(22, 18, 22, 18),
            Margin = new Padding(0, 8, 0, 0)
        };

        var options = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 2,
            BackColor = AppTheme.SurfaceSoft
        };

        options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        options.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        options.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        options.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        _chkActive = BuildModernCheckBox("Tarea activa");
        _chkSubfolders = BuildModernCheckBox("Incluir subcarpetas");
        _chkOverwrite = BuildModernCheckBox("Sobrescribir existentes");
        _chkDateFolder = BuildModernCheckBox("Crear carpeta con fecha");

        options.Controls.Add(_chkActive, 0, 0);
        options.Controls.Add(_chkSubfolders, 1, 0);
        options.Controls.Add(_chkOverwrite, 0, 1);
        options.Controls.Add(_chkDateFolder, 1, 1);

        card.Controls.Add(options);

        return card;
    }

    private static Control BuildInfoBox(string title, string text)
    {
        var box = new ModernPanel
        {
            Dock = DockStyle.Top,
            Height = 96,
            Radius = 18,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(18),
            Margin = new Padding(0, 24, 0, 0)
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

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 8.6F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        box.Controls.Add(layout);

        return box;
    }

    private static CheckBox BuildModernCheckBox(string text)
    {
        return new CheckBox
        {
            Text = text,
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8.6F, FontStyle.Bold),
            ForeColor = AppTheme.Text,
            Margin = new Padding(4),
            TextAlign = ContentAlignment.MiddleLeft,
            FlatStyle = FlatStyle.Standard
        };
    }

    private static Button CreateModernButton(string text, Color backColor, Color foreColor, int width, int height)
    {
        var button = new Button
        {
            Text = text,
            Width = width,
            Height = height,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(8, 0, 0, 0),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(backColor, 0.04F);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08F);

        return button;
    }

    private void SelectFolder(TextBox target)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Seleccionar carpeta",
            UseDescriptionForTitle = true,
            ShowNewFolderButton = true
        };

        if (!string.IsNullOrWhiteSpace(target.Text) && Directory.Exists(target.Text))
        {
            dialog.SelectedPath = target.Text;
        }

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            target.Text = dialog.SelectedPath;
        }
    }

    private void LoadTask(BackupTask task)
    {
        _txtName.Text = task.Name;
        _txtSource.Text = task.SourcePath;
        _txtDestination.Text = task.DestinationPath;
        _cmbFrequency.SelectedIndex = FrequencyToIndex(task.Frequency);

        if (_cmbTime.Items.Contains(task.ScheduledTime))
        {
            _cmbTime.SelectedItem = task.ScheduledTime;
        }
        else
        {
            _cmbTime.SelectedItem = "18:00";
        }

        _chkActive.Checked = task.IsActive;
        _chkSubfolders.Checked = task.IncludeSubfolders;
        _chkOverwrite.Checked = task.OverwriteExistingFiles;
        _chkDateFolder.Checked = task.CreateDateFolder;
        _txtNotes.Text = task.Notes;

        var schedule = BackupSchedule.FromDatabaseValue(task.ScheduledDays, task.ScheduledTime);

        _chkMonday.Checked = schedule.Monday;
        _chkTuesday.Checked = schedule.Tuesday;
        _chkWednesday.Checked = schedule.Wednesday;
        _chkThursday.Checked = schedule.Thursday;
        _chkFriday.Checked = schedule.Friday;
        _chkSaturday.Checked = schedule.Saturday;
        _chkSunday.Checked = schedule.Sunday;

        UpdateDaysEnabled();
    }

    private void Save()
    {
        if (!ValidateForm())
        {
            return;
        }

        var task = _editingTask ?? new BackupTask();

        task.Name = _txtName.Text.Trim();
        task.SourcePath = _txtSource.Text.Trim();
        task.DestinationPath = _txtDestination.Text.Trim();
        task.Frequency = IndexToFrequency(_cmbFrequency.SelectedIndex);
        task.ScheduledTime = _cmbTime.Text.Trim();
        task.ScheduledDays = BuildSchedule().ToDatabaseValue();
        task.IsActive = _chkActive.Checked;
        task.IncludeSubfolders = _chkSubfolders.Checked;
        task.OverwriteExistingFiles = _chkOverwrite.Checked;
        task.CreateDateFolder = _chkDateFolder.Checked;
        task.Notes = _txtNotes.Text.Trim();

        if (task.Id <= 0)
        {
            _repository.Create(task);
        }
        else
        {
            _repository.Update(task);
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private bool ValidateForm()
    {
        if (string.IsNullOrWhiteSpace(_txtName.Text))
        {
            _tabs.SelectedIndex = 0;
            ShowValidation("Ingresá un nombre para la tarea.");
            return false;
        }

        if (!PathHelper.IsValidDirectory(_txtSource.Text))
        {
            _tabs.SelectedIndex = 0;
            ShowValidation("La carpeta origen no existe.");
            return false;
        }

        if (string.IsNullOrWhiteSpace(_txtDestination.Text))
        {
            _tabs.SelectedIndex = 0;
            ShowValidation("Seleccioná una carpeta destino.");
            return false;
        }

        if (!Directory.Exists(_txtDestination.Text))
        {
            try
            {
                Directory.CreateDirectory(_txtDestination.Text);
            }
            catch
            {
                _tabs.SelectedIndex = 0;
                ShowValidation("La carpeta destino no existe y no se pudo crear.");
                return false;
            }
        }

        if (string.IsNullOrWhiteSpace(_cmbTime.Text) || !DateTimeHelper.IsValidTime(_cmbTime.Text))
        {
            _tabs.SelectedIndex = 1;
            ShowValidation("Seleccioná una hora programada válida.");
            return false;
        }

        var frequency = IndexToFrequency(_cmbFrequency.SelectedIndex);

        if ((frequency == BackupFrequency.SpecificDays || frequency == BackupFrequency.Weekly) &&
            !BuildSchedule().ToDatabaseValue().Any())
        {
            _tabs.SelectedIndex = 1;
            ShowValidation("Seleccioná al menos un día de ejecución.");
            return false;
        }

        return true;
    }

    private static void ShowValidation(string message)
    {
        MessageBox.Show(
            message,
            AppConfig.AppName,
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        );
    }

    private BackupSchedule BuildSchedule()
    {
        return new BackupSchedule
        {
            Monday = _chkMonday.Checked,
            Tuesday = _chkTuesday.Checked,
            Wednesday = _chkWednesday.Checked,
            Thursday = _chkThursday.Checked,
            Friday = _chkFriday.Checked,
            Saturday = _chkSaturday.Checked,
            Sunday = _chkSunday.Checked,
            Time = _cmbTime.Text.Trim()
        };
    }

    private void UpdateDaysEnabled()
    {
        var frequency = IndexToFrequency(_cmbFrequency.SelectedIndex);
        var enabled = frequency == BackupFrequency.SpecificDays || frequency == BackupFrequency.Weekly;

        _chkMonday.Enabled = enabled;
        _chkTuesday.Enabled = enabled;
        _chkWednesday.Enabled = enabled;
        _chkThursday.Enabled = enabled;
        _chkFriday.Enabled = enabled;
        _chkSaturday.Enabled = enabled;
        _chkSunday.Enabled = enabled;
        _cmbTime.Enabled = frequency != BackupFrequency.Manual;
    }

    private static int FrequencyToIndex(string frequency)
    {
        return frequency switch
        {
            BackupFrequency.Manual => 0,
            BackupFrequency.Daily => 1,
            BackupFrequency.SpecificDays => 2,
            BackupFrequency.Weekly => 3,
            BackupFrequency.Monthly => 4,
            _ => 0
        };
    }

    private static string IndexToFrequency(int index)
    {
        return index switch
        {
            0 => BackupFrequency.Manual,
            1 => BackupFrequency.Daily,
            2 => BackupFrequency.SpecificDays,
            3 => BackupFrequency.Weekly,
            4 => BackupFrequency.Monthly,
            _ => BackupFrequency.Manual
        };
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

        public int Radius { get; set; } = 16;

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