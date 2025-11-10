using System.Drawing;
using Microsoft.Win32;

namespace WinFormsBlazor.Theming;

/// <summary>
/// Manages application themes and theme switching.
/// </summary>
public static class ThemeManager
{
    public static event EventHandler? Changed;

    private static readonly Dictionary<string, Theme> _themes = new();
    private static bool _initialized = false;
    private static Theme? _current;

    static ThemeManager()
    {
        // Light themes
        _themes["Light"] = new Theme(
            Name: "Light",
            BackgroundColor: Color.White,
            ForegroundColor: Color.Black,
            AccentColor: Color.FromArgb(0, 120, 215),
            BackdropType: Win11Effects.BackdropType.Mica,
            IsDarkMode: false
        );

        _themes["Light Blue"] = new Theme(
            Name: "Light Blue",
            BackgroundColor: Color.FromArgb(240, 248, 255),
            ForegroundColor: Color.FromArgb(20, 20, 20),
            AccentColor: Color.FromArgb(30, 144, 255),
            BackdropType: Win11Effects.BackdropType.Acrylic,
            IsDarkMode: false
        );

        // Dark themes
        _themes["Dark"] = new Theme(
            Name: "Dark",
            BackgroundColor: Color.FromArgb(30, 30, 30),
            ForegroundColor: Color.White,
            AccentColor: Color.FromArgb(45, 120, 210),
            BackdropType: Win11Effects.BackdropType.Mica,
            IsDarkMode: true
        );

        _themes["Dark Purple"] = new Theme(
            Name: "Dark Purple",
            BackgroundColor: Color.FromArgb(25, 20, 35),
            ForegroundColor: Color.FromArgb(240, 240, 245),
            AccentColor: Color.FromArgb(138, 43, 226),
            BackdropType: Win11Effects.BackdropType.Acrylic,
            IsDarkMode: true
        );

        // Don't apply theme in static constructor - wait for first access
    }

    public static Theme Current
    {
        get
        {
            if (!_initialized)
            {
                _initialized = true;
                // Initialize with system theme on first access (lazy initialization)
                var themeName = IsWindowsDarkMode() ? "Dark" : "Light";
                _current = _themes[themeName];
            }
            return _current!;
        }
        private set => _current = value;
    }

    public static IReadOnlyList<string> AvailableThemes => _themes.Keys.ToList();

    public static IReadOnlyList<string> AvailableBackdropTypes => Enum.GetNames(typeof(Win11Effects.BackdropType)).ToList();

    public static void Apply(string themeName)
    {
        if (!_themes.TryGetValue(themeName, out var theme))
            return;

        Current = theme;
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void SetBackdropType(Win11Effects.BackdropType backdropType)
    {
        if (_current == null)
            return;

        _current = _current with { BackdropType = backdropType };
        Changed?.Invoke(null, EventArgs.Empty);
    }

    private static bool IsWindowsDarkMode()
    {
        try
        {
            var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            return key?.GetValue("AppsUseLightTheme") is int value && value == 0;
        }
        catch
        {
            return false;
        }
    }
}
