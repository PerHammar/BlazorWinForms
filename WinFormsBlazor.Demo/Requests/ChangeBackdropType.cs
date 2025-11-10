using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

public record ChangeBackdropType(string BackdropTypeName) : IRequest<bool>;
