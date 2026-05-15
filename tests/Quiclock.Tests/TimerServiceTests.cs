using Quiclock.Models;
using Quiclock.Services;
using Xunit;

namespace Quiclock.Tests;

public sealed class TimerServiceTests
{
    [Fact]
    public void Timers_CompleteIndependently()
    {
        using var service = new TimerService(startTicker: false);
        var start = new DateTimeOffset(2026, 5, 15, 12, 0, 0, TimeSpan.Zero);

        var timerA = service.StartTimer("5s", TimeSpan.FromSeconds(5), start);
        var timerB = service.StartTimer("10s", TimeSpan.FromSeconds(10), start);

        service.EvaluateTimers(start.AddSeconds(6));

        Assert.Equal(TimerStatus.Completed, timerA.Status);
        Assert.Equal(TimerStatus.Running, timerB.Status);
    }

    [Fact]
    public void CancelingOneTimer_DoesNotAffectOthers()
    {
        using var service = new TimerService(startTicker: false);
        var start = new DateTimeOffset(2026, 5, 15, 12, 0, 0, TimeSpan.Zero);

        var timerA = service.StartTimer("30s", TimeSpan.FromSeconds(30), start);
        var timerB = service.StartTimer("60s", TimeSpan.FromSeconds(60), start);

        service.CancelTimer(timerA.Id);
        service.EvaluateTimers(start.AddSeconds(10));

        Assert.Equal(TimerStatus.Canceled, timerA.Status);
        Assert.Equal(TimerStatus.Running, timerB.Status);
    }
}
