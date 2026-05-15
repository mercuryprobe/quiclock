namespace Quiclock.Models;

public sealed class AppSettings
{
    public HotkeyBinding Hotkey { get; set; } = new(HotkeyModifiers.Control | HotkeyModifiers.Alt, System.Windows.Input.Key.T);

    public bool LaunchAtLogin { get; set; } = true;

    public bool SoundEnabled { get; set; } = true;
}
