using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Interop;
using Quiclock.Models;

namespace Quiclock.Services;

public sealed class HotkeyService : IDisposable
{
    private const int WmHotkey = 0x0312;
    private const int HotkeyId = 0x5143;

    private readonly HwndSource _source;
    private HotkeyBinding? _currentBinding;

    public event EventHandler? HotkeyPressed;

    public HotkeyService()
    {
        var parameters = new HwndSourceParameters("QuiclockHotkeySink")
        {
            Width = 0,
            Height = 0,
            WindowStyle = 0x800000,
        };

        _source = new HwndSource(parameters);
        _source.AddHook(WndProc);
    }

    public bool Register(HotkeyBinding binding)
    {
        var previousBinding = _currentBinding;
        Unregister();

        var modifiers = (uint)binding.Modifiers;
        var virtualKey = (uint)KeyInterop.VirtualKeyFromKey(binding.Key);

        if (!RegisterHotKey(_source.Handle, HotkeyId, modifiers, virtualKey))
        {
            if (previousBinding is not null)
            {
                Register(previousBinding);
            }
            return false;
        }

        _currentBinding = binding;
        return true;
    }

    public void Unregister()
    {
        if (_currentBinding is null)
        {
            return;
        }

        UnregisterHotKey(_source.Handle, HotkeyId);
        _currentBinding = null;
    }

    public void Dispose()
    {
        Unregister();
        _source.RemoveHook(WndProc);
        _source.Dispose();
    }

    private IntPtr WndProc(IntPtr hwnd, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message == WmHotkey && wParam.ToInt32() == HotkeyId)
        {
            HotkeyPressed?.Invoke(this, EventArgs.Empty);
            handled = true;
        }

        return IntPtr.Zero;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
