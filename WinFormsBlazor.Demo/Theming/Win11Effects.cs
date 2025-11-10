using System.Runtime.InteropServices;

namespace WinFormsBlazor.Theming;

/// <summary>
/// Provides Windows 11 visual effects via DWM (Desktop Window Manager) APIs.
/// </summary>
public static class Win11Effects
{
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

    public enum BackdropType
    {
        Auto = 1,
        Mica = 2,
        Acrylic = 3,
        Tabbed = 4
    }

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int attribute,
        ref int attributeValue,
        int attributeSize);

    public static void EnableDarkMode(IntPtr windowHandle, bool enable)
    {
        int value = enable ? 1 : 0;
        _ = DwmSetWindowAttribute(
            windowHandle,
            DWMWA_USE_IMMERSIVE_DARK_MODE,
            ref value,
            sizeof(int));
    }

    public static void SetBackdropType(IntPtr windowHandle, BackdropType backdropType)
    {
        int value = (int)backdropType;
        _ = DwmSetWindowAttribute(
            windowHandle,
            DWMWA_SYSTEMBACKDROP_TYPE,
            ref value,
            sizeof(int));
    }
}
