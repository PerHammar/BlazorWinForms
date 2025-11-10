using WinFormsBlazor.Theming;
using WinFormsBlazor.Events;

namespace WinFormsBlazor;

/// <summary>
/// Singleton service that provides access to form controls for command handlers
/// and manages centralized theme updates for all windows.
/// </summary>
public sealed class FormController
{
    private static FormController? _instance;
    private MainForm? _mainForm; // Keep for specific operations

    public static FormController Instance => _instance ??= new FormController();

    private FormController()
    {
        // Single subscription point for theme changes - updates all windows
        ThemeManager.Changed += OnThemeChanged;
    }

    /// <summary>
    /// Called when theme changes - updates all open forms.
    /// Can also be called manually to force theme update on a new window.
    /// </summary>
    public void OnThemeChanged(object? sender, EventArgs e)
    {
        // Dynamically find and update ALL open forms
        foreach (Form form in Application.OpenForms)
        {
            ApplyThemeToWindow(form);
        }
    }

    private void ApplyThemeToWindow(Form? form)
    {
        if (form == null || !form.IsHandleCreated)
            return;

        var theme = ThemeManager.Current;

        if (form.InvokeRequired)
        {
            form.Invoke(() =>
            {
                form.BackColor = theme.BackgroundColor;
                form.ForeColor = theme.ForegroundColor;
                Win11Effects.EnableDarkMode(form.Handle, theme.IsDarkMode);
                Win11Effects.SetBackdropType(form.Handle, theme.BackdropType);
            });
        }
        else
        {
            form.BackColor = theme.BackgroundColor;
            form.ForeColor = theme.ForegroundColor;
            Win11Effects.EnableDarkMode(form.Handle, theme.IsDarkMode);
            Win11Effects.SetBackdropType(form.Handle, theme.BackdropType);
        }

        // Notify the window's Blazor UI about theme change
        if (form is BlazorWinForms.Forms.HybridFormBase hybridForm)
        {
            hybridForm.TriggerCustomEvent(); // Trigger custom event callback
            _ = hybridForm.PublishEventAsync(new ThemeChanged(theme.Name));
        }
    }

    public void RegisterMainForm(MainForm form)
    {
        _mainForm = form;
        // Apply initial theme to the form
        ApplyThemeToWindow(form);
    }

    public SecondForm? FindSecondForm()
    {
        // Dynamically find SecondForm in open forms
        foreach (Form form in Application.OpenForms)
        {
            if (form is SecondForm secondForm)
                return secondForm;
        }
        return null;
    }

    public void UpdateFormText(string text)
    {
        if (_mainForm?.FormTextBox == null)
            return;

        if (_mainForm.InvokeRequired)
        {
            _mainForm.Invoke(() => _mainForm.FormTextBox.Text = text);
        }
        else
        {
            _mainForm.FormTextBox.Text = text;
        }
    }

    public void CloseSecondForm()
    {
        var secondForm = FindSecondForm();
        if (secondForm == null)
            return;

        if (secondForm.InvokeRequired)
        {
            secondForm.Invoke(() => secondForm.Close());
        }
        else
        {
            secondForm.Close();
        }
    }
}
