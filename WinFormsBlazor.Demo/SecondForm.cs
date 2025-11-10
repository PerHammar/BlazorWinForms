using System.Drawing;
using BlazorWinForms.Forms;

namespace WinFormsBlazor;

/// <summary>
/// Second form demonstrating that multiple forms can host different Blazor components.
/// Theming is centrally managed by FormController - no manual subscription needed.
/// </summary>
public partial class SecondForm : HybridFormBase
{
    public SecondForm()
    {
        InitializeComponent();

        if (DesignMode)
            return;

        Text = "Simple Demo Window";
        Size = new Size(650, 550);
        StartPosition = FormStartPosition.CenterParent;

        InitializeHybridForm();

        // FormController will automatically find and theme this window via Application.OpenForms
        // Apply initial theme when window is shown
        Load += (s, e) => FormController.Instance.OnThemeChanged(null, EventArgs.Empty);
    }

    protected override string GetComponentNamespace() => "WinFormsBlazor.Web.Pages";

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
