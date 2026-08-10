namespace PinWindow;

internal sealed class UpdateAvailableForm : Form
{
    public UpdateAvailableForm(Version currentVersion, Version latestVersion, Action downloadUpdate)
    {
        Text = AppLocalization.Text("Обновление PinWindow", "PinWindow Update");

        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        TopMost = true;
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(470, 260);
        MinimumSize = new Size(470, 260);
        Font = new Font("Segoe UI", 9F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(22),
            ColumnCount = 1,
            RowCount = 4,
        };

        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        Controls.Add(root);

        var heading = new Label
        {
            Text = AppLocalization.Text("Доступна новая версия", "A new version is available"),
            AutoSize = true,
            Font = new Font(Font.FontFamily, 15F, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 10),
        };

        root.Controls.Add(heading);

        var description = new Label
        {
            Text = AppLocalization.Text(
                "Доступно обновление PinWindow. Рекомендуется установить последнюю версию.",
                "A PinWindow update is available. Installing the latest version is recommended."
            ),
            AutoSize = true,
            MaximumSize = new Size(390, 0),
            Margin = new Padding(0, 0, 0, 14),
        };

        root.Controls.Add(description);

        var versionLabel = new Label
        {
            Text = AppLocalization.Text(
                $"Текущая версия: {FormatVersion(currentVersion)}\nНовая версия: {FormatVersion(latestVersion)}",
                $"Current version: {FormatVersion(currentVersion)}\nNew version: {FormatVersion(latestVersion)}"
            ),
            AutoSize = true,
            ForeColor = SystemColors.GrayText,
            Margin = new Padding(0, 0, 0, 10),
        };

        root.Controls.Add(versionLabel);

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 14, 0, 0),
        };

        root.Controls.Add(buttons);

        var downloadButton = new Button
        {
            Text = AppLocalization.Text("Скачать", "Download"),
            AutoSize = true,
            Padding = new Padding(14, 4, 14, 4),
        };

        downloadButton.Click += (_, _) =>
        {
            downloadUpdate();
            Close();
        };

        var laterButton = new Button
        {
            Text = AppLocalization.Text("Позже", "Later"),
            AutoSize = true,
            Padding = new Padding(14, 4, 14, 4),
            DialogResult = DialogResult.Cancel,
        };

        buttons.Controls.Add(downloadButton);
        buttons.Controls.Add(laterButton);

        AcceptButton = downloadButton;
        CancelButton = laterButton;
    }

    private static string FormatVersion(Version version)
    {
        return version.Build >= 0
            ? $"{version.Major}.{version.Minor}.{version.Build}"
            : $"{version.Major}.{version.Minor}";
    }
}
