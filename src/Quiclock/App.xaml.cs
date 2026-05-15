using Quiclock.Models;
using Quiclock.Services;
using Quiclock.Views;
using WinForms = System.Windows.Forms;

namespace Quiclock;

public partial class App : System.Windows.Application
{
    private AppSettings _settings = null!;
    private SettingsService _settingsService = null!;
    private StartupService _startupService = null!;
    private HotkeyService _hotkeyService = null!;
    private TimerService _timerService = null!;
    private NotificationService _notificationService = null!;
    private WinForms.NotifyIcon _notifyIcon = null!;
    private TimerInputWindow? _timerInputWindow;
    private ActiveTimersWindow? _activeTimersWindow;
    private SettingsWindow? _settingsWindow;

    protected override void OnStartup(System.Windows.StartupEventArgs e)
    {
        base.OnStartup(e);
        ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

        _settingsService = new SettingsService();
        _settings = _settingsService.Load();
        _startupService = new StartupService();
        _timerService = new TimerService();
        _hotkeyService = new HotkeyService();
        _hotkeyService.HotkeyPressed += (_, _) => ShowTimerInput();

        _notifyIcon = BuildNotifyIcon();
        _notificationService = new NotificationService(_notifyIcon);
        _timerService.TimerCompleted += (_, args) => _notificationService.NotifyTimerCompleted(args.Timer, _settings.SoundEnabled);

        ApplySettings(_settings, isInitialLoad: true);
    }

    protected override void OnExit(System.Windows.ExitEventArgs e)
    {
        _timerInputWindow?.PrepareForExit();
        _activeTimersWindow?.PrepareForExit();
        _settingsWindow?.PrepareForExit();
        _timerService.Dispose();
        _hotkeyService.Dispose();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        base.OnExit(e);
    }

    private WinForms.NotifyIcon BuildNotifyIcon()
    {
        var trayMenu = new WinForms.ContextMenuStrip();
        trayMenu.Items.Add("Open timer input", null, (_, _) => ShowTimerInput());
        trayMenu.Items.Add("View active timers", null, (_, _) => ShowActiveTimers());
        trayMenu.Items.Add("Settings", null, (_, _) => ShowSettings());
        trayMenu.Items.Add("Quit", null, (_, _) => Shutdown());

        return new WinForms.NotifyIcon
        {
            Text = "Quiclock",
            Visible = true,
            ContextMenuStrip = trayMenu,
            Icon = System.Drawing.SystemIcons.Information,
        };
    }

    private void ShowTimerInput()
    {
        _timerInputWindow ??= new TimerInputWindow(_timerService);
        ShowWindow(_timerInputWindow);
    }

    private void ShowActiveTimers()
    {
        _activeTimersWindow ??= new ActiveTimersWindow(_timerService);
        ShowWindow(_activeTimersWindow);
    }

    private void ShowSettings()
    {
        if (_settingsWindow is null)
        {
            _settingsWindow = new SettingsWindow(_settings, settings => ApplySettings(settings, isInitialLoad: false));
        }

        ShowWindow(_settingsWindow);
    }

    private void ShowWindow(System.Windows.Window window)
    {
        if (!window.IsVisible)
        {
            window.Show();
        }

        if (window.WindowState == System.Windows.WindowState.Minimized)
        {
            window.WindowState = System.Windows.WindowState.Normal;
        }

        window.Activate();
        if (!window.Topmost)
        {
            window.Topmost = true;
            window.Topmost = false;
        }
        window.Focus();
    }

    private void ApplySettings(AppSettings settings, bool isInitialLoad)
    {
        if (!_hotkeyService.Register(settings.Hotkey))
        {
            throw new InvalidOperationException($"Could not register global shortcut {settings.Hotkey}. It may already be in use.");
        }

        _startupService.SetEnabled(settings.LaunchAtLogin);
        _settings = settings;
        _settingsService.Save(_settings);

        if (!isInitialLoad)
        {
            _settingsWindow = new SettingsWindow(_settings, updated => ApplySettings(updated, isInitialLoad: false));
        }
    }
}
