// Archivo: src\ZERNYX.BackupGuardian.App\Theme\RoundedPanel.cs

using System.Drawing.Drawing2D;

namespace ZERNYX.BackupGuardian.App.Theme;

public sealed class RoundedPanel : Panel
{
    public int Radius { get; set; } = AppTheme.Radius;

    public Color BorderColor { get; set; } = AppTheme.Border;

    public int BorderSize { get; set; } = 1;

    public bool UseBorder { get; set; } = true;

    public RoundedPanel()
    {
        DoubleBuffered = true;
        BackColor = AppTheme.Surface;
        Padding = new Padding(16);
        Margin = new Padding(8);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;

        if (rect.Width <= 1 || rect.Height <= 1)
        {
            return;
        }

        rect.Width -= 1;
        rect.Height -= 1;

        using var path = CreateRoundedRectanglePath(rect, Radius);

        using var brush = new SolidBrush(BackColor);
        e.Graphics.FillPath(brush, path);

        if (UseBorder && BorderSize > 0)
        {
            using var pen = new Pen(BorderColor, BorderSize);
            e.Graphics.DrawPath(pen, path);
        }
    }

    protected override void OnResize(EventArgs eventargs)
    {
        base.OnResize(eventargs);
        Invalidate();
    }

    private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
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