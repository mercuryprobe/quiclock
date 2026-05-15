using System.Windows;
using Quiclock.Models;

namespace Quiclock.Views;

public partial class SettingsWindow : Window
{
    private readonly Action<AppSettings> _saveAction;
    private bool _allowClose;

    public SettingsWindow(AppSettings settings, Action<AppSettings> saveAction)
    {
        InitializeComponent();
        _saveAction = saveAction;
        HotkeyTextBox.Text = settings.Hotkey.ToString();
        LaunchAtLoginCheckBox.IsChecked = settings.LaunchAtLogin;
        SoundEnabledCheckBox.IsChecked = settings.SoundEnabled;
    }

    private void SaveButton_OnClick(object sender, RoutedEventArgs e)
    {
        if (!HotkeyBinding.TryParse(HotkeyTextBox.Text, out var binding, out var error))
        {
            System.Windows.MessageBox.Show(
                this,
                error,
                "Invalid shortcut",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        var settings = new AppSettings
        {
            Hotkey = binding!,
            LaunchAtLogin = LaunchAtLoginCheckBox.IsChecked == true,
            SoundEnabled = SoundEnabledCheckBox.IsChecked == true,
        };

        try
        {
            _saveAction(settings);
            Hide();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                this,
                ex.Message,
                "Unable to save settings",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Error);
        }
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
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
