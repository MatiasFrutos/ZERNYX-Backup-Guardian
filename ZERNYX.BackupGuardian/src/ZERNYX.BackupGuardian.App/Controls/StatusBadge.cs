// Archivo: src\ZERNYX.BackupGuardian.App\Controls\StatusBadge.cs

using System.Drawing.Drawing2D;
using ZERNYX.BackupGuardian.App.Models;
using ZERNYX.BackupGuardian.App.Theme;

namespace ZERNYX.BackupGuardian.App.Controls;

public sealed class StatusBadge : Label
{
    private Color _badgeBackColor = AppTheme.NeutralSoft;
    private Color _badgeForeColor = AppTheme.Neutral;

    public int Radius { get; set; } = 12;

    public StatusBadge()
    {
        AutoSize = false;
        Height = 26;
        Width = 110;
        TextAlign = ContentAlignment.MiddleCenter;
        Font = AppTheme.BadgeFont;
        Margin = new Padding(4);
        SetNeutral("Pendiente");
    }

    public void SetStatus(string status)
    {
        switch (status)
        {
            case BackupStatus.Success:
            case "Exitoso":
            case "Activo":
                SetSuccess(status == BackupStatus.Success ? "Exitoso" : status);
                break;

            case BackupStatus.Error:
            case "Error":
            case "Inactivo":
                SetDanger(status == BackupStatus.Error ? "Error" : status);
                break;

            case BackupStatus.Running:
            case "En proceso":
                SetWarning("En proceso");
                break;

            default:
                SetNeutral(string.IsNullOrWhiteSpace(status) ? "Pendiente" : status);
                break;
        }
    }

    public void SetSuccess(string text)
    {
        Text = text;
        _badgeBackColor = AppTheme.SuccessSoft;
        _badgeForeColor = AppTheme.Success;
        ForeColor = _badgeForeColor;
        Invalidate();
    }

    public void SetDanger(string text)
    {
        Text = text;
        _badgeBackColor = AppTheme.DangerSoft;
        _badgeForeColor = AppTheme.Danger;
        ForeColor = _badgeForeColor;
        Invalidate();
    }

    public void SetWarning(string text)
    {
        Text = text;
        _badgeBackColor = AppTheme.WarningSoft;
        _badgeForeColor = AppTheme.Warning;
        ForeColor = _badgeForeColor;
        Invalidate();
    }

    public void SetNeutral(string text)
    {
        Text = text;
        _badgeBackColor = AppTheme.NeutralSoft;
        _badgeForeColor = AppTheme.Neutral;
        ForeColor = _badgeForeColor;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        var rect = ClientRectangle;

        if (rect.Width <= 1 || rect.Height <= 1)
        {
            return;
        }

        rect.Width -= 1;
        rect.Height -= 1;

        using var path = CreateRoundedPath(rect, Radius);
        using var brush = new SolidBrush(_badgeBackColor);

        e.Graphics.FillPath(brush, path);

        TextRenderer.DrawText(
            e.Graphics,
            Text,
            Font,
            ClientRectangle,
            _badgeForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
        );
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