using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;

namespace BlazorWinForms.Forms;

/// <summary>
/// Configuration for HybridFormController.
/// Provides a fluent API for setting up hybrid forms using composition.
/// </summary>
public sealed class HybridFormConfiguration
{
    internal string HostPage { get; set; } = "wwwroot/index.html";
    internal string RootComponentSelector { get; set; } = "#app";
    internal Type? RootComponentType { get; set; }
    internal List<Assembly> HandlerAssemblies { get; } = new();
    internal string ComponentNamespace { get; set; } = "App.Web.Pages";

    internal Action<CoreWebView2>? WebView2ReadyCallback { get; set; }
    internal Action<Exception?>? WebView2FailedCallback { get; set; }
    internal Action? ThemeChangedCallback { get; set; }
    internal Action<ServiceCollection>? ConfigureServicesCallback { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridFormConfiguration"/> class.
    /// Automatically includes the calling assembly for handler discovery.
    /// </summary>
    public HybridFormConfiguration()
    {
        // Add calling assembly by default
        HandlerAssemblies.Add(Assembly.GetCallingAssembly());
    }

    /// <summary>
    /// Specifies the Blazor component to host in the form.
    /// </summary>
    public HybridFormConfiguration WithComponent<TComponent>() where TComponent : IComponent
    {
        RootComponentType = typeof(TComponent);
        return this;
    }

    /// <summary>
    /// Specifies the Blazor component to host in the form by type.
    /// </summary>
    public HybridFormConfiguration WithComponent(Type componentType)
    {
        if (!typeof(IComponent).IsAssignableFrom(componentType))
            throw new ArgumentException($"Type {componentType.Name} must implement IComponent", nameof(componentType));

        RootComponentType = componentType;
        return this;
    }

    /// <summary>
    /// Sets the namespace used for convention-based component discovery.
    /// Default: "App.Web.Pages"
    /// </summary>
    public HybridFormConfiguration WithComponentNamespace(string ns)
    {
        ComponentNamespace = ns ?? throw new ArgumentNullException(nameof(ns));
        return this;
    }

    /// <summary>
    /// Uses convention-based component discovery.
    /// Convention: FormName -> ComponentName (e.g., MainForm -> Main)
    /// </summary>
    public HybridFormConfiguration WithConventionBasedComponent(Form form)
    {
        var formType = form.GetType();
        var attribute = formType.GetCustomAttribute<HybridPageAttribute>();

        string componentTypeName;

        if (attribute?.ComponentTypeName != null)
        {
            componentTypeName = attribute.ComponentTypeName;
        }
        else if (attribute?.ComponentName != null)
        {
            componentTypeName = $"{ComponentNamespace}.{attribute.ComponentName}";
        }
        else
        {
            // Convention: MainForm -> Main, SecondForm -> Second
            var formName = formType.Name;
            var componentName = formName.EndsWith("Form")
                ? formName.Substring(0, formName.Length - 4)
                : formName;

            componentTypeName = $"{ComponentNamespace}.{componentName}";
        }

        var type = formType.Assembly.GetTypes()
            .FirstOrDefault(t => t.FullName == componentTypeName);

        if (type == null)
            throw new InvalidOperationException(
                $"Could not find Blazor component type: {componentTypeName}. " +
                $"Use WithComponent<T>() or WithComponentNamespace() to specify the component location.");

        RootComponentType = type;
        return this;
    }

    /// <summary>
    /// Sets the host page path (relative to output directory).
    /// Default: "wwwroot/index.html"
    /// </summary>
    public HybridFormConfiguration WithHostPage(string hostPage)
    {
        HostPage = hostPage ?? throw new ArgumentNullException(nameof(hostPage));
        return this;
    }

    /// <summary>
    /// Sets the root component CSS selector.
    /// Default: "#app"
    /// </summary>
    public HybridFormConfiguration WithRootComponentSelector(string selector)
    {
        RootComponentSelector = selector ?? throw new ArgumentNullException(nameof(selector));
        return this;
    }

    /// <summary>
    /// Adds an assembly to scan for request/event handlers.
    /// </summary>
    public HybridFormConfiguration WithHandlerAssembly(Assembly assembly)
    {
        if (assembly != null && !HandlerAssemblies.Contains(assembly))
            HandlerAssemblies.Add(assembly);
        return this;
    }

    /// <summary>
    /// Callback invoked when WebView2 is successfully initialized.
    /// </summary>
    public HybridFormConfiguration OnWebView2Ready(Action<CoreWebView2> callback)
    {
        WebView2ReadyCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        return this;
    }

    /// <summary>
    /// Callback invoked when WebView2 initialization fails.
    /// </summary>
    public HybridFormConfiguration OnWebView2Failed(Action<Exception?> callback)
    {
        WebView2FailedCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        return this;
    }

    /// <summary>
    /// Callback invoked when a custom event (like theme change) occurs.
    /// This is a generic hook for application-specific events.
    /// </summary>
    public HybridFormConfiguration OnCustomEvent(Action callback)
    {
        ThemeChangedCallback = callback ?? throw new ArgumentNullException(nameof(callback));
        return this;
    }

    /// <summary>
    /// Configures additional services in the dependency injection container.
    /// </summary>
    public HybridFormConfiguration ConfigureServices(Action<ServiceCollection> configure)
    {
        ConfigureServicesCallback = configure ?? throw new ArgumentNullException(nameof(configure));
        return this;
    }
}
