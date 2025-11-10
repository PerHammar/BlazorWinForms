using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

public record GetAvailableBackdropTypes : IRequest<List<string>>;
