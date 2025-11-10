using BlazorWinForms.Interop;

namespace WinFormsBlazor.Events;

/// <summary>
/// Event fired when the application theme changes.
/// </summary>
public record ThemeChanged(string ThemeName) : IEvent;
