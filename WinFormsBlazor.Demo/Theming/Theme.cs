using System.Drawing;

namespace WinFormsBlazor.Theming;

/// <summary>
/// Represents a visual theme for the application.
/// </summary>
public record Theme(
    string Name,
    Color BackgroundColor,
    Color ForegroundColor,
    Color AccentColor,
    Win11Effects.BackdropType BackdropType,
    bool IsDarkMode)
{
    /// <summary>
    /// Gets CSS variable declarations for this theme.
    /// </summary>
    public string ToCssVariables()
    {
        return $@"
            --bg-color: {ToRgb(BackgroundColor)};
            --fg-color: {ToRgb(ForegroundColor)};
            --accent-color: {ToRgb(AccentColor)};
            --border-color: {ToRgb(GetBorderColor())};
            --hover-color: {ToRgb(GetHoverColor())};
        ";
    }

    private Color GetBorderColor()
    {
        return IsDarkMode
            ? Color.FromArgb(60, 60, 60)
            : Color.FromArgb(220, 220, 220);
    }

    private Color GetHoverColor()
    {
        return IsDarkMode
            ? Color.FromArgb(50, 50, 50)
            : Color.FromArgb(240, 240, 240);
    }

    private static string ToRgb(Color color)
    {
        return $"rgb({color.R}, {color.G}, {color.B})";
    }
}
