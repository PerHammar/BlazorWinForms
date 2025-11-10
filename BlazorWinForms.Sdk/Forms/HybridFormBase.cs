using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using BlazorWinForms.Interop;
using System.ComponentModel;

namespace BlazorWinForms.Forms;

/// <summary>
/// Base class for WinForms that host Blazor components using inheritance.
/// Provides automatic setup of Interop infrastructure.
/// For composition-based approach, use HybridFormController directly.
/// </summary>
[DesignerCategory("Code")]
public abstract class HybridFormBase : Form
{
    private HybridFormController? _controller;

    /// <summary>
    /// Gets the request dispatcher for handling incoming requests from Blazor.
    /// </summary>
    protected RequestDispatcher? RequestDispatcher => _controller?.RequestDispatcher;

    /// <summary>
    /// Gets the event bus for publishing events to Blazor.
    /// </summary>
    protected EventBus? EventBus => _controller?.EventBus;

    /// <summary>
    /// Gets the BlazorWebView control hosting the Blazor content.
    /// </summary>
    protected BlazorWebView? BlazorWebView => _controller?.BlazorWebView;

    /// <summary>
    /// Initializes a new instance of the HybridFormBase class using convention-based configuration.
    /// Convention: FormName -> ComponentName (e.g., MainForm -> Main, SecondForm -> Second)
    /// Use [HybridPage] attribute to override.
    /// </summary>
    protected HybridFormBase()
    {
        // Controller will be created in InitializeHybridForm
    }

    /// <summary>
    /// Initializes the hybrid form. Call this from your constructor after InitializeComponent().
    /// Override SetComponentNamespace() to customize the namespace for component discovery.
    /// </summary>
    protected void InitializeHybridForm()
    {
        if (DesignMode)
            return;

        _controller = new HybridFormController(this, config =>
        {
            // CRITICAL: Add the derived form's assembly for handler discovery
            config.WithHandlerAssembly(this.GetType().Assembly);
            config.WithComponentNamespace(GetComponentNamespace());
            config.WithConventionBasedComponent(this);
            config.OnWebView2Ready(OnWebView2Ready);
            config.OnWebView2Failed(OnWebView2InitializationFailed);
            config.OnCustomEvent(OnCustomEvent);
            config.ConfigureServices(ConfigureServices);
        });

        _controller.Initialize();
    }

    /// <summary>
    /// Override this to set the namespace for convention-based component discovery.
    /// Default: "App.Web.Pages"
    /// </summary>
    protected virtual string GetComponentNamespace() => "App.Web.Pages";

    /// <summary>
    /// Override this to add additional services to the DI container.
    /// </summary>
    protected virtual void ConfigureServices(ServiceCollection services)
    {
        // Override in derived classes to add custom services
    }

    /// <summary>
    /// Called when WebView2 initialization fails.
    /// </summary>
    protected virtual void OnWebView2InitializationFailed(Exception? exception)
    {
        System.Diagnostics.Debug.WriteLine($"WebView2 initialization failed: {exception?.Message}");
    }

    /// <summary>
    /// Called when WebView2 is successfully initialized and ready.
    /// </summary>
    protected virtual void OnWebView2Ready(Microsoft.Web.WebView2.Core.CoreWebView2 webView)
    {
        // Override in derived classes for custom initialization
    }

    /// <summary>
    /// Called when a custom event occurs (e.g., theme change, settings update).
    /// Override this to handle application-specific events.
    /// </summary>
    protected virtual void OnCustomEvent()
    {
        // Override in derived classes for custom event handling
    }

    /// <summary>
    /// Triggers the custom event callback. This is a public method that can be called
    /// by external controllers (e.g., FormController for centralized theme management).
    /// </summary>
    public void TriggerCustomEvent()
    {
        OnCustomEvent();
    }

    /// <summary>
    /// Publishes an event to the Blazor UI via the EventBus.
    /// This is a public method for external controllers to send events to Blazor components.
    /// </summary>
    /// <typeparam name="TEvent">The type of event to publish.</typeparam>
    /// <param name="eventData">The event data to publish.</param>
    public async Task PublishEventAsync<TEvent>(TEvent eventData) where TEvent : BlazorWinForms.Interop.IEvent
    {
        if (EventBus != null)
        {
            await EventBus.PublishAsync(eventData);
        }
    }

    /// <summary>
    /// Disposes the form and cleans up the hybrid form controller.
    /// Override this method to perform custom cleanup, but be sure to call base.Dispose(disposing).
    /// </summary>
    /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _controller?.Dispose();
        }
        base.Dispose(disposing);
    }
}
