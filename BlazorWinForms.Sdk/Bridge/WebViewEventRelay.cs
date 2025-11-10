using System.Text.Json;
using Microsoft.Web.WebView2.Core;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Bridge;

/// <summary>
/// Relays events from the WinForms host to the Blazor guest via WebView2.
/// </summary>
public class WebViewEventRelay
{
    private readonly CoreWebView2? _webView;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebViewEventRelay"/> class.
    /// </summary>
    /// <param name="webView">The WebView2 control to send messages to.</param>
    public WebViewEventRelay(CoreWebView2 webView)
    {
        _webView = webView;
    }

    /// <summary>
    /// Parameterless constructor for testing/mocking purposes.
    /// </summary>
    protected WebViewEventRelay()
    {
        _webView = null;
    }

    /// <summary>
    /// Sends an event to the Blazor guest via WebView2's PostWebMessageAsJson.
    /// </summary>
    /// <param name="event">The event to send.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public virtual Task SendAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.CompletedTask;

        if (_webView == null)
            return Task.CompletedTask;

        // Serialize using concrete type to preserve properties
        var concreteType = @event.GetType();
        var eventJson = JsonSerializer.Serialize(@event, concreteType);

        var envelope = new
        {
            type = "event",
            name = concreteType.AssemblyQualifiedName,
            payload = JsonDocument.Parse(eventJson).RootElement
        };

        // Use PostWebMessageAsString instead of PostWebMessageAsJson
        // to avoid conflicts with Blazor's internal message handling
        var envelopeJson = JsonSerializer.Serialize(envelope);
        _webView.PostWebMessageAsString(envelopeJson);
        return Task.CompletedTask;
    }
}
