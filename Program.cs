using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PinWindow;

internal static class Program
{
    private const string MutexName =
        "PinWindow.SingleInstance.1499A0DA-7CD9-43C8-88E1-67E4DBCEAD43";

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var mutex = new Mutex(
            initiallyOwned: true,
            name: MutexName,
            createdNew: out var isFirstInstance);

        if (!isFirstInstance)
        {
            MessageBox.Show(
                "PinWindow уже запущен. Найдите его значок в системном трее.",
                "PinWindow",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            Application.Run(new TrayApplicationContext());
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.ToString(),
                "Ошибка PinWindow",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly HotkeyWindow _hotkeyWindow;
    private readonly PinController _pinController;
    private readonly ToolStripMenuItem _toggleWindowItem;
    private readonly ToolStripMenuItem _settingsItem;
    private readonly ToolStripMenuItem _showButtonItem;
    private readonly ToolStripMenuItem _hotkeyLabel;
    private readonly ToolStripMenuItem _exitItem;

    private AppSettings _settings;
    private SettingsForm? _settingsForm;
    private bool _updatingMenu;

    public TrayApplicationContext()
    {
        _settings = SettingsStore.Load();

        var menu = new ContextMenuStrip();

        _toggleWindowItem = new ToolStripMenuItem(
            "Закрепить/открепить активное окно",
            null,
            (_, _) => ToggleForegroundWindow());

        _settingsItem = new ToolStripMenuItem(
            "Настройки…",
            null,
            (_, _) => OpenSettings());

        _showButtonItem = new ToolStripMenuItem(
            "Показывать кнопку у активного окна")
        {
            CheckOnClick = true
        };
        _showButtonItem.CheckedChanged += (_, _) =>
            ToggleButtonVisibilityFromMenu();

        _hotkeyLabel = new ToolStripMenuItem
        {
            Enabled = false
        };

        _exitItem = new ToolStripMenuItem(
            "Выход",
            null,
            (_, _) => ExitThread());

        menu.Items.Add(_toggleWindowItem);
        menu.Items.Add(_settingsItem);
        menu.Items.Add(_showButtonItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_hotkeyLabel);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_exitItem);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "PinWindow — закрепление окон",
            ContextMenuStrip = menu,
            Visible = true
        };
        _notifyIcon.DoubleClick += (_, _) => OpenSettings();

        _pinController = new PinController(
            _settings,
            ShowResult);

        _hotkeyWindow = new HotkeyWindow(
            ToggleForegroundWindow,
            _settings.GetHotkey());

        UpdateMenuFromSettings();

        if (_settings.ShowNotifications)
        {
            _notifyIcon.ShowBalloonTip(
                2600,
                "PinWindow запущен",
                $"Откройте настройки двойным кликом по значку в трее. Горячая клавиша: {_settings.GetHotkey().DisplayText}.",
                ToolTipIcon.Info);
        }
    }

    private void ToggleForegroundWindow()
    {
        var window = NativeMethods.GetForegroundWindow();
        var result = WindowPinning.ToggleWindow(window);

        _pinController.RefreshNow();
        ShowResult(result);
    }

    private void OpenSettings()
    {
        if (_settingsForm is { IsDisposed: false })
        {
            if (_settingsForm.WindowState == FormWindowState.Minimized)
            {
                _settingsForm.WindowState = FormWindowState.Normal;
            }

            _settingsForm.Activate();
            _settingsForm.BringToFront();
            return;
        }

        _settingsForm = new SettingsForm(
            _settings,
            ApplySettings);

        _settingsForm.FormClosed += (_, _) =>
            _settingsForm = null;

        _settingsForm.Show();
        _settingsForm.Activate();
    }

    private string? ApplySettings(AppSettings requestedSettings)
    {
        requestedSettings.Normalize();

        var previousSettings = _settings.Clone();

        try
        {
            _hotkeyWindow.UpdateHotkey(
                requestedSettings.GetHotkey());

            AutostartManager.SetEnabled(
                requestedSettings.StartWithWindows);

            SettingsStore.Save(requestedSettings);
            _settings = requestedSettings.Clone();

            _pinController.ApplySettings(_settings);
            UpdateMenuFromSettings();

            if (_settings.ShowNotifications)
            {
                _notifyIcon.ShowBalloonTip(
                    1500,
                    "PinWindow",
                    "Настройки сохранены.",
                    ToolTipIcon.Info);
            }

            return null;
        }
        catch (Exception exception)
        {
            try
            {
                _hotkeyWindow.UpdateHotkey(
                    previousSettings.GetHotkey());

                AutostartManager.SetEnabled(
                    previousSettings.StartWithWindows);
            }
            catch
            {
                // Не скрываем первоначальную ошибку отката.
            }

            return exception.Message;
        }
    }

    private void ToggleButtonVisibilityFromMenu()
    {
        if (_updatingMenu)
        {
            return;
        }

        _settings.ShowButton = _showButtonItem.Checked;
        _pinController.ApplySettings(_settings);

        try
        {
            SettingsStore.Save(_settings);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "PinWindow",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void UpdateMenuFromSettings()
    {
        _updatingMenu = true;

        try
        {
            _showButtonItem.Checked = _settings.ShowButton;
            _hotkeyLabel.Text =
                $"Горячая клавиша: {_settings.GetHotkey().DisplayText}";
        }
        finally
        {
            _updatingMenu = false;
        }
    }

    private void ShowResult(ToggleResult result)
    {
        if (result.Success && !_settings.ShowNotifications)
        {
            return;
        }

        _notifyIcon.ShowBalloonTip(
            1300,
            result.Success ? "PinWindow" : "Не удалось изменить окно",
            result.Message,
            result.Success ? ToolTipIcon.Info : ToolTipIcon.Error);
    }

    protected override void ExitThreadCore()
    {
        _settingsForm?.Close();
        _pinController.Dispose();
        _hotkeyWindow.Dispose();

        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();

        base.ExitThreadCore();
    }
}

internal sealed class PinController : IDisposable
{
    private readonly PinOverlayForm _overlay;
    private readonly WinEventMonitor _eventMonitor;
    private readonly System.Windows.Forms.Timer _moveTimer;
    private readonly System.Windows.Forms.Timer _settleTimer;

    private IntPtr _targetWindow;
    private bool _enabled;
    private bool _disposed;
    private int _refreshPosted;
    private int _settleAttemptsRemaining;

    public PinController(
        AppSettings settings,
        Action<ToggleResult> showResult)
    {
        _enabled = settings.ShowButton;
        _overlay = new PinOverlayForm(
            ToggleTargetWindow,
            showResult,
            settings);

        _moveTimer = new System.Windows.Forms.Timer
        {
            Interval = 16
        };
        _moveTimer.Tick += (_, _) => UpdateOverlayPosition();

        _settleTimer = new System.Windows.Forms.Timer
        {
            Interval = 45
        };
        _settleTimer.Tick += (_, _) =>
        {
            UpdateOverlayPosition();

            _settleAttemptsRemaining--;

            if (_settleAttemptsRemaining <= 0)
            {
                _settleTimer.Stop();
            }
        };

        _eventMonitor = new WinEventMonitor(OnWinEvent);

        if (_enabled)
        {
            RefreshNow();
        }
    }

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (_enabled == value)
            {
                return;
            }

            _enabled = value;

            if (_enabled)
            {
                RefreshNow();
            }
            else
            {
                _moveTimer.Stop();
                _settleTimer.Stop();
                _settleAttemptsRemaining = 0;
                _overlay.HideOverlay();
                _targetWindow = IntPtr.Zero;
            }
        }
    }

    public void ApplySettings(AppSettings settings)
    {
        _overlay.ApplySettings(settings);
        Enabled = settings.ShowButton;

        if (Enabled)
        {
            RefreshNow();
            ScheduleSettledRefresh();
        }
    }

    public void RefreshNow()
    {
        if (!_enabled || _disposed)
        {
            return;
        }

        var foregroundWindow = NativeMethods.GetForegroundWindow();

        if (!WindowFiltering.IsEligible(foregroundWindow))
        {
            _targetWindow = IntPtr.Zero;
            _overlay.HideOverlay();
            return;
        }

        if (_targetWindow != foregroundWindow)
        {
            _targetWindow = foregroundWindow;
            _overlay.AttachTo(_targetWindow);
            ScheduleSettledRefresh();
        }

        UpdateOverlayPosition();
    }

    private ToggleResult ToggleTargetWindow()
    {
        var result = WindowPinning.ToggleWindow(_targetWindow);
        UpdateOverlayPosition();
        return result;
    }

    private void OnWinEvent(
        uint eventType,
        IntPtr window,
        int objectId)
    {
        if (_disposed || !_enabled)
        {
            return;
        }

        if (eventType == NativeMethods.EventSystemForeground)
        {
            PostRefresh();
            return;
        }

        if (window == IntPtr.Zero || window != _targetWindow)
        {
            return;
        }

        switch (eventType)
        {
            case NativeMethods.EventSystemMoveSizeStart:
                PostToUi(() =>
                {
                    if (!_disposed && _enabled)
                    {
                        _moveTimer.Start();
                        UpdateOverlayPosition();
                    }
                });
                break;

            case NativeMethods.EventSystemMoveSizeEnd:
                PostToUi(() =>
                {
                    _moveTimer.Stop();
                    UpdateOverlayPosition();
                    ScheduleSettledRefresh();
                });
                break;

            case NativeMethods.EventSystemMinimizeStart:
            case NativeMethods.EventObjectHide:
            case NativeMethods.EventObjectDestroy:
                PostToUi(() =>
                {
                    _moveTimer.Stop();
                    _settleTimer.Stop();
                    _settleAttemptsRemaining = 0;
                    _overlay.HideOverlay();
                });
                break;

            case NativeMethods.EventSystemMinimizeEnd:
            case NativeMethods.EventObjectShow:
                PostToUi(() =>
                {
                    RefreshNow();
                    ScheduleSettledRefresh();
                });
                break;

            case NativeMethods.EventObjectLocationChange:
                if (objectId == NativeMethods.ObjIdWindow)
                {
                    PostToUi(() =>
                    {
                        UpdateOverlayPosition();
                        ScheduleSettledRefresh(5);
                    });
                }
                break;
        }
    }

    private void ScheduleSettledRefresh(int attempts = 8)
    {
        if (_disposed || !_enabled)
        {
            return;
        }

        _settleAttemptsRemaining = Math.Max(
            _settleAttemptsRemaining,
            attempts);

        if (!_settleTimer.Enabled)
        {
            _settleTimer.Start();
        }
    }

    private void PostRefresh()
    {
        PostToUi(() =>
        {
            RefreshNow();
            ScheduleSettledRefresh();
        });
    }

    private void PostPositionRefresh()
    {
        if (Interlocked.Exchange(ref _refreshPosted, 1) == 1)
        {
            return;
        }

        PostToUi(() =>
        {
            Interlocked.Exchange(ref _refreshPosted, 0);
            UpdateOverlayPosition();
        });
    }

    private void PostToUi(Action action)
    {
        if (_disposed ||
            !_overlay.IsHandleCreated ||
            _overlay.IsDisposed)
        {
            return;
        }

        try
        {
            _overlay.BeginInvoke(action);
        }
        catch (InvalidOperationException)
        {
            // Приложение уже закрывается.
        }
    }

    private void UpdateOverlayPosition()
    {
        if (!_enabled || _disposed)
        {
            return;
        }

        if (_targetWindow == IntPtr.Zero ||
            !WindowFiltering.IsEligible(_targetWindow))
        {
            _overlay.HideOverlay();
            return;
        }

        _overlay.UpdateFromTarget();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _moveTimer.Stop();
        _moveTimer.Dispose();

        _settleTimer.Stop();
        _settleTimer.Dispose();

        _eventMonitor.Dispose();
        _overlay.Dispose();

        GC.SuppressFinalize(this);
    }
}

internal sealed class PinOverlayForm : Form
{
    private const int WmMouseActivate = 0x0021;
    private const int MaNoActivate = 3;

    private const int WsExLayered = 0x00080000;
    private const int WsExToolWindow = 0x00000080;
    private const int WsExNoActivate = 0x08000000;

    private readonly Func<ToggleResult> _toggleTarget;
    private readonly Action<ToggleResult> _showResult;
    private readonly ToolTip _toolTip;

    private IntPtr _targetWindow;
    private Rectangle _lastBounds;
    private bool _isPinned;
    private bool _isHovered;
    private bool _isDarkTitleBar;
    private OverlayVisualStyle _visualStyle = OverlayVisualStyle.Default;
    private AppSettings _settings;
    private Color _activeColor;

    public PinOverlayForm(
        Func<ToggleResult> toggleTarget,
        Action<ToggleResult> showResult,
        AppSettings settings)
    {
        _toggleTarget = toggleTarget;
        _showResult = showResult;
        _settings = settings.Clone();
        _activeColor = _settings.GetActiveColor();

        AutoScaleMode = AutoScaleMode.None;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        Cursor = Cursors.Hand;

        _toolTip = new ToolTip
        {
            InitialDelay = 350,
            ReshowDelay = 100,
            AutoPopDelay = 4500,
            ShowAlways = true
        };

        _ = Handle;
    }

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            var parameters = base.CreateParams;
            parameters.ExStyle |=
                WsExLayered |
                WsExToolWindow |
                WsExNoActivate;
            return parameters;
        }
    }

    public void ApplySettings(AppSettings settings)
    {
        _settings = settings.Clone();
        _activeColor = _settings.GetActiveColor();

        if (!_lastBounds.IsEmpty)
        {
            UpdateFromTarget();
        }
    }

    public void AttachTo(IntPtr targetWindow)
    {
        _targetWindow = targetWindow;
        _visualStyle = WindowProfiles.GetVisualStyle(targetWindow);

        if (targetWindow != IntPtr.Zero)
        {
            NativeMethods.SetWindowOwner(
                Handle,
                targetWindow);
        }

        UpdateFromTarget();
    }

    public void UpdateFromTarget()
    {
        if (_targetWindow == IntPtr.Zero ||
            !NativeMethods.IsWindow(_targetWindow))
        {
            HideOverlay();
            return;
        }

        if (!WindowOverlayGeometry.TryGetButtonBounds(
                _targetWindow,
                _visualStyle,
                _settings,
                out var bounds,
                out var isDarkTitleBar))
        {
            HideOverlay();
            return;
        }

        _lastBounds = bounds;
        _isDarkTitleBar = isDarkTitleBar;
        _isPinned = WindowPinning.IsTopMost(_targetWindow);

        // При максимизации Windows может пересобрать non-client area
        // и временно изменить порядок owned-окон.
        NativeMethods.SetWindowOwner(
            Handle,
            _targetWindow);

        _toolTip.SetToolTip(
            this,
            _isPinned
                ? "Открепить окно"
                : "Закрепить поверх остальных окон");

        RenderLayeredWindow(bounds);

        // Возвращаем overlay над окном-владельцем после максимизации,
        // не активируя саму кнопку и не забирая фокус.
        NativeMethods.SetWindowPos(
            Handle,
            NativeMethods.HwndTop,
            0,
            0,
            0,
            0,
            NativeMethods.SwpNoMove |
            NativeMethods.SwpNoSize |
            NativeMethods.SwpNoActivate |
            NativeMethods.SwpShowWindow |
            NativeMethods.SwpNoOwnerZOrder);

        if (!Visible)
        {
            NativeMethods.ShowWindow(
                Handle,
                NativeMethods.SwShowNoActivate);
        }
    }

    public void HideOverlay()
    {
        if (IsHandleCreated && Visible)
        {
            NativeMethods.ShowWindow(
                Handle,
                NativeMethods.SwHide);
        }
    }

    protected override void OnMouseEnter(EventArgs eventArgs)
    {
        _isHovered = true;

        if (!_lastBounds.IsEmpty)
        {
            RenderLayeredWindow(_lastBounds);
        }

        base.OnMouseEnter(eventArgs);
    }

    protected override void OnMouseLeave(EventArgs eventArgs)
    {
        _isHovered = false;

        if (!_lastBounds.IsEmpty)
        {
            RenderLayeredWindow(_lastBounds);
        }

        base.OnMouseLeave(eventArgs);
    }

    protected override void OnMouseUp(MouseEventArgs eventArgs)
    {
        if (eventArgs.Button == MouseButtons.Left)
        {
            var result = _toggleTarget();
            _isPinned = WindowPinning.IsTopMost(_targetWindow);
            _showResult(result);

            if (!_lastBounds.IsEmpty)
            {
                RenderLayeredWindow(_lastBounds);
            }
        }

        base.OnMouseUp(eventArgs);
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == WmMouseActivate)
        {
            message.Result = new IntPtr(MaNoActivate);
            return;
        }

        base.WndProc(ref message);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _toolTip.Dispose();
        }

        base.Dispose(disposing);
    }

    private void RenderLayeredWindow(Rectangle bounds)
    {
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            return;
        }

        using var bitmap = new Bitmap(
            bounds.Width,
            bounds.Height,
            PixelFormat.Format32bppArgb);

        using (var graphics = Graphics.FromImage(bitmap))
        {
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            DrawButton(graphics, bounds.Size);
        }

        UpdateLayeredBitmap(bitmap, bounds);
    }

    private void DrawButton(Graphics graphics, Size size)
    {
        if (_visualStyle == OverlayVisualStyle.Telegram)
        {
            DrawTelegramButton(graphics, size);
            return;
        }

        var scale = Math.Max(0.75f, size.Width / 28f);
        var centerX = size.Width / 2f;
        var centerY = size.Height / 2f;

        if (_isHovered)
        {
            var hoverSize = Math.Min(
                size.Width,
                size.Height) - 4f * scale;

            var hoverRectangle = new RectangleF(
                centerX - hoverSize / 2f,
                centerY - hoverSize / 2f,
                hoverSize,
                hoverSize);

            var hoverColor = _isDarkTitleBar
                ? Color.FromArgb(46, 255, 255, 255)
                : Color.FromArgb(34, 0, 0, 0);

            using var hoverBrush =
                new SolidBrush(hoverColor);

            graphics.FillEllipse(
                hoverBrush,
                hoverRectangle);
        }

        var iconColor = _isPinned
            ? _activeColor
            : _isDarkTitleBar
                ? Color.FromArgb(235, 245, 245, 247)
                : Color.FromArgb(220, 42, 42, 46);

        var shadowColor = _isDarkTitleBar
            ? Color.FromArgb(90, 0, 0, 0)
            : Color.FromArgb(70, 255, 255, 255);

        DrawPin(
            graphics,
            centerX + 0.6f * scale,
            centerY + 0.7f * scale,
            scale,
            shadowColor,
            filled: _isPinned,
            thicknessMultiplier: 1.65f);

        DrawPin(
            graphics,
            centerX,
            centerY,
            scale,
            iconColor,
            filled: _isPinned,
            thicknessMultiplier: 1f);
    }

    private void DrawTelegramButton(
        Graphics graphics,
        Size size)
    {
        var centerX = size.Width / 2f;
        var centerY = size.Height / 2f;
        var scale = Math.Max(
            0.68f,
            Math.Min(0.92f, size.Width / 35f));

        if (_isHovered)
        {
            var hoverRectangle = new RectangleF(
                1f,
                1f,
                size.Width - 2f,
                size.Height - 2f);

            using var hoverPath = CreateRoundedRectanglePath(
                hoverRectangle,
                Math.Max(3f, size.Width * 0.14f));

            using var hoverBrush = new SolidBrush(
                Color.FromArgb(28, 255, 255, 255));

            graphics.FillPath(
                hoverBrush,
                hoverPath);
        }

        var iconColor = _isPinned
            ? _activeColor
            : Color.FromArgb(165, 167, 184, 201);

        DrawCompactPin(
            graphics,
            centerX,
            centerY + 0.2f,
            scale,
            iconColor,
            _isPinned);
    }

    private static void DrawCompactPin(
        Graphics graphics,
        float centerX,
        float centerY,
        float scale,
        Color color,
        bool filled)
    {
        using var pen = new Pen(
            color,
            Math.Max(1.15f, 1.35f * scale))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        using var brush = new SolidBrush(color);

        var headWidth = 8.2f * scale;
        var headHeight = 3.6f * scale;

        var head = new RectangleF(
            centerX - headWidth / 2f,
            centerY - 6.5f * scale,
            headWidth,
            headHeight);

        if (filled)
        {
            graphics.FillRectangle(brush, head);
        }
        else
        {
            graphics.DrawRectangle(
                pen,
                head.X,
                head.Y,
                head.Width,
                head.Height);
        }

        var shoulderY = centerY - 2.3f * scale;
        var baseY = centerY + 2.4f * scale;

        graphics.DrawLine(
            pen,
            centerX - 3.2f * scale,
            shoulderY,
            centerX - 4.6f * scale,
            baseY);

        graphics.DrawLine(
            pen,
            centerX + 3.2f * scale,
            shoulderY,
            centerX + 4.6f * scale,
            baseY);

        graphics.DrawLine(
            pen,
            centerX - 4.6f * scale,
            baseY,
            centerX + 4.6f * scale,
            baseY);

        graphics.DrawLine(
            pen,
            centerX,
            baseY,
            centerX,
            centerY + 7.6f * scale);
    }

    private static GraphicsPath CreateRoundedRectanglePath(
        RectangleF rectangle,
        float radius)
    {
        var diameter = radius * 2f;
        var path = new GraphicsPath();

        path.AddArc(
            rectangle.Left,
            rectangle.Top,
            diameter,
            diameter,
            180,
            90);

        path.AddArc(
            rectangle.Right - diameter,
            rectangle.Top,
            diameter,
            diameter,
            270,
            90);

        path.AddArc(
            rectangle.Right - diameter,
            rectangle.Bottom - diameter,
            diameter,
            diameter,
            0,
            90);

        path.AddArc(
            rectangle.Left,
            rectangle.Bottom - diameter,
            diameter,
            diameter,
            90,
            90);

        path.CloseFigure();
        return path;
    }

    private static void DrawPin(
        Graphics graphics,
        float centerX,
        float centerY,
        float scale,
        Color color,
        bool filled,
        float thicknessMultiplier)
    {
        using var pen = new Pen(
            color,
            Math.Max(
                1.45f,
                1.65f * scale * thicknessMultiplier))
        {
            StartCap = LineCap.Round,
            EndCap = LineCap.Round,
            LineJoin = LineJoin.Round
        };

        using var brush = new SolidBrush(color);

        var headWidth = 9.5f * scale;
        var headHeight = 4.4f * scale;

        var head = new RectangleF(
            centerX - headWidth / 2f,
            centerY - 7.3f * scale,
            headWidth,
            headHeight);

        if (filled)
        {
            graphics.FillRectangle(brush, head);
        }
        else
        {
            graphics.DrawRectangle(
                pen,
                head.X,
                head.Y,
                head.Width,
                head.Height);
        }

        var shoulderY = centerY - 2.7f * scale;
        var baseY = centerY + 3.0f * scale;
        var leftShoulderX = centerX - 3.8f * scale;
        var rightShoulderX = centerX + 3.8f * scale;
        var leftBaseX = centerX - 5.5f * scale;
        var rightBaseX = centerX + 5.5f * scale;

        graphics.DrawLine(
            pen,
            leftShoulderX,
            shoulderY,
            leftBaseX,
            baseY);

        graphics.DrawLine(
            pen,
            rightShoulderX,
            shoulderY,
            rightBaseX,
            baseY);

        graphics.DrawLine(
            pen,
            leftBaseX,
            baseY,
            rightBaseX,
            baseY);

        graphics.DrawLine(
            pen,
            centerX,
            baseY,
            centerX,
            centerY + 9.3f * scale);
    }

    private void UpdateLayeredBitmap(
        Bitmap bitmap,
        Rectangle bounds)
    {
        var screenDeviceContext =
            NativeMethods.GetDC(IntPtr.Zero);

        var memoryDeviceContext =
            NativeMethods.CreateCompatibleDC(
                screenDeviceContext);

        IntPtr bitmapHandle = IntPtr.Zero;
        IntPtr oldBitmap = IntPtr.Zero;

        try
        {
            bitmapHandle = bitmap.GetHbitmap(
                Color.FromArgb(0));

            oldBitmap = NativeMethods.SelectObject(
                memoryDeviceContext,
                bitmapHandle);

            var destination = new NativeMethods.Point(
                bounds.Left,
                bounds.Top);

            var size = new NativeMethods.Size(
                bounds.Width,
                bounds.Height);

            var source = new NativeMethods.Point(0, 0);

            var blend = new NativeMethods.BlendFunction
            {
                BlendOp = NativeMethods.AcSrcOver,
                BlendFlags = 0,
                SourceConstantAlpha = 255,
                AlphaFormat = NativeMethods.AcSrcAlpha
            };

            var updated = NativeMethods.UpdateLayeredWindow(
                Handle,
                screenDeviceContext,
                ref destination,
                ref size,
                memoryDeviceContext,
                ref source,
                0,
                ref blend,
                NativeMethods.UlwAlpha);

            if (!updated)
            {
                throw new Win32Exception(
                    Marshal.GetLastWin32Error());
            }
        }
        finally
        {
            if (oldBitmap != IntPtr.Zero)
            {
                NativeMethods.SelectObject(
                    memoryDeviceContext,
                    oldBitmap);
            }

            if (bitmapHandle != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(
                    bitmapHandle);
            }

            NativeMethods.DeleteDC(
                memoryDeviceContext);

            NativeMethods.ReleaseDC(
                IntPtr.Zero,
                screenDeviceContext);
        }
    }
}

internal sealed class WinEventMonitor : IDisposable
{
    private readonly NativeMethods.WinEventDelegate _callback;
    private readonly Action<uint, IntPtr, int> _onEvent;
    private readonly List<IntPtr> _hooks = new();

    private bool _disposed;

    public WinEventMonitor(
        Action<uint, IntPtr, int> onEvent)
    {
        _onEvent = onEvent;
        _callback = HandleWinEvent;

        AddHook(
            NativeMethods.EventSystemForeground,
            NativeMethods.EventSystemForeground);

        AddHook(
            NativeMethods.EventSystemMoveSizeStart,
            NativeMethods.EventSystemMoveSizeEnd);

        AddHook(
            NativeMethods.EventSystemMinimizeStart,
            NativeMethods.EventSystemMinimizeEnd);

        AddHook(
            NativeMethods.EventObjectDestroy,
            NativeMethods.EventObjectLocationChange);
    }

    private void AddHook(
        uint minimumEvent,
        uint maximumEvent)
    {
        var hook = NativeMethods.SetWinEventHook(
            minimumEvent,
            maximumEvent,
            IntPtr.Zero,
            _callback,
            0,
            0,
            NativeMethods.WinEventOutOfContext |
            NativeMethods.WinEventSkipOwnProcess);

        if (hook == IntPtr.Zero)
        {
            throw new Win32Exception(
                Marshal.GetLastWin32Error(),
                "Не удалось подписаться на события окон Windows.");
        }

        _hooks.Add(hook);
    }

    private void HandleWinEvent(
        IntPtr hook,
        uint eventType,
        IntPtr window,
        int objectId,
        int childId,
        uint eventThread,
        uint eventTime)
    {
        if (_disposed)
        {
            return;
        }

        _onEvent(eventType, window, objectId);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        foreach (var hook in _hooks)
        {
            if (hook != IntPtr.Zero)
            {
                NativeMethods.UnhookWinEvent(hook);
            }
        }

        _hooks.Clear();
        GC.SuppressFinalize(this);
    }
}

internal enum OverlayVisualStyle
{
    Default,
    Telegram
}

internal static class WindowProfiles
{
    public static OverlayVisualStyle GetVisualStyle(
        IntPtr window)
    {
        if (window == IntPtr.Zero)
        {
            return OverlayVisualStyle.Default;
        }

        NativeMethods.GetWindowThreadProcessId(
            window,
            out var processId);

        if (processId == 0)
        {
            return OverlayVisualStyle.Default;
        }

        try
        {
            using var process =
                System.Diagnostics.Process.GetProcessById(
                    checked((int)processId));

            return process.ProcessName.Equals(
                "Telegram",
                StringComparison.OrdinalIgnoreCase)
                ? OverlayVisualStyle.Telegram
                : OverlayVisualStyle.Default;
        }
        catch
        {
            return OverlayVisualStyle.Default;
        }
    }
}

internal static class WindowOverlayGeometry
{
    private const int DwmwaCaptionButtonBounds = 5;
    private const int DwmwaUseImmersiveDarkMode = 20;

    public static bool TryGetButtonBounds(
        IntPtr window,
        OverlayVisualStyle visualStyle,
        AppSettings settings,
        out Rectangle bounds,
        out bool isDarkTitleBar)
    {
        bounds = Rectangle.Empty;
        isDarkTitleBar = GetDarkTitleBarState(window);

        if (!NativeMethods.IsWindowVisible(window) ||
            NativeMethods.IsIconic(window) ||
            !NativeMethods.GetWindowRect(
                window,
                out var windowRectangle))
        {
            return false;
        }

        var dpi = NativeMethods.GetDpiForWindow(window);
        if (dpi == 0)
        {
            dpi = 96;
        }

        var scale = dpi / 96f;
        var isTelegram =
            visualStyle == OverlayVisualStyle.Telegram;

        var logicalButtonSize = isTelegram
            ? Math.Max(20, settings.PinSize - 3)
            : settings.PinSize;

        var buttonSize = Math.Max(
            18,
            (int)Math.Round(logicalButtonSize * scale));

        var gap = isTelegram
            ? Math.Max(
                2,
                (int)Math.Round(2 * scale))
            : Math.Max(
                5,
                (int)Math.Round(7 * scale));

        var x = 0;
        var y = 0;
        var usedCaptionBounds = false;

        NativeMethods.Rect captionButtons;
        var captionResult =
            NativeMethods.DwmGetWindowAttributeRect(
                window,
                DwmwaCaptionButtonBounds,
                out captionButtons,
                Marshal.SizeOf<NativeMethods.Rect>());

        if (!isTelegram &&
            captionResult == 0 &&
            captionButtons.Right > captionButtons.Left &&
            captionButtons.Bottom > captionButtons.Top)
        {
            var captionWidth =
                captionButtons.Right - captionButtons.Left;

            var captionHeight =
                captionButtons.Bottom - captionButtons.Top;

            var windowWidth =
                windowRectangle.Right - windowRectangle.Left;

            if (captionButtons.Left >= 0 &&
                captionButtons.Right <=
                    windowWidth + (int)(24 * scale) &&
                captionWidth >= (int)(60 * scale) &&
                captionHeight >= (int)(20 * scale))
            {
                x =
                    windowRectangle.Left +
                    captionButtons.Left -
                    buttonSize -
                    gap;

                y =
                    windowRectangle.Top +
                    captionButtons.Top +
                    Math.Max(
                        0,
                        (captionHeight - buttonSize) / 2);

                usedCaptionBounds = true;
            }
        }

        if (!usedCaptionBounds)
        {
            // Telegram Desktop сам рисует заголовок. Его блок системных
            // кнопок заметно уже стандартного DWM-блока, поэтому для него
            // используется отдельная базовая ширина.
            var systemButtonsWidth = isTelegram
                ? (int)Math.Round(108 * scale)
                : (int)Math.Round(138 * scale);

            x =
                windowRectangle.Right -
                systemButtonsWidth -
                buttonSize -
                gap;

            y =
                windowRectangle.Top +
                Math.Max(
                    1,
                    (int)Math.Round(
                        (isTelegram ? 4 : 3) * scale));
        }

        x += (int)Math.Round(settings.OffsetX * scale);
        y += (int)Math.Round(settings.OffsetY * scale);

        var windowHeight =
            windowRectangle.Bottom -
            windowRectangle.Top;

        if (x < windowRectangle.Left ||
            x + buttonSize > windowRectangle.Right ||
            y < windowRectangle.Top ||
            y + buttonSize > windowRectangle.Bottom ||
            windowHeight < buttonSize + 8)
        {
            return false;
        }

        bounds = new Rectangle(
            x,
            y,
            buttonSize,
            buttonSize);

        return true;
    }

    private static bool GetDarkTitleBarState(
        IntPtr window)
    {
        int darkMode;

        var result =
            NativeMethods.DwmGetWindowAttributeInt(
                window,
                DwmwaUseImmersiveDarkMode,
                out darkMode,
                sizeof(int));

        if (result == 0)
        {
            return darkMode != 0;
        }

        return IsSystemDarkMode();
    }

    private static bool IsSystemDarkMode()
    {
        try
        {
            using var key =
                Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

            var value = key?.GetValue(
                "AppsUseLightTheme");

            return value is int lightTheme &&
                   lightTheme == 0;
        }
        catch
        {
            return false;
        }
    }
}

internal static class WindowFiltering
{
    private const int GwlStyle = -16;
    private const int GwlExStyle = -20;

    private const long WsCaption = 0x00C00000L;
    private const long WsExToolWindow = 0x00000080L;

    private const int DwmwaCloaked = 14;

    private static readonly uint CurrentProcessId =
        (uint)Environment.ProcessId;

    private static readonly HashSet<string> IgnoredClasses =
        new(StringComparer.Ordinal)
        {
            "Progman",
            "WorkerW",
            "Shell_TrayWnd",
            "Shell_SecondaryTrayWnd",
            "NotifyIconOverflowWindow",
            "DV2ControlHost",
            "MultitaskingViewFrame"
        };

    public static bool IsEligible(
        IntPtr window)
    {
        if (window == IntPtr.Zero ||
            !NativeMethods.IsWindow(window) ||
            !NativeMethods.IsWindowVisible(window) ||
            NativeMethods.IsIconic(window))
        {
            return false;
        }

        NativeMethods.GetWindowThreadProcessId(
            window,
            out var processId);

        if (processId == CurrentProcessId)
        {
            return false;
        }

        if (IsCloaked(window))
        {
            return false;
        }

        var className =
            WindowText.GetClassName(window);

        if (IgnoredClasses.Contains(className))
        {
            return false;
        }

        var style =
            NativeMethods.GetWindowLongPtr(
                window,
                GwlStyle).ToInt64();

        if ((style & WsCaption) != WsCaption)
        {
            return false;
        }

        var extendedStyle =
            NativeMethods.GetWindowLongPtr(
                window,
                GwlExStyle).ToInt64();

        if ((extendedStyle & WsExToolWindow) != 0)
        {
            return false;
        }

        if (!NativeMethods.GetWindowRect(
                window,
                out var rectangle))
        {
            return false;
        }

        var width =
            rectangle.Right - rectangle.Left;

        var height =
            rectangle.Bottom - rectangle.Top;

        return width >= 240 && height >= 80;
    }

    private static bool IsCloaked(
        IntPtr window)
    {
        int cloaked;

        var result =
            NativeMethods.DwmGetWindowAttributeInt(
                window,
                DwmwaCloaked,
                out cloaked,
                sizeof(int));

        return result == 0 && cloaked != 0;
    }
}

internal sealed class HotkeyWindow :
    NativeWindow,
    IDisposable
{
    private const int HotkeyId = 1;
    private const int WmHotkey = 0x0312;
    private const uint ModNoRepeat = 0x4000;

    private readonly Action _onHotkey;
    private HotkeyDefinition _definition;
    private bool _disposed;

    public HotkeyWindow(
        Action onHotkey,
        HotkeyDefinition definition)
    {
        _onHotkey = onHotkey;
        CreateHandle(new CreateParams());

        RegisterOrThrow(definition);
        _definition = definition;
    }

    public void UpdateHotkey(HotkeyDefinition definition)
    {
        if (_definition == definition)
        {
            return;
        }

        var previous = _definition;
        NativeMethods.UnregisterHotKey(Handle, HotkeyId);

        try
        {
            RegisterOrThrow(definition);
            _definition = definition;
        }
        catch
        {
            try
            {
                RegisterOrThrow(previous);
                _definition = previous;
            }
            catch
            {
                // Первоначальная ошибка важнее ошибки восстановления.
            }

            throw;
        }
    }

    private void RegisterOrThrow(HotkeyDefinition definition)
    {
        var registered = NativeMethods.RegisterHotKey(
            Handle,
            HotkeyId,
            definition.NativeModifiers | ModNoRepeat,
            (uint)definition.Key);

        if (registered)
        {
            return;
        }

        var error = new Win32Exception(
            Marshal.GetLastWin32Error());

        throw new InvalidOperationException(
            $"Не удалось зарегистрировать {definition.DisplayText}. " +
            "Возможно, это сочетание уже занято другой программой.\n\n" +
            error.Message,
            error);
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == WmHotkey &&
            message.WParam.ToInt32() == HotkeyId)
        {
            _onHotkey();
        }

        base.WndProc(ref message);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        NativeMethods.UnregisterHotKey(Handle, HotkeyId);
        DestroyHandle();
        GC.SuppressFinalize(this);
    }
}

internal static class WindowPinning
{
    private static readonly IntPtr HwndTopmost =
        new(-1);

    private static readonly IntPtr HwndNotTopmost =
        new(-2);

    private const int GwlExStyle = -20;
    private const long WsExTopmost = 0x00000008L;

    public static ToggleResult ToggleWindow(
        IntPtr window)
    {
        if (window == IntPtr.Zero ||
            !NativeMethods.IsWindow(window))
        {
            return ToggleResult.Error(
                "Окно не найдено.");
        }

        var className =
            WindowText.GetClassName(window);

        if (className is
            "Progman" or
            "WorkerW" or
            "Shell_TrayWnd" or
            "Shell_SecondaryTrayWnd")
        {
            return ToggleResult.Error(
                "Рабочий стол и панель задач закреплять нельзя.");
        }

        var wasTopMost = IsTopMost(window);

        var newPosition = wasTopMost
            ? HwndNotTopmost
            : HwndTopmost;

        var changed =
            NativeMethods.SetWindowPos(
                window,
                newPosition,
                0,
                0,
                0,
                0,
                NativeMethods.SwpNoMove |
                NativeMethods.SwpNoSize |
                NativeMethods.SwpNoActivate);

        if (!changed)
        {
            var error = new Win32Exception(
                Marshal.GetLastWin32Error());

            return ToggleResult.Error(
                $"Windows не разрешила изменить окно: {error.Message}");
        }

        var title =
            WindowText.GetWindowTitle(window);

        var action = wasTopMost
            ? "Откреплено"
            : "Закреплено поверх окон";

        return ToggleResult.Ok(
            $"{action}: {title}");
    }

    public static bool IsTopMost(
        IntPtr window)
    {
        if (window == IntPtr.Zero ||
            !NativeMethods.IsWindow(window))
        {
            return false;
        }

        var extendedStyle =
            NativeMethods.GetWindowLongPtr(
                window,
                GwlExStyle).ToInt64();

        return (extendedStyle & WsExTopmost) != 0;
    }
}

internal static class WindowText
{
    public static string GetWindowTitle(
        IntPtr window)
    {
        var length =
            NativeMethods.GetWindowTextLength(
                window);

        if (length <= 0)
        {
            var className =
                GetClassName(window);

            return string.IsNullOrWhiteSpace(
                className)
                ? "окно без названия"
                : className;
        }

        var buffer =
            new StringBuilder(length + 1);

        NativeMethods.GetWindowText(
            window,
            buffer,
            buffer.Capacity);

        return buffer.ToString();
    }

    public static string GetClassName(
        IntPtr window)
    {
        var buffer = new StringBuilder(256);

        NativeMethods.GetClassName(
            window,
            buffer,
            buffer.Capacity);

        return buffer.ToString();
    }
}

internal readonly record struct ToggleResult(
    bool Success,
    string Message)
{
    public static ToggleResult Ok(
        string message) =>
        new(true, message);

    public static ToggleResult Error(
        string message) =>
        new(false, message);
}

internal static class NativeMethods
{
    internal static readonly IntPtr HwndTop = IntPtr.Zero;

    internal const int SwHide = 0;
    internal const int SwShowNoActivate = 4;

    internal const uint SwpNoSize = 0x0001;
    internal const uint SwpNoMove = 0x0002;
    internal const uint SwpNoActivate = 0x0010;
    internal const uint SwpShowWindow = 0x0040;
    internal const uint SwpNoOwnerZOrder = 0x0200;

    internal const uint EventSystemForeground = 0x0003;
    internal const uint EventSystemMoveSizeStart = 0x000A;
    internal const uint EventSystemMoveSizeEnd = 0x000B;
    internal const uint EventSystemMinimizeStart = 0x0016;
    internal const uint EventSystemMinimizeEnd = 0x0017;

    internal const uint EventObjectDestroy = 0x8001;
    internal const uint EventObjectShow = 0x8002;
    internal const uint EventObjectHide = 0x8003;
    internal const uint EventObjectLocationChange = 0x800B;

    internal const int ObjIdWindow = 0;

    internal const uint WinEventOutOfContext = 0x0000;
    internal const uint WinEventSkipOwnProcess = 0x0002;

    internal const byte AcSrcOver = 0x00;
    internal const byte AcSrcAlpha = 0x01;
    internal const uint UlwAlpha = 0x00000002;

    private const int GwlHwndParent = -8;

    internal delegate void WinEventDelegate(
        IntPtr hook,
        uint eventType,
        IntPtr window,
        int objectId,
        int childId,
        uint eventThread,
        uint eventTime);

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Size
    {
        public int Width;
        public int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct BlendFunction
    {
        public byte BlendOp;
        public byte BlendFlags;
        public byte SourceConstantAlpha;
        public byte AlphaFormat;
    }

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(
        IntPtr window,
        int id,
        uint modifiers,
        uint virtualKey);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(
        IntPtr window,
        int id);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindow(
        IntPtr window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsWindowVisible(
        IntPtr window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool IsIconic(
        IntPtr window);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShowWindow(
        IntPtr window,
        int command);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowRect(
        IntPtr window,
        out Rect rectangle);

    [DllImport("user32.dll")]
    internal static extern uint GetDpiForWindow(
        IntPtr window);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
        IntPtr window,
        IntPtr insertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode)]
    internal static extern int GetWindowText(
        IntPtr window,
        StringBuilder text,
        int maximumCount);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode)]
    internal static extern int GetWindowTextLength(
        IntPtr window);

    [DllImport(
        "user32.dll",
        CharSet = CharSet.Unicode)]
    internal static extern int GetClassName(
        IntPtr window,
        StringBuilder className,
        int maximumCount);

    [DllImport("user32.dll")]
    internal static extern uint GetWindowThreadProcessId(
        IntPtr window,
        out uint processId);

    [DllImport(
        "dwmapi.dll",
        EntryPoint = "DwmGetWindowAttribute")]
    internal static extern int DwmGetWindowAttributeRect(
        IntPtr window,
        int attribute,
        out Rect value,
        int valueSize);

    [DllImport(
        "dwmapi.dll",
        EntryPoint = "DwmGetWindowAttribute")]
    internal static extern int DwmGetWindowAttributeInt(
        IntPtr window,
        int attribute,
        out int value,
        int valueSize);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    internal static extern IntPtr SetWinEventHook(
        uint eventMinimum,
        uint eventMaximum,
        IntPtr module,
        WinEventDelegate callback,
        uint processId,
        uint threadId,
        uint flags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnhookWinEvent(
        IntPtr hook);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetDC(
        IntPtr window);

    [DllImport("user32.dll")]
    internal static extern int ReleaseDC(
        IntPtr window,
        IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    internal static extern IntPtr CreateCompatibleDC(
        IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteDC(
        IntPtr deviceContext);

    [DllImport("gdi32.dll")]
    internal static extern IntPtr SelectObject(
        IntPtr deviceContext,
        IntPtr graphicsObject);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool DeleteObject(
        IntPtr graphicsObject);

    [DllImport(
        "user32.dll",
        SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UpdateLayeredWindow(
        IntPtr window,
        IntPtr destinationDeviceContext,
        ref Point destinationPoint,
        ref Size windowSize,
        IntPtr sourceDeviceContext,
        ref Point sourcePoint,
        int colorKey,
        ref BlendFunction blendFunction,
        uint flags);

    internal static IntPtr GetWindowLongPtr(
        IntPtr window,
        int index)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(window, index)
            : new IntPtr(
                GetWindowLong32(window, index));
    }

    internal static IntPtr SetWindowOwner(
        IntPtr window,
        IntPtr owner)
    {
        return SetWindowLongPtr(
            window,
            GwlHwndParent,
            owner);
    }

    private static IntPtr SetWindowLongPtr(
        IntPtr window,
        int index,
        IntPtr newValue)
    {
        return IntPtr.Size == 8
            ? SetWindowLongPtr64(
                window,
                index,
                newValue)
            : new IntPtr(
                SetWindowLong32(
                    window,
                    index,
                    newValue.ToInt32()));
    }

    [DllImport(
        "user32.dll",
        EntryPoint = "GetWindowLongPtrW",
        SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(
        IntPtr window,
        int index);

    [DllImport(
        "user32.dll",
        EntryPoint = "GetWindowLongW",
        SetLastError = true)]
    private static extern int GetWindowLong32(
        IntPtr window,
        int index);

    [DllImport(
        "user32.dll",
        EntryPoint = "SetWindowLongPtrW",
        SetLastError = true)]
    private static extern IntPtr SetWindowLongPtr64(
        IntPtr window,
        int index,
        IntPtr newValue);

    [DllImport(
        "user32.dll",
        EntryPoint = "SetWindowLongW",
        SetLastError = true)]
    private static extern int SetWindowLong32(
        IntPtr window,
        int index,
        int newValue);
}
