using BlazorWinForms.Interop;
using WinFormsBlazor.Theming;

namespace WinFormsBlazor.Requests.Handlers;

public class ChangeBackdropTypeHandler : IRequestHandler<ChangeBackdropType, bool>
{
    public Task<bool> HandleAsync(ChangeBackdropType request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<Win11Effects.BackdropType>(request.BackdropTypeName, out var backdropType))
            return Task.FromResult(false);

        ThemeManager.SetBackdropType(backdropType);
        return Task.FromResult(true);
    }
}
