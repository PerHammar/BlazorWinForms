using BlazorWinForms.Interop;
using WinFormsBlazor.Theming;

namespace WinFormsBlazor.Requests.Handlers;

public class GetUserSettingsHandler : IRequestHandler<GetUserSettings, UserSettings>
{
    public Task<UserSettings> HandleAsync(GetUserSettings request, CancellationToken cancellationToken = default)
    {
        var settings = new UserSettings(
            Theme: ThemeManager.Current.Name,
            NotificationsEnabled: true,
            BackdropType: ThemeManager.Current.BackdropType.ToString()
        );

        return Task.FromResult(settings);
    }
}
