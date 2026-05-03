// Archivo: src\ZERNYX.BackupGuardian.App\Theme\AppTheme.cs

namespace ZERNYX.BackupGuardian.App.Theme;

public static class AppTheme
{
    public static readonly Color Background = Color.FromArgb(248, 247, 252);
    public static readonly Color Surface = Color.White;
    public static readonly Color SurfaceSoft = Color.FromArgb(252, 250, 255);

    public static readonly Color Text = Color.FromArgb(16, 24, 40);
    public static readonly Color TextMuted = Color.FromArgb(92, 77, 124);

    public static readonly Color Border = Color.FromArgb(226, 220, 238);

    public static readonly Color Primary = Color.FromArgb(124, 58, 237);
    public static readonly Color PrimaryDark = Color.FromArgb(91, 33, 182);
    public static readonly Color PrimarySoft = Color.FromArgb(245, 240, 255);

    public static readonly Color Success = Color.FromArgb(3, 152, 85);
    public static readonly Color SuccessSoft = Color.FromArgb(236, 253, 243);

    public static readonly Color Danger = Color.FromArgb(217, 45, 32);
    public static readonly Color DangerSoft = Color.FromArgb(254, 243, 242);

    public static readonly Color Warning = Color.FromArgb(220, 104, 3);
    public static readonly Color WarningSoft = Color.FromArgb(255, 250, 235);

    public static readonly Color Neutral = Color.FromArgb(71, 84, 103);
    public static readonly Color NeutralSoft = Color.FromArgb(246, 244, 250);

    public const int Radius = 18;

    public static readonly Font TitleFont = new("Segoe UI", 20F, FontStyle.Bold);
    public static readonly Font SubtitleFont = new("Segoe UI", 10F, FontStyle.Regular);
    public static readonly Font SectionFont = new("Segoe UI", 13F, FontStyle.Bold);
    public static readonly Font BodyFont = new("Segoe UI", 9.5F, FontStyle.Regular);
    public static readonly Font SmallFont = new("Segoe UI", 8.5F, FontStyle.Regular);
    public static readonly Font ButtonFont = new("Segoe UI", 9.5F, FontStyle.Bold);
    public static readonly Font BadgeFont = new("Segoe UI", 8.5F, FontStyle.Bold);

    public static void ApplyForm(Form form)
    {
        form.BackColor = Background;
        form.Font = BodyFont;
        form.StartPosition = FormStartPosition.CenterScreen;
    }

    public static void ApplyGrid(DataGridView grid)
    {
        grid.BackgroundColor = Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Border;
        grid.EnableHeadersVisualStyles = false;

        grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        grid.ColumnHeadersDefaultCellStyle.BackColor = PrimarySoft;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = Text;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8);
        grid.ColumnHeadersHeight = 42;

        grid.DefaultCellStyle.BackColor = Surface;
        grid.DefaultCellStyle.ForeColor = Text;
        grid.DefaultCellStyle.SelectionBackColor = PrimarySoft;
        grid.DefaultCellStyle.SelectionForeColor = Text;
        grid.DefaultCellStyle.Font = BodyFont;
        grid.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.RowTemplate.Height = 42;
    }
}