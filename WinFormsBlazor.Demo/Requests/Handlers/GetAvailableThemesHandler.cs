using BlazorWinForms.Interop;
using WinFormsBlazor.Theming;

namespace WinFormsBlazor.Requests.Handlers;

public class GetAvailableThemesHandler : IRequestHandler<GetAvailableThemes, List<string>>
{
    public Task<List<string>> HandleAsync(GetAvailableThemes request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ThemeManager.AvailableThemes.ToList());
    }
}
