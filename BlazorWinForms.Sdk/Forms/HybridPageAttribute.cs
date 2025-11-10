namespace BlazorWinForms.Forms;

/// <summary>
/// Specifies the Blazor component to host in a HybridFormBase.
/// If not specified, uses convention: FormName -> ComponentName (e.g., MainForm -> Main)
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class HybridPageAttribute : Attribute
{
    /// <summary>
    /// The full type name of the Blazor component to host.
    /// Example: "MyApp.Web.Pages.CustomPage"
    /// </summary>
    public string? ComponentTypeName { get; set; }

    /// <summary>
    /// The simple name of the Blazor component (without namespace).
    /// Convention will add configured namespace prefix.
    /// Example: "CustomPage" becomes "{ConfiguredNamespace}.CustomPage"
    /// </summary>
    public string? ComponentName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridPageAttribute"/> class.
    /// Uses convention-based component discovery.
    /// </summary>
    public HybridPageAttribute() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HybridPageAttribute"/> class with a component name.
    /// </summary>
    /// <param name="componentName">The simple name of the Blazor component (without namespace).</param>
    public HybridPageAttribute(string componentName)
    {
        ComponentName = componentName;
    }
}
