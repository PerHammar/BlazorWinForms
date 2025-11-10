using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using BlazorWinForms.Bridge;
using BlazorWinForms.Interop;

namespace BlazorWinForms.Forms;

/// <summary>
/// Controller that manages the hybrid form lifecycle and Blazor/WinForms interop.
/// Can be used via composition OR by HybridFormBase (inheritance).
/// </summary>
public sealed class HybridFormController : IDisposable
{
    private readonly Form _form;
    private readonly HybridFormConfiguration _config;
    private bool _blazorInitialized = false;
    private bool _interopSetup = false;
    private bool _disposed = false;

    /// <summary>
    /// Gets the request dispatcher for handling incoming requests from Blazor.
    /// </summary>
    public RequestDispatcher? RequestDispatcher { get; private set; }

    /// <summary>
    /// Gets the event bus for publishing events to Blazor.
    /// </summary>
    public EventBus? EventBus { get; private set; }

    /// <summary>
    /// Gets the BlazorWebView control hosting the Blazor content.
    /// </summary>
    public BlazorWebView? BlazorWebView { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridFormController"/> class with configuration via callback.
    /// </summary>
    /// <param name="form">The WinForms form to host the Blazor content.</param>
    /// <param name="configure">Optional callback to configure the hybrid form.</param>
    public HybridFormController(Form form, Action<HybridFormConfiguration>? configure = null)
    {
        _form = form ?? throw new ArgumentNullException(nameof(form));
        _config = new HybridFormConfiguration();

        // Auto-add the form's assembly for handler discovery (composition pattern)
        _config.WithHandlerAssembly(form.GetType().Assembly);

        configure?.Invoke(_config);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridFormController"/> class for fluent configuration.
    /// Usage: new HybridFormController(form).WithComponent&lt;T&gt;().Initialize()
    /// </summary>
    /// <param name="form">The WinForms form to host the Blazor content.</param>
    public HybridFormController(Form form)
    {
        _form = form ?? throw new ArgumentNullException(nameof(form));
        _config = new HybridFormConfiguration();

        // Auto-add the form's assembly for handler discovery (fluent pattern)
        _config.WithHandlerAssembly(form.GetType().Assembly);
    }

    /// <summary>
    /// Fluent API: Specifies the Blazor component to host.
    /// </summary>
    public HybridFormController WithComponent<TComponent>() where TComponent : Microsoft.AspNetCore.Components.IComponent
    {
        _config.WithComponent<TComponent>();
        return this;
    }

    /// <summary>
    /// Fluent API: Specifies the Blazor component by type.
    /// </summary>
    public HybridFormController WithComponent(Type componentType)
    {
        _config.WithComponent(componentType);
        return this;
    }

    /// <summary>
    /// Fluent API: Sets the namespace for convention-based component discovery.
    /// </summary>
    public HybridFormController WithComponentNamespace(string ns)
    {
        _config.WithComponentNamespace(ns);
        return this;
    }

    /// <summary>
    /// Fluent API: Uses convention-based component discovery.
    /// </summary>
    public HybridFormController WithConventionBasedComponent()
    {
        _config.WithConventionBasedComponent(_form);
        return this;
    }

    /// <summary>
    /// Fluent API: Callback when WebView2 is ready.
    /// </summary>
    public HybridFormController OnWebView2Ready(Action<CoreWebView2> callback)
    {
        _config.OnWebView2Ready(callback);
        return this;
    }

    /// <summary>
    /// Fluent API: Callback when WebView2 initialization fails.
    /// </summary>
    public HybridFormController OnWebView2Failed(Action<Exception?> callback)
    {
        _config.OnWebView2Failed(callback);
        return this;
    }

    /// <summary>
    /// Fluent API: Callback for custom events (e.g., theme changes).
    /// </summary>
    public HybridFormController OnCustomEvent(Action callback)
    {
        _config.OnCustomEvent(callback);
        return this;
    }

    /// <summary>
    /// Fluent API: Configure services.
    /// </summary>
    public HybridFormController ConfigureServices(Action<ServiceCollection> configure)
    {
        _config.ConfigureServices(configure);
        return this;
    }

    /// <summary>
    /// Initializes the hybrid form. Must be called after configuration.
    /// </summary>
    public HybridFormController Initialize()
    {
        if (_form.IsDisposed)
            throw new ObjectDisposedException(nameof(Form));

        if (_config.RootComponentType == null)
            throw new InvalidOperationException(
                "No component specified. Call WithComponent<T>() or WithConventionBasedComponent() before Initialize().");

        // Initialize request dispatcher (needed for AppBridge)
        var assemblies = _config.HandlerAssemblies.ToArray();
        RequestDispatcher = new RequestDispatcher(assemblies);
        // Note: EventBus created later in SetupJavaScriptInterop with WebView relay

        // Defer Blazor initialization until form is shown
        _form.Shown += OnFirstShown;

        return this;
    }

    private void OnFirstShown(object? sender, EventArgs e)
    {
        if (_blazorInitialized)
            return;

        _blazorInitialized = true;
        _form.Shown -= OnFirstShown;
        InitializeBlazorWebView();
    }

    private void InitializeBlazorWebView()
    {
        // HostPage path is provided by the consuming application
        // Static web assets from this library are automatically available at _content/BlazorWinForms.Sdk/
        BlazorWebView = new BlazorWebView
        {
            Dock = DockStyle.Fill,
            HostPage = _config.HostPage
        };

        // Subscribe to WebView2 initialization
        BlazorWebView.WebView.CoreWebView2InitializationCompleted += OnWebView2Initialized;

        // Configure services
        var services = CreateServiceCollection();
        _config.ConfigureServicesCallback?.Invoke(services);
        BlazorWebView.Services = services.BuildServiceProvider();

        // Add root component
        BlazorWebView.RootComponents.Add(
            new Microsoft.AspNetCore.Components.WebView.WindowsForms.RootComponent(
                _config.RootComponentSelector,
                _config.RootComponentType!,
                null));

        _form.Controls.Add(BlazorWebView);
    }

    private ServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddWindowsFormsBlazorWebView();
        // Register as SCOPED, not Singleton, so they're created within the WebView context
        services.AddScoped<RequestService>();
        services.AddScoped<EventBridgeService>();
        return services;
    }

    private async void OnWebView2Initialized(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        if (!e.IsSuccess)
        {
            _config.WebView2FailedCallback?.Invoke(e.InitializationException);
            System.Diagnostics.Debug.WriteLine($"[HybridForm] WebView2 initialization failed: {e.InitializationException?.Message}");
            return;
        }

        var webView = BlazorWebView!.WebView.CoreWebView2;
        webView.Settings.AreDevToolsEnabled = true;

        System.Diagnostics.Debug.WriteLine("[HybridForm] WebView2 initialized, setting up interop...");

        // Set up host object and event bus immediately
        SetupInteropInfrastructure();

        System.Diagnostics.Debug.WriteLine("[HybridForm] Injecting bridge JavaScript...");

        // Inject JavaScript after host object is ready
        // This runs once per document creation (including iframes)
        await webView.AddScriptToExecuteOnDocumentCreatedAsync(BridgeJavaScript.Code);

        System.Diagnostics.Debug.WriteLine("[HybridForm] JavaScript injected");

        _config.WebView2ReadyCallback?.Invoke(webView);
    }

    private void SetupInteropInfrastructure()
    {
        if (_interopSetup)
        {
            System.Diagnostics.Debug.WriteLine("[HybridForm] Interop already set up, skipping");
            return;
        }

        _interopSetup = true;

        var webView = BlazorWebView!.WebView.CoreWebView2;

        System.Diagnostics.Debug.WriteLine("[HybridForm] Creating EventBus and WebViewEventRelay...");

        // Set up event relay for WinForms → Blazor communication
        var relay = new WebViewEventRelay(webView);
        EventBus = new EventBus(relay, _config.HandlerAssemblies.ToArray());

        System.Diagnostics.Debug.WriteLine("[HybridForm] Adding host object 'appBridge' to script...");

        // Add host object for Blazor → WinForms communication
        // Must be added before AddScriptToExecuteOnDocumentCreatedAsync is called
        webView.AddHostObjectToScript("appBridge", new AppBridge(RequestDispatcher!));

        System.Diagnostics.Debug.WriteLine("[HybridForm] Host object added successfully");
    }

    /// <summary>
    /// Disposes the controller and cleans up all resources including event subscriptions.
    /// Call this method when the form is disposed to prevent memory leaks.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _form.Shown -= OnFirstShown;
    }
}
