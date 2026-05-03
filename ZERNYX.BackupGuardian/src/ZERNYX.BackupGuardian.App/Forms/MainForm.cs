// Archivo: src\ZERNYX.BackupGuardian.App\Forms\MainForm.cs

using System.Drawing.Drawing2D;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;
using ZERNYX.BackupGuardian.App.Services;
using ZERNYX.BackupGuardian.App.Theme;
using ZERNYX.BackupGuardian.App.Utils;

namespace ZERNYX.BackupGuardian.App.Forms;

public sealed class MainForm : Form
{
    private readonly BackupTaskRepository _taskRepository;
    private readonly BackupHistoryRepository _historyRepository;
    private readonly BackupService _backupService;
    private readonly SchedulerService _schedulerService;

    private DataGridView _gridTasks = null!;
    private DataGridView _gridHistory = null!;

    private Label _lblStatus = null!;
    private Label _lblSummary = null!;

    private Button _btnRun = null!;
    private Button _btnEdit = null!;
    private Button _btnDelete = null!;

    private TabControl _tabs = null!;

    private List<BackupTask> _tasks = [];

    public MainForm()
    {
        _taskRepository = new BackupTaskRepository();
        _historyRepository = new BackupHistoryRepository();
        _backupService = new BackupService();
        _schedulerService = new SchedulerService();

        InitializeComponent();

        _schedulerService.StatusChanged += (_, message) =>
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(() => _lblStatus.Text = message);
                return;
            }

            _lblStatus.Text = message;
        };

        _schedulerService.BackupExecuted += (_, _) =>
        {
            if (IsDisposed)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(ReloadAll);
                return;
            }

            ReloadAll();
        };

        Load += (_, _) =>
        {
            ReloadAll();
            _schedulerService.Start();
        };

        FormClosing += (_, _) =>
        {
            _schedulerService.Dispose();
        };
    }

    private void InitializeComponent()
    {
        AppTheme.ApplyForm(this);

        Text = $"{AppConfig.AppName} v{AppConfig.AppVersion}";
        MinimumSize = new Size(1360, 800);
        Size = new Size(1440, 860);
        BackColor = AppTheme.Background;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24, 22, 34, 18),
            ColumnCount = 1,
            RowCount = 4,
            BackColor = AppTheme.Background
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 102));   // Header
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));    // Texto operativo
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));    // Tabs + tablas
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));    // Footer

        Controls.Add(root);

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildSummaryText(), 0, 1);
        root.Controls.Add(BuildTabs(), 0, 2);
        root.Controls.Add(BuildFooter(), 0, 3);
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 4, 0, 16)
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 610));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Background,
            Padding = new Padding(6, 4, 0, 0)
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        titleBox.Controls.Add(new Label
        {
            Text = AppConfig.AppName,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = AppConfig.Slogan,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.3F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 1);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 28, 8, 0)
        };

        var btnAdd = CreateModernButton(
            "+ Nueva tarea",
            AppTheme.Primary,
            Color.White,
            138,
            36,
            AppTheme.PrimaryDark,
            Color.White,
            AppTheme.Primary
        );

        btnAdd.Click += (_, _) =>
        {
            using var form = new BackupTaskForm();
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                ReloadAll();
            }
        };

        var btnHistory = CreateModernButton(
            "Historial",
            AppTheme.Surface,
            AppTheme.Text,
            104,
            36,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnHistory.Click += (_, _) =>
        {
            using var form = new HistoryForm();
            form.ShowDialog(this);
            ReloadAll();
        };

        var btnSettings = CreateModernButton(
            "Config.",
            AppTheme.Surface,
            AppTheme.Text,
            94,
            36,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnSettings.Click += (_, _) =>
        {
            using var form = new SettingsForm();
            form.ShowDialog(this);
        };

        var btnAbout = CreateModernButton(
            "Acerca de",
            AppTheme.Surface,
            AppTheme.Text,
            104,
            36,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnAbout.Click += (_, _) => new AboutForm().ShowDialog(this);

        buttons.Controls.Add(btnAdd);
        buttons.Controls.Add(btnHistory);
        buttons.Controls.Add(btnSettings);
        buttons.Controls.Add(btnAbout);

        header.Controls.Add(titleBox, 0, 0);
        header.Controls.Add(buttons, 1, 0);

        return header;
    }

    private Control BuildSummaryText()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(6, 2, 6, 14)
        };

        _lblSummary = new Label
        {
            Text = "Resumen operativo: sin tareas cargadas todavía.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.6F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        };

        panel.Controls.Add(_lblSummary, 0, 0);

        return panel;
    }

    private Control BuildTabs()
    {
        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 22,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(18),
            Margin = new Padding(0)
        };

        _tabs = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 8.7F, FontStyle.Bold),
            Appearance = TabAppearance.Normal,
            SizeMode = TabSizeMode.Fixed,
            ItemSize = new Size(170, 34)
        };

        var tabTasks = new TabPage
        {
            Text = "Tareas de backup",
            BackColor = AppTheme.Surface
        };

        var tabHistory = new TabPage
        {
            Text = "Últimos backups",
            BackColor = AppTheme.Surface
        };

        tabTasks.Controls.Add(BuildTasksSection());
        tabHistory.Controls.Add(BuildHistorySection());

        _tabs.TabPages.Add(tabTasks);
        _tabs.TabPages.Add(tabHistory);

        card.Controls.Add(_tabs);

        return card;
    }

    private Control BuildTasksSection()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface,
            Padding = new Padding(18, 14, 18, 18)
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 8)
        };

        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        titleBox.Controls.Add(new Label
        {
            Text = "Tareas de backup",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 10.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = "Listado de respaldos configurados.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.6F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 3, 0, 0)
        };

        _btnDelete = CreateModernButton(
            "Eliminar",
            AppTheme.Surface,
            AppTheme.Danger,
            104,
            34,
            AppTheme.DangerSoft,
            AppTheme.Danger,
            AppTheme.DangerSoft
        );

        _btnDelete.Click += (_, _) => DeleteSelectedTask();

        _btnEdit = CreateModernButton(
            "Editar",
            AppTheme.Surface,
            AppTheme.Text,
            88,
            34,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        _btnEdit.Click += (_, _) => EditSelectedTask();

        _btnRun = CreateModernButton(
            "Ejecutar",
            AppTheme.Success,
            Color.White,
            98,
            34,
            ControlPaint.Dark(AppTheme.Success, 0.08F),
            Color.White,
            AppTheme.Success
        );

        _btnRun.Click += (_, _) => RunSelectedTask();

        actions.Controls.Add(_btnDelete);
        actions.Controls.Add(_btnEdit);
        actions.Controls.Add(_btnRun);

        top.Controls.Add(titleBox, 0, 0);
        top.Controls.Add(actions, 1, 0);

        _gridTasks = CreateModernGrid();
        _gridTasks.AutoGenerateColumns = false;
        _gridTasks.SelectionChanged += (_, _) => UpdateActionButtons();
        _gridTasks.CellDoubleClick += (_, _) => EditSelectedTask();

        AddTaskColumns();

        root.Controls.Add(top, 0, 0);
        root.Controls.Add(_gridTasks, 0, 1);

        return root;
    }

    private Control BuildHistorySection()
    {
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface,
            Padding = new Padding(18, 14, 18, 18)
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 0, 0, 8)
        };

        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        titleBox.Controls.Add(new Label
        {
            Text = "Últimos backups",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 10.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = "Ejecuciones recientes y resultados.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.6F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 1);

        var btnExport = CreateModernButton(
            "Exportar TXT",
            AppTheme.Surface,
            AppTheme.Text,
            150,
            34,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnExport.Click += (_, _) => ExportHistory();

        var exportHost = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            BackColor = AppTheme.Surface,
            Padding = new Padding(0, 3, 8, 0)
        };

        exportHost.Controls.Add(btnExport);

        top.Controls.Add(titleBox, 0, 0);
        top.Controls.Add(exportHost, 1, 0);

        _gridHistory = CreateModernGrid();
        _gridHistory.AutoGenerateColumns = false;

        AddHistoryColumns();

        root.Controls.Add(top, 0, 0);
        root.Controls.Add(_gridHistory, 0, 1);

        return root;
    }

    private void AddTaskColumns()
    {
        _gridTasks.Columns.Clear();

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "ID",
            DataPropertyName = "Id",
            FillWeight = 35,
            MinimumWidth = 48
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tarea",
            DataPropertyName = "Name",
            FillWeight = 125,
            MinimumWidth = 130
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Origen",
            DataPropertyName = "SourcePath",
            FillWeight = 160,
            MinimumWidth = 160
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Destino",
            DataPropertyName = "DestinationPath",
            FillWeight = 160,
            MinimumWidth = 160
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Frecuencia",
            DataPropertyName = "FrequencyDisplay",
            FillWeight = 90,
            MinimumWidth = 100
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Hora",
            DataPropertyName = "ScheduledTime",
            FillWeight = 55,
            MinimumWidth = 64
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Estado",
            DataPropertyName = "StatusLabel",
            FillWeight = 70,
            MinimumWidth = 84
        });

        _gridTasks.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Último resultado",
            DataPropertyName = "LastStatus",
            FillWeight = 95,
            MinimumWidth = 112
        });
    }

    private void AddHistoryColumns()
    {
        _gridHistory.Columns.Clear();

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Fecha",
            DataPropertyName = "StartedAtText",
            FillWeight = 90,
            MinimumWidth = 115
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tarea",
            DataPropertyName = "TaskName",
            FillWeight = 140,
            MinimumWidth = 140
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tipo",
            DataPropertyName = "TriggerText",
            FillWeight = 70,
            MinimumWidth = 76
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Estado",
            DataPropertyName = "StatusText",
            FillWeight = 70,
            MinimumWidth = 84
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Archivos",
            DataPropertyName = "FilesCopied",
            FillWeight = 60,
            MinimumWidth = 76
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tamaño",
            DataPropertyName = "SizeText",
            FillWeight = 70,
            MinimumWidth = 84
        });

        _gridHistory.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Mensaje",
            DataPropertyName = "Message",
            FillWeight = 220,
            MinimumWidth = 240
        });
    }

    private Control BuildFooter()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(2, 5, 2, 0)
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 390));

        _lblStatus = new Label
        {
            Text = "Listo",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.8F, FontStyle.Regular),
            BackColor = AppTheme.Background
        };

        var brand = new Label
        {
            Text = $"Desarrollado por {AppConfig.CompanyName}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.8F, FontStyle.Regular),
            BackColor = AppTheme.Background
        };

        panel.Controls.Add(_lblStatus, 0, 0);
        panel.Controls.Add(brand, 1, 0);

        return panel;
    }

    private void ReloadAll()
    {
        ReloadTasks();
        ReloadHistory();
        ReloadSummary();
        UpdateActionButtons();
    }

    private void ReloadTasks()
    {
        _tasks = _taskRepository.GetAll();

        var rows = _tasks.Select(task => new
        {
            task.Id,
            task.Name,
            SourcePath = PathHelper.ShortenPath(task.SourcePath, 65),
            DestinationPath = PathHelper.ShortenPath(task.DestinationPath, 65),
            FrequencyDisplay = task.GetFrequencyLabel(),
            task.ScheduledTime,
            task.StatusLabel,
            task.LastStatus
        }).ToList();

        _gridTasks.DataSource = rows;
    }

    private void ReloadHistory()
    {
        var rows = _historyRepository.GetLatest(30).Select(item => new
        {
            StartedAtText = DateTimeHelper.FormatDateTime(item.StartedAt),
            item.TaskName,
            TriggerText = BackupTriggerType.ToDisplayName(item.TriggerType),
            StatusText = BackupStatus.ToDisplayName(item.Status),
            item.FilesCopied,
            SizeText = FileSizeFormatter.Format(item.TotalBytesCopied),
            item.Message
        }).ToList();

        _gridHistory.DataSource = rows;
    }

    private void ReloadSummary()
    {
        var totalTasks = _tasks.Count;
        var successBackups = _historyRepository.CountByStatus(BackupStatus.Success);
        var errorBackups = _historyRepository.CountByStatus(BackupStatus.Error);
        var last = _historyRepository.GetLast();

        var lastText = last is null
            ? "sin ejecuciones registradas"
            : $"{BackupStatus.ToDisplayName(last.Status)} el {last.StartedAt:dd/MM/yyyy HH:mm}";

        _lblSummary.Text =
            $"Resumen operativo: {totalTasks} tarea(s) configurada(s) · {successBackups} backup(s) correcto(s) · {errorBackups} con error · Último evento: {lastText}.";
    }

    private BackupTask? GetSelectedTask()
    {
        if (_gridTasks.CurrentRow is null)
        {
            return null;
        }

        var value = _gridTasks.CurrentRow.Cells[0].Value;

        if (value is null || !int.TryParse(value.ToString(), out var id))
        {
            return null;
        }

        return _taskRepository.GetById(id);
    }

    private void UpdateActionButtons()
    {
        var hasSelection = GetSelectedTask() is not null;

        _btnRun.Enabled = hasSelection;
        _btnEdit.Enabled = hasSelection;
        _btnDelete.Enabled = hasSelection;

        ApplyButtonState(_btnRun, hasSelection);
        ApplyButtonState(_btnEdit, hasSelection);
        ApplyButtonState(_btnDelete, hasSelection);
    }

    private void RunSelectedTask()
    {
        var task = GetSelectedTask();

        if (task is null)
        {
            return;
        }

        Cursor = Cursors.WaitCursor;
        _lblStatus.Text = $"Ejecutando backup: {task.Name}";

        try
        {
            var result = _backupService.ExecuteBackup(task, BackupTriggerType.Manual);

            MessageBox.Show(
                result.Success
                    ? $"Backup finalizado correctamente.\n\nArchivos copiados: {result.FilesCopied}\nDestino:\n{result.FinalBackupPath}"
                    : $"Backup finalizado con errores.\n\nDetalle:\n{result.Message}\n{result.ErrorDetails}",
                AppConfig.AppName,
                MessageBoxButtons.OK,
                result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Warning
            );
        }
        finally
        {
            Cursor = Cursors.Default;
            _lblStatus.Text = "Listo";
            ReloadAll();
        }
    }

    private void EditSelectedTask()
    {
        var task = GetSelectedTask();

        if (task is null)
        {
            return;
        }

        using var form = new BackupTaskForm(task);

        if (form.ShowDialog(this) == DialogResult.OK)
        {
            ReloadAll();
        }
    }

    private void DeleteSelectedTask()
    {
        var task = GetSelectedTask();

        if (task is null)
        {
            return;
        }

        var confirm = MessageBox.Show(
            $"¿Eliminar la tarea de backup?\n\n{task.Name}",
            AppConfig.AppName,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _taskRepository.Delete(task.Id);
        ReloadAll();
    }

    private void ExportHistory()
    {
        try
        {
            var reportService = new ReportService();
            var path = reportService.ExportHistoryToTxt();

            MessageBox.Show(
                $"Reporte exportado correctamente:\n\n{path}",
                AppConfig.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo exportar el reporte.\n\n{ex.Message}",
                AppConfig.AppName,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private static DataGridView CreateModernGrid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            BorderStyle = BorderStyle.None,
            BackgroundColor = AppTheme.Surface,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical,
            GridColor = AppTheme.Border,
            EnableHeadersVisualStyles = false,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 38,
            RowTemplate = { Height = 36 }
        };

        grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
        grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
        grid.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        grid.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;

        grid.AdvancedColumnHeadersBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
        grid.AdvancedColumnHeadersBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
        grid.AdvancedColumnHeadersBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.None;
        grid.AdvancedColumnHeadersBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceSoft;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.PrimaryDark;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(7, 3, 7, 3);

        grid.DefaultCellStyle.BackColor = AppTheme.Surface;
        grid.DefaultCellStyle.ForeColor = AppTheme.Text;
        grid.DefaultCellStyle.SelectionBackColor = AppTheme.PrimarySoft;
        grid.DefaultCellStyle.SelectionForeColor = AppTheme.Text;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 8F, FontStyle.Regular);
        grid.DefaultCellStyle.Padding = new Padding(7, 3, 7, 3);

        grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.SurfaceSoft;

        return grid;
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
            Font = new Font("Segoe UI", 8.1F, FontStyle.Bold),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand,
            Margin = new Padding(7, 0, 0, 0),
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = borderColor;
        button.FlatAppearance.MouseOverBackColor = hoverBackColor;
        button.FlatAppearance.MouseDownBackColor = hoverBackColor;

        button.MouseEnter += (_, _) =>
        {
            if (!button.Enabled)
            {
                return;
            }

            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
            button.FlatAppearance.BorderColor = borderColor;
        };

        button.MouseLeave += (_, _) =>
        {
            if (!button.Enabled)
            {
                return;
            }

            button.BackColor = backColor;
            button.ForeColor = foreColor;
            button.FlatAppearance.BorderColor = borderColor;
        };

        button.MouseDown += (_, _) =>
        {
            if (!button.Enabled)
            {
                return;
            }

            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
        };

        button.MouseUp += (_, _) =>
        {
            if (!button.Enabled)
            {
                return;
            }

            button.BackColor = hoverBackColor;
            button.ForeColor = hoverForeColor;
        };

        return button;
    }

    private static void ApplyButtonState(Button button, bool enabled)
    {
        button.Enabled = enabled;
        button.Cursor = enabled ? Cursors.Hand : Cursors.Default;

        if (!enabled)
        {
            button.ForeColor = AppTheme.TextMuted;
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
}