using Chaos.Time;
using Chaos.Time.Abstractions;
using FluentAssertions;

namespace Chaos.Time.Tests;

public class ResettingCounterTests
{
    [Test]
    public void Reset_Should_Clear_Counter()
    {
        var counter = new ResettingCounter(maxPerSecond: 1);

        counter.TryIncrement()
               .Should()
               .BeTrue();
        counter.Reset();

        counter.Counter
               .Should()
               .Be(0);

        counter.CanIncrement
               .Should()
               .BeTrue();
    }

    [Test]
    public void SetMaxCount_Should_Multiply_By_UpdateInterval()
    {
        var counter = new ResettingCounter(maxPerSecond: 2, updateIntervalSecs: 3); // MaxCount = 6

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.SetMaxCount(1); // 1 * 3 = 3

        // We already used 2 increments; ensure we cannot exceed 3
        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryIncrement_Should_Return_False_When_At_Max()
    {
        var counter = new ResettingCounter(maxPerSecond: 1, updateIntervalSecs: 1);

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeFalse();

        counter.CanIncrement
               .Should()
               .BeFalse();
    }

    [Test]
    public void Update_Should_Reset_Counter_When_Timer_Elapsed()
    {
        var timer = new ManualIntervalTimer();
        var counter = new ResettingCounter(maxCount: 3, timer);

        counter.TryIncrement()
               .Should()
               .BeTrue();

        counter.TryIncrement()
               .Should()
               .BeTrue();

        timer.ForceElapse();
        counter.Update(TimeSpan.FromMilliseconds(1));

        counter.Counter
               .Should()
               .Be(0);

        counter.CanIncrement
               .Should()
               .BeTrue();
    }

    private sealed class ManualIntervalTimer : IIntervalTimer
    {
        private bool _elapsed;

        public bool IntervalElapsed => _elapsed;

        public void Reset() => _elapsed = false;

        public void SetOrigin(DateTime origin) { }

        public void Update(TimeSpan delta) { }

        public void ForceElapse() => _elapsed = true;
    }
}