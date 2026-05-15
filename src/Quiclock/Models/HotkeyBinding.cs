using System.Globalization;
using System.Windows.Input;

namespace Quiclock.Models;

[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Windows = 8,
}

public sealed class HotkeyBinding
{
    public HotkeyBinding()
    {
    }

    public HotkeyBinding(HotkeyModifiers modifiers, Key key)
    {
        Modifiers = modifiers;
        Key = key;
    }

    public HotkeyModifiers Modifiers { get; set; }

    public Key Key { get; set; }

    public override string ToString()
    {
        var parts = new List<string>();

        if (Modifiers.HasFlag(HotkeyModifiers.Control))
        {
            parts.Add("Ctrl");
        }

        if (Modifiers.HasFlag(HotkeyModifiers.Alt))
        {
            parts.Add("Alt");
        }

        if (Modifiers.HasFlag(HotkeyModifiers.Shift))
        {
            parts.Add("Shift");
        }

        if (Modifiers.HasFlag(HotkeyModifiers.Windows))
        {
            parts.Add("Win");
        }

        parts.Add(Key.ToString().ToUpper(CultureInfo.InvariantCulture));
        return string.Join("+", parts);
    }

    public static bool TryParse(string? text, out HotkeyBinding? binding, out string error)
    {
        binding = null;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(text))
        {
            error = "Shortcut is required.";
            return false;
        }

        var tokens = text.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (tokens.Length < 2)
        {
            error = "Include at least one modifier and one key, for example Ctrl+Alt+T.";
            return false;
        }

        var modifiers = HotkeyModifiers.None;
        Key? key = null;

        foreach (var token in tokens)
        {
            switch (token.ToLowerInvariant())
            {
                case "ctrl":
                case "control":
                    modifiers |= HotkeyModifiers.Control;
                    continue;
                case "alt":
                    modifiers |= HotkeyModifiers.Alt;
                    continue;
                case "shift":
                    modifiers |= HotkeyModifiers.Shift;
                    continue;
                case "win":
                case "windows":
                    modifiers |= HotkeyModifiers.Windows;
                    continue;
            }

            if (key is not null)
            {
                error = "Only one non-modifier key is allowed.";
                return false;
            }

            var converter = new KeyConverter();
            try
            {
                var converted = converter.ConvertFromString(token);
                if (converted is Key parsedKey && parsedKey != Key.None)
                {
                    key = parsedKey;
                    continue;
                }
            }
            catch
            {
            }

            error = $"Unsupported key token '{token}'.";
            return false;
        }

        if (modifiers == HotkeyModifiers.None)
        {
            error = "At least one modifier is required.";
            return false;
        }

        if (key is null)
        {
            error = "A final key is required.";
            return false;
        }

        binding = new HotkeyBinding(modifiers, key.Value);
        return true;
    }
}
