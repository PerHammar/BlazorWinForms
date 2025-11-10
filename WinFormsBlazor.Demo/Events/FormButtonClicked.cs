using BlazorWinForms.Interop;

namespace WinFormsBlazor.Events;

/// <summary>
/// Event published when a button is clicked in the WinForms host.
/// </summary>
public record FormButtonClicked(string Message, DateTime Timestamp) : IEvent;
