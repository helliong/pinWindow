using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PinWindow;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            Application.Run(new TrayApplicationContext());
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
}

internal sealed class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly HotkeyWindow _hotkeyWindow;

    public TrayApplicationContext()
    {
        var menu = new ContextMenuStrip();
        menu.Items.Add("Горячая клавиша: Ctrl + Alt + T").Enabled = false;
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Выход", null, (_, _) => ExitThread());

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            Text = "PinWindow — закрепление окон",
            ContextMenuStrip = menu,
            Visible = true
        };

        _hotkeyWindow = new HotkeyWindow(ToggleActiveWindow);

        _notifyIcon.ShowBalloonTip(
            2500,
            "PinWindow запущен",
            "Нажмите Ctrl + Alt + T, чтобы закрепить или открепить активное окно.",
            ToolTipIcon.Info);
    }

    private void ToggleActiveWindow()
    {
        var result = WindowPinning.ToggleForegroundWindow();

        _notifyIcon.ShowBalloonTip(
            1800,
            result.Success ? "PinWindow" : "Не удалось изменить окно",
            result.Message,
            result.Success ? ToolTipIcon.Info : ToolTipIcon.Error);
    }

    protected override void ExitThreadCore()
    {
        _hotkeyWindow.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        base.ExitThreadCore();
    }
}

internal sealed class HotkeyWindow : NativeWindow, IDisposable
{
    private const int HotkeyId = 1;
    private const int WmHotkey = 0x0312;

    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;
    private const uint ModNoRepeat = 0x4000;
    private const uint VkT = 0x54;

    private readonly Action _onHotkey;
    private bool _disposed;

    public HotkeyWindow(Action onHotkey)
    {
        _onHotkey = onHotkey;
        CreateHandle(new CreateParams());

        var registered = NativeMethods.RegisterHotKey(
            Handle,
            HotkeyId,
            ModAlt | ModControl | ModNoRepeat,
            VkT);

        if (!registered)
        {
            var error = new Win32Exception(Marshal.GetLastWin32Error());
            DestroyHandle();
            throw new InvalidOperationException(
                "Не удалось зарегистрировать Ctrl + Alt + T. " +
                "Возможно, это сочетание уже занято другой программой.\n\n" +
                error.Message,
                error);
        }
    }

    protected override void WndProc(ref Message message)
    {
        if (message.Msg == WmHotkey && message.WParam.ToInt32() == HotkeyId)
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
    private static readonly IntPtr HwndTopmost = new(-1);
    private static readonly IntPtr HwndNotTopmost = new(-2);

    private const int GwlExStyle = -20;
    private const long WsExTopmost = 0x00000008L;

    private const uint SwpNoSize = 0x0001;
    private const uint SwpNoMove = 0x0002;
    private const uint SwpNoActivate = 0x0010;

    public static ToggleResult ToggleForegroundWindow()
    {
        var window = NativeMethods.GetForegroundWindow();

        if (window == IntPtr.Zero)
        {
            return ToggleResult.Error("Активное окно не найдено.");
        }

        var className = GetClassName(window);
        if (className is "Progman" or "WorkerW" or "Shell_TrayWnd")
        {
            return ToggleResult.Error("Рабочий стол и панель задач закреплять нельзя.");
        }

        var extendedStyle = NativeMethods.GetWindowLongPtr(window, GwlExStyle).ToInt64();
        var isCurrentlyTopmost = (extendedStyle & WsExTopmost) != 0;
        var newPosition = isCurrentlyTopmost ? HwndNotTopmost : HwndTopmost;

        var changed = NativeMethods.SetWindowPos(
            window,
            newPosition,
            0,
            0,
            0,
            0,
            SwpNoMove | SwpNoSize | SwpNoActivate);

        if (!changed)
        {
            var error = new Win32Exception(Marshal.GetLastWin32Error());
            return ToggleResult.Error($"Windows не разрешила изменить окно: {error.Message}");
        }

        var title = GetWindowTitle(window);
        var action = isCurrentlyTopmost ? "Откреплено" : "Закреплено поверх окон";

        return ToggleResult.Ok($"{action}: {title}");
    }

    private static string GetWindowTitle(IntPtr window)
    {
        var length = NativeMethods.GetWindowTextLength(window);
        if (length <= 0)
        {
            var className = GetClassName(window);
            return string.IsNullOrWhiteSpace(className) ? "окно без названия" : className;
        }

        var buffer = new StringBuilder(length + 1);
        NativeMethods.GetWindowText(window, buffer, buffer.Capacity);
        return buffer.ToString();
    }

    private static string GetClassName(IntPtr window)
    {
        var buffer = new StringBuilder(256);
        NativeMethods.GetClassName(window, buffer, buffer.Capacity);
        return buffer.ToString();
    }
}

internal readonly record struct ToggleResult(bool Success, string Message)
{
    public static ToggleResult Ok(string message) => new(true, message);
    public static ToggleResult Error(string message) => new(false, message);
}

internal static class NativeMethods
{
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        uint fsModifiers,
        uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int width,
        int height,
        uint flags);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxCount);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern int GetClassName(IntPtr hWnd, StringBuilder className, int maxCount);

    internal static IntPtr GetWindowLongPtr(IntPtr hWnd, int index)
    {
        return IntPtr.Size == 8
            ? GetWindowLongPtr64(hWnd, index)
            : new IntPtr(GetWindowLong32(hWnd, index));
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int index);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
    private static extern int GetWindowLong32(IntPtr hWnd, int index);
}
