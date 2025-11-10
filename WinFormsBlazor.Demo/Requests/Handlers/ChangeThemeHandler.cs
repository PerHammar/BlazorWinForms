using BlazorWinForms.Interop;
using WinFormsBlazor.Theming;

namespace WinFormsBlazor.Requests.Handlers;

public class ChangeThemeHandler : IRequestHandler<ChangeTheme, bool>
{
    public Task<bool> HandleAsync(ChangeTheme request, CancellationToken cancellationToken = default)
    {
        ThemeManager.Apply(request.ThemeName);
        return Task.FromResult(true);
    }
}
