using System.Globalization;

namespace Quiclock.Services;

public static class DurationParser
{
    public static bool TryParse(string? input, out TimeSpan duration, out string error)
    {
        duration = TimeSpan.Zero;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            error = "Enter a duration.";
            return false;
        }

        var text = input.Trim();

        if (text.Contains(':'))
        {
            var parts = text.Split(':', StringSplitOptions.TrimEntries);
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var minutes) ||
                !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var seconds))
            {
                error = "Use mm:ss for clock-style input.";
                return false;
            }

            if (minutes < 0 || seconds < 0 || seconds >= 60)
            {
                error = "Minutes must be positive and seconds must be between 0 and 59.";
                return false;
            }

            duration = new TimeSpan(0, minutes, seconds);
            return ValidatePositiveDuration(duration, ref error);
        }

        if (text.EndsWith("min", StringComparison.OrdinalIgnoreCase))
        {
            var rawMinutes = text[..^3];
            if (!double.TryParse(rawMinutes, NumberStyles.Float, CultureInfo.InvariantCulture, out var minutesFromMinSuffix))
            {
                error = "Use a number followed by min, for example 5min.";
                return false;
            }

            duration = TimeSpan.FromMinutes(minutesFromMinSuffix);
            return ValidatePositiveDuration(duration, ref error);
        }

        if (text.EndsWith('m') || text.EndsWith('M'))
        {
            var rawMinutes = text[..^1];
            if (!double.TryParse(rawMinutes, NumberStyles.Float, CultureInfo.InvariantCulture, out var minutesFromMSuffix))
            {
                error = "Use a number followed by m, for example 5m.";
                return false;
            }

            duration = TimeSpan.FromMinutes(minutesFromMSuffix);
            return ValidatePositiveDuration(duration, ref error);
        }

        if (text.EndsWith('s') || text.EndsWith('S'))
        {
            var rawSeconds = text[..^1];
            if (!double.TryParse(rawSeconds, NumberStyles.Float, CultureInfo.InvariantCulture, out var seconds))
            {
                error = "Use a number of seconds followed by s, for example 90s.";
                return false;
            }

            duration = TimeSpan.FromSeconds(seconds);
            return ValidatePositiveDuration(duration, ref error);
        }

        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var minutesAsDefault))
        {
            error = "Unsupported format. Try 5, 5m, 5min, 1.5, 90s, or 2:30.";
            return false;
        }

        duration = TimeSpan.FromMinutes(minutesAsDefault);
        return ValidatePositiveDuration(duration, ref error);
    }

    private static bool ValidatePositiveDuration(TimeSpan duration, ref string error)
    {
        if (duration <= TimeSpan.Zero)
        {
            error = "Duration must be greater than zero.";
            return false;
        }

        return true;
    }
}
