using System.Drawing;
using BlazorWinForms.Forms;
using WinFormsBlazor.Events;

namespace WinFormsBlazor;

/// <summary>
/// Main application form that hosts the Blazor WebView.
/// Inherits from HybridFormBase for automatic interop setup.
/// Theming is centrally managed by FormController (not individual windows).
/// </summary>
public partial class MainForm : HybridFormBase
{
    public MainForm()
    {
        InitializeComponent();

        if (DesignMode)
            return;

        Text = "WinForms + Blazor Hybrid Demo - Press F12 for DevTools";
        Size = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterScreen;
        KeyPreview = true; // Enable form to receive key events

        InitializeHybridForm();
        FormController.Instance.RegisterMainForm(this); // Centralized theme handling
    }

    protected override string GetComponentNamespace() => "WinFormsBlazor.Web.Pages";

    private async void SendEventButton_Click(object? sender, EventArgs e)
    {
        if (EventBus == null)
            return;

        var message = $"Button clicked at {DateTime.Now:HH:mm:ss}";
        await EventBus.PublishAsync(new FormButtonClicked(message, DateTime.Now));
    }

    private void OpenSecondFormButton_Click(object? sender, EventArgs e)
    {
        var secondForm = new SecondForm();
        secondForm.Show(this);
    }

    protected override void OnWebView2Ready(Microsoft.Web.WebView2.Core.CoreWebView2 webView)
    {
        base.OnWebView2Ready(webView);
        System.Diagnostics.Debug.WriteLine("[MainForm] WebView2 ready - Press F12 to open DevTools");
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // F12 to toggle DevTools
        if (e.KeyCode == Keys.F12)
        {
            if (BlazorWebView?.WebView?.CoreWebView2 != null)
            {
                BlazorWebView.WebView.CoreWebView2.OpenDevToolsWindow();
                e.Handled = true;
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
        }
        base.Dispose(disposing);
    }
}
