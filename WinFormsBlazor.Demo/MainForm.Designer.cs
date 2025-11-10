namespace WinFormsBlazor;

partial class MainForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private Panel controlPanel;
    private Button sendEventButton;
    private Button openSecondFormButton;
    private TextBox formTextBox;
    private Label textBoxLabel;

    public TextBox FormTextBox => formTextBox;

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        controlPanel = new Panel();
        sendEventButton = new Button();
        openSecondFormButton = new Button();
        formTextBox = new TextBox();
        textBoxLabel = new Label();

        // controlPanel
        controlPanel.Dock = DockStyle.Bottom;
        controlPanel.Height = 80;
        controlPanel.Padding = new Padding(10);
        controlPanel.Controls.Add(openSecondFormButton);
        controlPanel.Controls.Add(sendEventButton);
        controlPanel.Controls.Add(formTextBox);
        controlPanel.Controls.Add(textBoxLabel);

        // textBoxLabel
        textBoxLabel.Text = "Text from Blazor:";
        textBoxLabel.AutoSize = true;
        textBoxLabel.Location = new Point(10, 15);

        // formTextBox
        formTextBox.Location = new Point(120, 12);
        formTextBox.Width = 300;
        formTextBox.PlaceholderText = "Blazor can update this text";

        // sendEventButton
        sendEventButton.Text = "Send Event to Blazor";
        sendEventButton.Location = new Point(430, 10);
        sendEventButton.Width = 180;
        sendEventButton.Height = 30;
        sendEventButton.Click += SendEventButton_Click;

        // openSecondFormButton
        openSecondFormButton.Text = "Open Simple Window";
        openSecondFormButton.Location = new Point(620, 10);
        openSecondFormButton.Width = 180;
        openSecondFormButton.Height = 30;
        openSecondFormButton.Click += OpenSecondFormButton_Click;

        // MainForm
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 700);
        Text = "WinForms + Blazor Hybrid";
        Controls.Add(controlPanel);
    }

    #endregion
}
