using System.Media;
using CommunityToolkit.WinUI.Notifications;
using Quiclock.Models;
using WinForms = System.Windows.Forms;

namespace Quiclock.Services;

public sealed class NotificationService
{
    private readonly WinForms.NotifyIcon _notifyIcon;

    public NotificationService(WinForms.NotifyIcon notifyIcon)
    {
        _notifyIcon = notifyIcon;
    }

    public void NotifyTimerCompleted(TimerEntry entry, bool soundEnabled)
    {
        var title = "Timer complete";
        var message = $"{entry.OriginalInput} finished.";

        try
        {
            new ToastContentBuilder()
                .AddText(title)
                .AddText(message)
                .Show();
        }
        catch
        {
            _notifyIcon.ShowBalloonTip(5000, title, message, WinForms.ToolTipIcon.Info);
        }

        if (soundEnabled)
        {
            SystemSounds.Asterisk.Play();
        }
    }
}
