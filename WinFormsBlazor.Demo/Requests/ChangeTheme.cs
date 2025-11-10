using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

/// <summary>
/// Request to change the application theme.
/// </summary>
public record ChangeTheme(string ThemeName) : IRequest<bool>;
