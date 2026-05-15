using System.Collections.ObjectModel;
using System.Windows.Threading;
using Quiclock.Models;

namespace Quiclock.Services;

public sealed class TimerCompletedEventArgs : EventArgs
{
    public TimerCompletedEventArgs(TimerEntry timer)
    {
        Timer = timer;
    }

    public TimerEntry Timer { get; }
}

public sealed class TimerService : IDisposable
{
    private readonly DispatcherTimer? _dispatcherTimer;
    private readonly ObservableCollection<TimerEntry> _timers = [];

    public TimerService(bool startTicker = true)
    {
        Timers = new ReadOnlyObservableCollection<TimerEntry>(_timers);

        if (startTicker)
        {
            _dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1),
            };
            _dispatcherTimer.Tick += (_, _) => EvaluateTimers(DateTimeOffset.Now);
            _dispatcherTimer.Start();
        }
    }

    public ReadOnlyObservableCollection<TimerEntry> Timers { get; }

    public event EventHandler<TimerCompletedEventArgs>? TimerCompleted;

    public TimerEntry StartTimer(string originalInput, TimeSpan duration, DateTimeOffset? now = null)
    {
        var start = now ?? DateTimeOffset.Now;
        var timer = new TimerEntry
        {
            Id = Guid.NewGuid(),
            OriginalInput = originalInput.Trim(),
            Duration = duration,
            StartTime = start,
            DueTime = start.Add(duration),
            Remaining = duration,
            Status = TimerStatus.Running,
        };

        _timers.Add(timer);
        return timer;
    }

    public void CancelTimer(Guid id)
    {
        var timer = _timers.FirstOrDefault(x => x.Id == id);
        if (timer is null || timer.Status != TimerStatus.Running)
        {
            return;
        }

        timer.Status = TimerStatus.Canceled;
        timer.Remaining = TimeSpan.Zero;
    }

    public void EvaluateTimers(DateTimeOffset now)
    {
        foreach (var timer in _timers)
        {
            if (timer.Status != TimerStatus.Running)
            {
                continue;
            }

            var remaining = timer.DueTime - now;
            if (remaining <= TimeSpan.Zero)
            {
                timer.Remaining = TimeSpan.Zero;
                timer.Status = TimerStatus.Completed;
                TimerCompleted?.Invoke(this, new TimerCompletedEventArgs(timer));
                continue;
            }

            timer.Remaining = remaining;
        }
    }

    public void Dispose()
    {
        if (_dispatcherTimer is not null)
        {
            _dispatcherTimer.Stop();
        }
    }
}
