using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

/// <summary>
/// Request to update text in the WinForms host.
/// </summary>
public record UpdateFormText(string Text) : IRequest<bool>;
