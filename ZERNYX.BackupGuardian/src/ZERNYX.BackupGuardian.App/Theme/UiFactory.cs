// Archivo: src\ZERNYX.BackupGuardian.App\Theme\UiFactory.cs

namespace ZERNYX.BackupGuardian.App.Theme;

public static class UiFactory
{
    public static Label Title(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = AppTheme.Text,
            Font = AppTheme.TitleFont,
            Margin = new Padding(0, 0, 0, 4)
        };
    }

    public static Label Subtitle(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.SubtitleFont,
            Margin = new Padding(0, 0, 0, 12)
        };
    }

    public static Label SectionTitle(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = AppTheme.Text,
            Font = AppTheme.SectionFont,
            Margin = new Padding(0, 0, 0, 8)
        };
    }

    public static Label Body(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = AppTheme.Text,
            Font = AppTheme.BodyFont,
            Margin = new Padding(0, 0, 0, 6)
        };
    }

    public static Label Muted(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            ForeColor = AppTheme.TextMuted,
            Font = AppTheme.SmallFont,
            Margin = new Padding(0, 0, 0, 6)
        };
    }

    public static TextBox TextBox(string placeholder = "")
    {
        return new TextBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.Surface,
            PlaceholderText = placeholder,
            Height = 34,
            Margin = new Padding(0, 4, 0, 10)
        };
    }

    public static ComboBox ComboBox()
    {
        return new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
            BackColor = AppTheme.Surface,
            Height = 34,
            Margin = new Padding(0, 4, 0, 10)
        };
    }

    public static CheckBox CheckBox(string text)
    {
        return new CheckBox
        {
            Text = text,
            AutoSize = true,
            Font = AppTheme.BodyFont,
            ForeColor = AppTheme.Text,
            Margin = new Padding(0, 4, 0, 6)
        };
    }

    public static Button PrimaryButton(string text)
    {
        return Button(text, AppTheme.Primary, Color.White);
    }

    public static Button SecondaryButton(string text)
    {
        return Button(text, AppTheme.NeutralSoft, AppTheme.Text);
    }

    public static Button DangerButton(string text)
    {
        return Button(text, AppTheme.Danger, Color.White);
    }

    public static Button SuccessButton(string text)
    {
        return Button(text, AppTheme.Success, Color.White);
    }

    public static Button Button(string text, Color backColor, Color foreColor)
    {
        var button = new Button
        {
            Text = text,
            BackColor = backColor,
            ForeColor = foreColor,
            Font = AppTheme.ButtonFont,
            FlatStyle = FlatStyle.Flat,
            Height = 38,
            Width = 132,
            Cursor = Cursors.Hand,
            Margin = new Padding(4),
            TextAlign = ContentAlignment.MiddleCenter,
            UseVisualStyleBackColor = false
        };

        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = ControlPaint.Dark(backColor, 0.04F);
        button.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08F);

        return button;
    }

    public static RoundedPanel Card(int height = 0)
    {
        return new RoundedPanel
        {
            BackColor = AppTheme.Surface,
            BorderColor = AppTheme.Border,
            UseBorder = true,
            Height = height,
            Padding = new Padding(16),
            Margin = new Padding(6)
        };
    }

    public static DataGridView Grid()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true
        };

        AppTheme.ApplyGrid(grid);

        return grid;
    }
}