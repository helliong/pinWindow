using System.Drawing;
using System.Windows.Forms;

namespace PinWindow;

internal sealed class SettingsForm : Form
{
    private readonly Func<AppSettings, string?> _applySettings;

    private readonly CheckBox _controlCheckBox;
    private readonly CheckBox _altCheckBox;
    private readonly CheckBox _shiftCheckBox;
    private readonly CheckBox _winCheckBox;
    private readonly ComboBox _keyComboBox;

    private readonly NumericUpDown _pinSizeInput;
    private readonly NumericUpDown _offsetXInput;
    private readonly NumericUpDown _offsetYInput;
    private readonly Button _colorButton;

    private readonly CheckBox _showButtonCheckBox;
    private readonly CheckBox _notificationsCheckBox;
    private readonly CheckBox _autostartCheckBox;
    private readonly ComboBox _languageComboBox;

    private Color _selectedColor;

    public SettingsForm(AppSettings settings, Func<AppSettings, string?> applySettings)
    {
        _applySettings = applySettings;
        _selectedColor = settings.GetActiveColor();

        Text = AppLocalization.Text("Настройки PinWindow", "PinWindow Settings");
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(520, 515);
        Font = new Font("Segoe UI", 9F);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18),
            ColumnCount = 1,
            RowCount = 5,
            AutoSize = false,
        };
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        Controls.Add(root);

        var heading = new Label
        {
            Text = AppLocalization.Text("Настройки PinWindow", "PinWindow Settings"),
            AutoSize = true,
            Font = new Font(Font.FontFamily, 16F, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 4),
        };
        root.Controls.Add(heading);

        var subtitle = new Label
        {
            Text = AppLocalization.Text(
                "Изменения сохраняются в %AppData%\\PinWindow\\settings.json",
                "Settings are saved to %AppData%\\PinWindow\\settings.json"
            ),
            AutoSize = true,
            ForeColor = SystemColors.GrayText,
            Margin = new Padding(0, 0, 0, 14),
        };
        root.Controls.Add(subtitle);

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            AutoScroll = true,
            Margin = Padding.Empty,
        };
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.Controls.Add(content);

        var hotkeyGroup = new GroupBox
        {
            Text = AppLocalization.Text("Горячая клавиша", "Keyboard shortcut"),
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12),
            Margin = new Padding(0, 0, 0, 12),
        };
        content.Controls.Add(hotkeyGroup);

        var hotkeyLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
        };
        hotkeyGroup.Controls.Add(hotkeyLayout);

        _controlCheckBox = CreateModifierCheckBox("Ctrl", settings.HotkeyControl);
        _altCheckBox = CreateModifierCheckBox("Alt", settings.HotkeyAlt);
        _shiftCheckBox = CreateModifierCheckBox("Shift", settings.HotkeyShift);
        _winCheckBox = CreateModifierCheckBox("Win", settings.HotkeyWin);

        hotkeyLayout.Controls.Add(_controlCheckBox);
        hotkeyLayout.Controls.Add(_altCheckBox);
        hotkeyLayout.Controls.Add(_shiftCheckBox);
        hotkeyLayout.Controls.Add(_winCheckBox);

        _keyComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 125,
            Margin = new Padding(12, 3, 0, 3),
        };

        foreach (var option in CreateKeyOptions())
        {
            _keyComboBox.Items.Add(option);
        }

        var selectedKeyIndex = FindKeyIndex((Keys)settings.HotkeyKey);
        _keyComboBox.SelectedIndex =
            selectedKeyIndex >= 0 ? selectedKeyIndex : FindKeyIndex(Keys.T);

        hotkeyLayout.Controls.Add(_keyComboBox);

        var appearanceGroup = new GroupBox
        {
            Text = AppLocalization.Text("Внешний вид булавки", "Pin appearance"),
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12),
            Margin = new Padding(0, 0, 0, 12),
        };
        content.Controls.Add(appearanceGroup);

        var appearanceGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 2,
            RowCount = 4,
        };
        appearanceGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62));
        appearanceGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38));
        appearanceGroup.Controls.Add(appearanceGrid);

        _pinSizeInput = CreateNumberInput(20, 42, settings.PinSize);
        _offsetXInput = CreateNumberInput(-80, 80, settings.OffsetX);
        _offsetYInput = CreateNumberInput(-80, 80, settings.OffsetY);

        AddSettingRow(
            appearanceGrid,
            0,
            AppLocalization.Text("Размер кнопки", "Button size"),
            _pinSizeInput
        );
        AddSettingRow(
            appearanceGrid,
            1,
            AppLocalization.Text("Смещение по горизонтали", "Horizontal offset"),
            _offsetXInput
        );
        AddSettingRow(
            appearanceGrid,
            2,
            AppLocalization.Text("Смещение по вертикали", "Vertical offset"),
            _offsetYInput
        );
        _colorButton = new Button
        {
            Text = AppSettings.ColorToHtml(_selectedColor),
            Width = 125,
            Height = 30,
            FlatStyle = FlatStyle.Flat,
        };
        _colorButton.Click += (_, _) => SelectColor();
        UpdateColorButton();
        AddSettingRow(
            appearanceGrid,
            3,
            AppLocalization.Text("Цвет закреплённой булавки", "Pinned icon color"),
            _colorButton
        );
        var behaviorGroup = new GroupBox
        {
            Text = AppLocalization.Text("Поведение", "Behavior"),
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12),
            Margin = new Padding(0),
        };
        content.Controls.Add(behaviorGroup);

        var behaviorLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
        };
        behaviorGroup.Controls.Add(behaviorLayout);

        var languageLayout = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 8),
        };

        var languageLabel = new Label
        {
            Text = "Язык интерфейса:",
            AutoSize = true,
            Margin = new Padding(0, 7, 10, 0),
        };

        _languageComboBox = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 125,
            Margin = new Padding(0, 3, 0, 3),
        };

        _languageComboBox.Items.Add("Русский");
        _languageComboBox.Items.Add("English");

        _languageComboBox.SelectedIndex = settings.Language.Equals(
            "en",
            StringComparison.OrdinalIgnoreCase
        )
            ? 1
            : 0;

        languageLayout.Controls.Add(languageLabel);
        languageLayout.Controls.Add(_languageComboBox);

        behaviorLayout.Controls.Add(languageLayout);

        _showButtonCheckBox = CreateBehaviorCheckBox(
            AppLocalization.Text(
                "Показывать кнопку у активного окна",
                "Show button near active window"
            ),
            settings.ShowButton
        );

        _notificationsCheckBox = CreateBehaviorCheckBox(
            AppLocalization.Text("Показывать уведомления", "Show notifications"),
            settings.ShowNotifications
        );

        _autostartCheckBox = CreateBehaviorCheckBox(
            AppLocalization.Text(
                "Запускать PinWindow вместе с Windows",
                "Run PinWindow with Windows"
            ),
            settings.StartWithWindows
        );

        behaviorLayout.Controls.Add(_showButtonCheckBox);
        behaviorLayout.Controls.Add(_notificationsCheckBox);
        behaviorLayout.Controls.Add(_autostartCheckBox);

        var assemblyVersion = typeof(SettingsForm).Assembly.GetName().Version;

        var versionText = assemblyVersion is null
            ? "неизвестна"
            : $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{Math.Max(0, assemblyVersion.Build)}";

        var versionPanel = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 10, 0, 0),
        };

        var versionLabel = new Label
        {
            Text = $"PinWindow v{versionText}",
            AutoSize = true,
            ForeColor = SystemColors.GrayText,
            Margin = new Padding(0, 3, 0, 0),
        };

        var projectLink = new LinkLabel
        {
            Text = "GitHub",
            AutoSize = true,
            LinkBehavior = LinkBehavior.HoverUnderline,
            Margin = new Padding(10, 3, 0, 0),
        };

        projectLink.LinkClicked += (_, _) =>
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/helliong/pinWindow",
                    UseShellExecute = true,
                }
            );
        };

        var bugReportLink = new LinkLabel
        {
            Text = AppLocalization.Text("Сообщить об ошибке", "Bug report"),
            AutoSize = true,
            LinkBehavior = LinkBehavior.HoverUnderline,
            Margin = new Padding(10, 3, 0, 0),
        };

        bugReportLink.LinkClicked += (_, _) =>
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/helliong/pinWindow/issues/new",
                    UseShellExecute = true,
                }
            );
        };

        versionPanel.Controls.Add(versionLabel);
        versionPanel.Controls.Add(projectLink);
        versionPanel.Controls.Add(bugReportLink);

        root.Controls.Add(versionPanel);

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Padding = new Padding(0, 14, 0, 0),
        };
        root.Controls.Add(buttonPanel);

        var saveButton = new Button
        {
            Text = AppLocalization.Text("Сохранить", "Save"),
            AutoSize = true,
            Padding = new Padding(12, 3, 12, 3),
        };
        saveButton.Click += (_, _) => SaveSettings();

        var cancelButton = new Button
        {
            Text = AppLocalization.Text("Отмена", "Cancel"),
            DialogResult = DialogResult.Cancel,
            AutoSize = true,
            Padding = new Padding(12, 3, 12, 3),
        };

        var defaultsButton = new Button
        {
            Text = AppLocalization.Text("По умолчанию", "Defaults"),
            AutoSize = true,
            Padding = new Padding(8, 3, 8, 3),
            Margin = new Padding(0, 0, 16, 0),
        };
        defaultsButton.Click += (_, _) => LoadValues(AppSettings.CreateDefault());

        buttonPanel.Controls.Add(saveButton);
        buttonPanel.Controls.Add(cancelButton);
        buttonPanel.Controls.Add(defaultsButton);

        AcceptButton = saveButton;
        CancelButton = cancelButton;
    }

    private static CheckBox CreateModifierCheckBox(string text, bool isChecked) =>
        new()
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            Margin = new Padding(0, 6, 12, 3),
        };

    private static CheckBox CreateBehaviorCheckBox(string text, bool isChecked) =>
        new()
        {
            Text = text,
            Checked = isChecked,
            AutoSize = true,
            Margin = new Padding(0, 3, 0, 8),
        };

    private static NumericUpDown CreateNumberInput(int minimum, int maximum, int value) =>
        new()
        {
            Minimum = minimum,
            Maximum = maximum,
            Value = Math.Clamp(value, minimum, maximum),
            Width = 125,
            TextAlign = HorizontalAlignment.Right,
        };

    private static void AddSettingRow(
        TableLayoutPanel grid,
        int row,
        string labelText,
        Control control
    )
    {
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = labelText,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 7, 8, 8),
        };

        control.Anchor = AnchorStyles.Right;
        control.Margin = new Padding(8, 3, 0, 6);

        grid.Controls.Add(label, 0, row);
        grid.Controls.Add(control, 1, row);
    }

    private static IReadOnlyList<HotkeyKeyOption> CreateKeyOptions()
    {
        var options = new List<HotkeyKeyOption>();

        for (var value = (int)Keys.A; value <= (int)Keys.Z; value++)
        {
            options.Add(new HotkeyKeyOption((Keys)value));
        }

        for (var value = (int)Keys.D0; value <= (int)Keys.D9; value++)
        {
            options.Add(new HotkeyKeyOption((Keys)value));
        }

        for (var value = (int)Keys.F1; value <= (int)Keys.F12; value++)
        {
            options.Add(new HotkeyKeyOption((Keys)value));
        }

        options.Add(new HotkeyKeyOption(Keys.Space));
        options.Add(new HotkeyKeyOption(Keys.Insert));
        options.Add(new HotkeyKeyOption(Keys.Home));
        options.Add(new HotkeyKeyOption(Keys.End));
        options.Add(new HotkeyKeyOption(Keys.PageUp));
        options.Add(new HotkeyKeyOption(Keys.PageDown));

        return options;
    }

    private int FindKeyIndex(Keys key)
    {
        for (var index = 0; index < _keyComboBox.Items.Count; index++)
        {
            if (_keyComboBox.Items[index] is HotkeyKeyOption option && option.Key == key)
            {
                return index;
            }
        }

        return -1;
    }

    private void SelectColor()
    {
        using var dialog = new ColorDialog
        {
            Color = _selectedColor,
            FullOpen = true,
            AnyColor = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _selectedColor = dialog.Color;
        UpdateColorButton();
    }

    private void UpdateColorButton()
    {
        _colorButton.Text = AppSettings.ColorToHtml(_selectedColor);
        _colorButton.BackColor = _selectedColor;
        _colorButton.ForeColor = GetContrastingTextColor(_selectedColor);
        _colorButton.UseVisualStyleBackColor = false;
    }

    private static Color GetContrastingTextColor(Color color)
    {
        var luminance = 0.299 * color.R + 0.587 * color.G + 0.114 * color.B;

        return luminance > 150 ? Color.Black : Color.White;
    }

    private void LoadValues(AppSettings settings)
    {
        _languageComboBox.SelectedIndex = settings.Language.Equals(
            "en",
            StringComparison.OrdinalIgnoreCase
        )
            ? 1
            : 0;
        _controlCheckBox.Checked = settings.HotkeyControl;
        _altCheckBox.Checked = settings.HotkeyAlt;
        _shiftCheckBox.Checked = settings.HotkeyShift;
        _winCheckBox.Checked = settings.HotkeyWin;

        var index = FindKeyIndex((Keys)settings.HotkeyKey);
        _keyComboBox.SelectedIndex = index >= 0 ? index : FindKeyIndex(Keys.T);

        _pinSizeInput.Value = settings.PinSize;
        _offsetXInput.Value = settings.OffsetX;
        _offsetYInput.Value = settings.OffsetY;

        _selectedColor = settings.GetActiveColor();
        UpdateColorButton();

        _showButtonCheckBox.Checked = settings.ShowButton;
        _notificationsCheckBox.Checked = settings.ShowNotifications;
        _autostartCheckBox.Checked = settings.StartWithWindows;
    }

    private void SaveSettings()
    {
        if (
            !_controlCheckBox.Checked
            && !_altCheckBox.Checked
            && !_shiftCheckBox.Checked
            && !_winCheckBox.Checked
        )
        {
            MessageBox.Show(
                this,
                AppLocalization.Text(
                    "Выберите хотя бы один модификатор: Ctrl, Alt, Shift или Win.",
                    "Select at least one modifier: Ctrl, Alt, Shift, or Win."
                ),
                AppLocalization.Text("Горячая клавиша", "Keyboard shortcut"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        if (_keyComboBox.SelectedItem is not HotkeyKeyOption keyOption)
        {
            MessageBox.Show(
                this,
                AppLocalization.Text("Выберите клавишу.", "Select a key."),
                AppLocalization.Text("Горячая клавиша", "Keyboard shortcut"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
            return;
        }

        var settings = new AppSettings
        {
            Language = _languageComboBox.SelectedIndex == 1 ? "en" : "ru",
            ShowButton = _showButtonCheckBox.Checked,
            ShowNotifications = _notificationsCheckBox.Checked,
            StartWithWindows = _autostartCheckBox.Checked,
            PinSize = Decimal.ToInt32(_pinSizeInput.Value),
            OffsetX = Decimal.ToInt32(_offsetXInput.Value),
            OffsetY = Decimal.ToInt32(_offsetYInput.Value),
            ActiveColor = AppSettings.ColorToHtml(_selectedColor),
            HotkeyControl = _controlCheckBox.Checked,
            HotkeyAlt = _altCheckBox.Checked,
            HotkeyShift = _shiftCheckBox.Checked,
            HotkeyWin = _winCheckBox.Checked,
            HotkeyKey = (int)keyOption.Key,
        };

        var error = _applySettings(settings);

        if (error is not null)
        {
            MessageBox.Show(
                this,
                error,
                AppLocalization.Text("Не удалось сохранить настройки", "Failed to save settings"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed class HotkeyKeyOption
    {
        public HotkeyKeyOption(Keys key)
        {
            Key = key;
        }

        public Keys Key { get; }

        public override string ToString() => HotkeyKeyNames.ToDisplayName(Key);
    }
}
