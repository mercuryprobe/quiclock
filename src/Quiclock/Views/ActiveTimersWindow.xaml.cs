using System.Windows;
using Quiclock.Services;

namespace Quiclock.Views;

public partial class ActiveTimersWindow : Window
{
    private readonly TimerService _timerService;
    private bool _allowClose;

    public ActiveTimersWindow(TimerService timerService)
    {
        InitializeComponent();
        _timerService = timerService;
        TimersGrid.ItemsSource = _timerService.Timers;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: Guid timerId })
        {
            return;
        }

        _timerService.CancelTimer(timerId);
    }

    public void PrepareForExit()
    {
        _allowClose = true;
        Close();
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (!_allowClose)
        {
            e.Cancel = true;
            Hide();
            return;
        }

        base.OnClosing(e);
    }
}
