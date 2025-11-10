using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

/// <summary>
/// Request to get list of available themes.
/// </summary>
public record GetAvailableThemes() : IRequest<List<string>>;
