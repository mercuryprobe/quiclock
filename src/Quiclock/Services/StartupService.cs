using Microsoft.Win32;

namespace Quiclock.Services;

public sealed class StartupService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Quiclock";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
        return key?.GetValue(AppName) is string;
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.CreateSubKey(RunKeyPath, true);
        if (enabled)
        {
            var exePath = Environment.ProcessPath ?? throw new InvalidOperationException("Unable to determine executable path.");
            key.SetValue(AppName, $"\"{exePath}\"");
            return;
        }

        key.DeleteValue(AppName, false);
    }
}
