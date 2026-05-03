// Archivo: src\ZERNYX.BackupGuardian.App\Forms\HistoryForm.cs

using System.Drawing.Drawing2D;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Repositories;
using ZERNYX.BackupGuardian.App.Services;
using ZERNYX.BackupGuardian.App.Theme;
using ZERNYX.BackupGuardian.App.Utils;

namespace ZERNYX.BackupGuardian.App.Forms;

public sealed class HistoryForm : Form
{
    private readonly BackupHistoryRepository _historyRepository;

    private DataGridView _grid = null!;
    private Label _lblTotal = null!;
    private Label _lblSuccess = null!;
    private Label _lblErrors = null!;
    private Label _lblLast = null!;

    public HistoryForm()
    {
        _historyRepository = new BackupHistoryRepository();
        InitializeComponent();
        Load += (_, _) => ReloadHistory();
    }

    private void InitializeComponent()
    {
        AppTheme.ApplyForm(this);

        Text = "Historial de backups";
        Size = new Size(1500, 920);
        MinimumSize = new Size(1420, 860);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(42, 34, 42, 32),
            ColumnCount = 1,
            RowCount = 4,
            BackColor = AppTheme.Background
        };

        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86));

        Controls.Add(root);

        root.Controls.Add(BuildHeader(), 0, 0);
        root.Controls.Add(BuildStats(), 0, 1);
        root.Controls.Add(BuildGridCard(), 0, 2);
        root.Controls.Add(BuildActions(), 0, 3);
    }

    private Control BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 0, 0, 14)
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 460));

        var titleBox = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Background,
            Padding = new Padding(6, 4, 0, 0)
        };

        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        titleBox.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        titleBox.Controls.Add(new Label
        {
            Text = "Historial de backups",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.PrimaryDark,
            Font = new Font("Segoe UI", 24F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        titleBox.Controls.Add(new Label
        {
            Text = "Consulta operativa de ejecuciones manuales, programadas y resultados técnicos.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        var badgeHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background
        };

        var badge = new ModernPanel
        {
            Width = 210,
            Height = 36,
            Radius = 16,
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            Padding = new Padding(10),
            Anchor = AnchorStyles.Top | AnchorStyles.Left,
            Location = new Point(42, 16)
        };

        badge.Controls.Add(new Label
        {
            Text = "Auditoría local · SQLite",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 8.1F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter,
            BackColor = AppTheme.Surface
        });

        badgeHost.Controls.Add(badge);

        header.Controls.Add(titleBox, 0, 0);
        header.Controls.Add(badgeHost, 1, 0);

        return header;
    }

    private Control BuildStats()
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 0, 0, 34),
            Margin = new Padding(0, 0, 0, 8)
        };

        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        _lblTotal = new Label();
        _lblSuccess = new Label();
        _lblErrors = new Label();
        _lblLast = new Label();

        panel.Controls.Add(BuildStatCard("Registros", _lblTotal, "Total de ejecuciones", AppTheme.SurfaceSoft, new Padding(0, 0, 18, 0)), 0, 0);
        panel.Controls.Add(BuildStatCard("Exitosos", _lblSuccess, "Backups correctos", AppTheme.SurfaceSoft, new Padding(18, 0, 18, 0)), 1, 0);
        panel.Controls.Add(BuildStatCard("Errores", _lblErrors, "Requieren revisión", AppTheme.SurfaceSoft, new Padding(18, 0, 18, 0)), 2, 0);
        panel.Controls.Add(BuildStatCard("Último evento", _lblLast, "Actividad reciente", AppTheme.SurfaceSoft, new Padding(18, 0, 0, 0)), 3, 0);

        return panel;
    }

    private static Control BuildStatCard(string title, Label valueLabel, string subtitle, Color softColor, Padding margin)
    {
        var cardHost = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Background,
            Padding = margin
        };

        var card = new ModernPanel
        {
            Dock = DockStyle.Fill,
            Radius = 18,
            BackColor = softColor,
            BorderColor = AppTheme.Border,
            Padding = new Padding(16, 10, 16, 10),
            Margin = new Padding(0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = softColor
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 20));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = title,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = softColor
        }, 0, 0);

        valueLabel.Text = "-";
        valueLabel.Dock = DockStyle.Fill;
        valueLabel.ForeColor = AppTheme.Text;
        valueLabel.Font = new Font("Segoe UI", 14.2F, FontStyle.Bold);
        valueLabel.TextAlign = ContentAlignment.MiddleLeft;
        valueLabel.BackColor = softColor;

        layout.Controls.Add(valueLabel, 0, 1);

        layout.Controls.Add(new Label
        {
            Text = subtitle,
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 7.6F, FontStyle.Regular),
            TextAlign = ContentAlignment.TopLeft,
            BackColor = softColor
        }, 0, 2);

        card.Controls.Add(layout);
        cardHost.Controls.Add(card);

        return cardHost;
    }

    private Control BuildGridCard()
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

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = AppTheme.Surface
        };

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Surface,
            Padding = new Padding(2, 0, 2, 8)
        };

        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));

        top.Controls.Add(new Label
        {
            Text = "Detalle de ejecuciones",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.Text,
            Font = new Font("Segoe UI", 11.4F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Surface
        }, 0, 0);

        top.Controls.Add(new Label
        {
            Text = "Últimos 1000 registros",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8F, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleRight,
            BackColor = AppTheme.Surface
        }, 1, 0);

        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            BorderStyle = BorderStyle.None,
            BackgroundColor = AppTheme.Surface,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = AppTheme.Border,
            EnableHeadersVisualStyles = false,
            RowHeadersVisible = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            ColumnHeadersHeight = 44,
            RowTemplate = { Height = 42 }
        };

        _grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = AppTheme.SurfaceSoft;
        _grid.ColumnHeadersDefaultCellStyle.ForeColor = AppTheme.PrimaryDark;
        _grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        _grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);

        _grid.DefaultCellStyle.BackColor = AppTheme.Surface;
        _grid.DefaultCellStyle.ForeColor = AppTheme.Text;
        _grid.DefaultCellStyle.SelectionBackColor = AppTheme.PrimarySoft;
        _grid.DefaultCellStyle.SelectionForeColor = AppTheme.Text;
        _grid.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F, FontStyle.Regular);
        _grid.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

        _grid.AlternatingRowsDefaultCellStyle.BackColor = AppTheme.SurfaceSoft;

        AddColumns();

        layout.Controls.Add(top, 0, 0);
        layout.Controls.Add(_grid, 0, 1);

        card.Controls.Add(layout);

        return card;
    }

    private void AddColumns()
    {
        _grid.Columns.Clear();

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "ID",
            DataPropertyName = "Id",
            FillWeight = 35,
            MinimumWidth = 50
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Fecha",
            DataPropertyName = "StartedAtText",
            FillWeight = 92,
            MinimumWidth = 115
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tarea",
            DataPropertyName = "TaskName",
            FillWeight = 135,
            MinimumWidth = 150
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tipo",
            DataPropertyName = "TriggerText",
            FillWeight = 70,
            MinimumWidth = 85
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Estado",
            DataPropertyName = "StatusText",
            FillWeight = 75,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Copiados",
            DataPropertyName = "FilesCopied",
            FillWeight = 65,
            MinimumWidth = 80
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Fallidos",
            DataPropertyName = "FilesFailed",
            FillWeight = 65,
            MinimumWidth = 80
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Tamaño",
            DataPropertyName = "SizeText",
            FillWeight = 75,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Duración",
            DataPropertyName = "DurationText",
            FillWeight = 75,
            MinimumWidth = 90
        });

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = "Mensaje",
            DataPropertyName = "Message",
            FillWeight = 190,
            MinimumWidth = 220
        });
    }

    private Control BuildActions()
    {
        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = AppTheme.Background,
            Padding = new Padding(0, 12, 0, 0)
        };

        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        actions.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 570));

        actions.Controls.Add(new Label
        {
            Text = "El historial no elimina archivos de backup; solo registra la trazabilidad de cada ejecución.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.TextMuted,
            Font = new Font("Segoe UI", 8.2F, FontStyle.Regular),
            TextAlign = ContentAlignment.MiddleLeft,
            BackColor = AppTheme.Background
        }, 0, 0);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 14, 0, 0),
            BackColor = AppTheme.Background
        };

        var btnClose = CreateModernButton(
            "Cerrar",
            AppTheme.Surface,
            AppTheme.Text,
            140,
            44,
            AppTheme.SurfaceSoft,
            AppTheme.Text,
            AppTheme.Border
        );

        btnClose.Click += (_, _) => Close();

        var btnExport = CreateModernButton(
            "Exportar TXT",
            AppTheme.Primary,
            Color.White,
            150,
            44,
            AppTheme.PrimaryDark,
            Color.White,
            AppTheme.Primary
        );

        btnExport.Click += (_, _) => Export();

        var btnClear = CreateModernButton(
            "Limpiar historial",
            AppTheme.Surface,
            AppTheme.Danger,
            170,
            44,
            AppTheme.DangerSoft,
            AppTheme.Danger,
            AppTheme.DangerSoft
        );

        btnClear.Click += (_, _) => ClearHistory();

        buttons.Controls.Add(btnClose);
        buttons.Controls.Add(btnExport);
        buttons.Controls.Add(btnClear);

        actions.Controls.Add(buttons, 1, 0);

        return actions;
    }

    private void ReloadHistory()
    {
        var history = _historyRepository.GetLatest(1000);

        var rows = history.Select(item => new
        {
            item.Id,
            StartedAtText = DateTimeHelper.FormatDateTime(item.StartedAt),
            item.TaskName,
            TriggerText = BackupTriggerType.ToDisplayName(item.TriggerType),
            StatusText = BackupStatus.ToDisplayName(item.Status),
            item.FilesCopied,
            item.FilesFailed,
            SizeText = FileSizeFormatter.Format(item.TotalBytesCopied),
            DurationText = DateTimeHelper.FormatDuration(item.DurationSeconds),
            item.Message
        }).ToList();

        _grid.DataSource = rows;

        _lblTotal.Text = history.Count.ToString();
        _lblSuccess.Text = history.Count(x => x.Status == BackupStatus.Success).ToString();
        _lblErrors.Text = history.Count(x => x.Status == BackupStatus.Error).ToString();

        var last = history.FirstOrDefault();
        _lblLast.Text = last is null
            ? "-"
            : $"{BackupStatus.ToDisplayName(last.Status)} · {last.StartedAt:dd/MM HH:mm}";
    }

    private void Export()
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

    private void ClearHistory()
    {
        var confirm = MessageBox.Show(
            "¿Seguro que querés limpiar todo el historial?\n\nEsta acción no elimina los archivos de backup, solo los registros.",
            AppConfig.AppName,
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning
        );

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        _historyRepository.DeleteAll();
        ReloadHistory();
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
            Font = new Font("Segoe UI", 8.8F, FontStyle.Bold),
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