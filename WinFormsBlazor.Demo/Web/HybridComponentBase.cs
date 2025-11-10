using Microsoft.JSInterop;

namespace WinFormsBlazor.Web;

/// <summary>
/// Demo-specific base class for Blazor components.
/// Extends the library's HybridComponentBase with theme support.
/// </summary>
public abstract class HybridComponentBase : BlazorWinForms.Components.HybridComponentBase
{
    /// <summary>
    /// Apply theme colors to the Blazor UI (demo feature).
    /// </summary>
    protected async Task ApplyTheme(string themeName)
    {
        await JSRuntime.InvokeVoidAsync("themeManager.applyTheme", themeName);
    }
}
