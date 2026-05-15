using Quiclock.Infrastructure;

namespace Quiclock.Models;

public sealed class TimerEntry : BindableBase
{
    private TimeSpan _remaining;
    private TimerStatus _status;

    public required Guid Id { get; init; }

    public required string OriginalInput { get; init; }

    public required TimeSpan Duration { get; init; }

    public required DateTimeOffset StartTime { get; init; }

    public required DateTimeOffset DueTime { get; init; }

    public TimeSpan Remaining
    {
        get => _remaining;
        set
        {
            if (SetProperty(ref _remaining, value))
            {
                RaisePropertyChanged(nameof(RemainingText));
            }
        }
    }

    public TimerStatus Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    public string RemainingText =>
        Remaining.TotalHours >= 1
            ? Remaining.ToString(@"hh\:mm\:ss")
            : Remaining.ToString(@"mm\:ss");
}
