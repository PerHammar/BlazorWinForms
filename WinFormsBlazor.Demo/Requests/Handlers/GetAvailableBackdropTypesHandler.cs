using BlazorWinForms.Interop;
using WinFormsBlazor.Theming;

namespace WinFormsBlazor.Requests.Handlers;

public class GetAvailableBackdropTypesHandler : IRequestHandler<GetAvailableBackdropTypes, List<string>>
{
    public Task<List<string>> HandleAsync(GetAvailableBackdropTypes request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ThemeManager.AvailableBackdropTypes.ToList());
    }
}
