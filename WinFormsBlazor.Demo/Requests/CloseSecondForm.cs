using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

/// <summary>
/// Request to close the second form window.
/// </summary>
public record CloseSecondForm : IRequest<bool>;
