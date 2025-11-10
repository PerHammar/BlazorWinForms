using BlazorWinForms.Interop;

namespace WinFormsBlazor.Requests;

/// <summary>
/// Request to retrieve current user settings from the host.
/// </summary>
public record GetUserSettings() : IRequest<UserSettings>;

/// <summary>
/// User settings data transfer object.
/// </summary>
public record UserSettings(string Theme, bool NotificationsEnabled, string BackdropType);
