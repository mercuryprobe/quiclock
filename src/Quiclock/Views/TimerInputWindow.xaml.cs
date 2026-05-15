using System.Windows;
using System.Windows.Input;
using Quiclock.Services;

namespace Quiclock.Views;

public partial class TimerInputWindow : Window
{
    private readonly TimerService _timerService;
    private bool _allowClose;

    public TimerInputWindow(TimerService timerService)
    {
        InitializeComponent();
        _timerService = timerService;
    }

    protected override void OnActivated(EventArgs e)
    {
        base.OnActivated(e);
        DurationTextBox.Focus();
        DurationTextBox.SelectAll();
    }

    private void StartButton_OnClick(object sender, RoutedEventArgs e)
    {
        Submit();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        Hide();
    }

    private void DurationTextBox_OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Submit();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            Hide();
            e.Handled = true;
        }
    }

    private void Submit()
    {
        if (!DurationParser.TryParse(DurationTextBox.Text, out var duration, out var error))
        {
            ErrorTextBlock.Text = error;
            return;
        }

        _timerService.StartTimer(DurationTextBox.Text, duration);
        ErrorTextBlock.Text = string.Empty;
        DurationTextBox.Clear();
        Hide();
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
