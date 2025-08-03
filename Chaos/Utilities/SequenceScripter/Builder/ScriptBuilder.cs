#region
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Utilities.SequenceScripter.Builder;

public sealed class ScriptBuilder<T> where T: Creature
{
    private readonly TimeSpan ScriptUpdateInterval;

    public ScriptBuilder(TimeSpan scriptUpdateInterval) => ScriptUpdateInterval = scriptUpdateInterval;

    public ScriptBuilder<T> AtThresholdThenAfterTimeRepeatAction(
        TimeSpan time,
        int threshold,
        Action<T> action,
        bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(
            new TimedActionDescriptor<T>(time, action, startAsElapsed)
            {
                StartingAtHealthPercent = threshold
            });

        return this;
    }

    public ScriptBuilder<T> AtThresholdThenAfterTimeRepeatSequence(TimeSpan time, int threshold, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(threshold, time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptedSequence<T> Build(T entity)
        => new(
            entity,
            ScriptUpdateInterval,
            RepeatedConditionalActions,
            RepeatedConditionalActionSequences,
            ConditionalActions,
            ConditionalActionSequences,
            RepeatedTimedActions,
            RepeatedTimedActionSequences,
            TimedActions,
            TimedActionSequences,
            RepeatedThresholdActions,
            RepeatedThresholdActionSequences,
            ThresholdActions,
            ThresholdActionSequences);

    #region Conditional
    private readonly List<ConditionalActionDescriptor<T>> RepeatedConditionalActions = [];
    private readonly List<ConditionalActionSequenceDescriptor<T>> RepeatedConditionalActionSequences = [];
    private readonly List<ConditionalActionDescriptor<T>> ConditionalActions = [];
    private readonly List<ConditionalActionSequenceDescriptor<T>> ConditionalActionSequences = [];
    #endregion

    #region Timed
    private readonly List<TimedActionDescriptor<T>> RepeatedTimedActions = [];
    private readonly List<TimedActionSequenceDescriptor<T>> RepeatedTimedActionSequences = [];
    private readonly List<TimedActionDescriptor<T>> TimedActions = [];
    private readonly List<TimedActionSequenceDescriptor<T>> TimedActionSequences = [];
    #endregion

    #region Threshold
    private readonly List<ThresholdActionDescriptor<T>> ThresholdActions = [];
    private readonly List<ThresholdActionSequenceDescriptor<T>> ThresholdActionSequences = [];
    private readonly List<ThresholdActionDescriptor<T>> RepeatedThresholdActions = [];
    private readonly List<ThresholdActionSequenceDescriptor<T>> RepeatedThresholdActionSequences = [];
    #endregion

    #region Conditional
    public ScriptBuilder<T> WhileThenRepeatAction(Func<T, bool> condition, Action<T> action)
    {
        RepeatedConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public ScriptBuilder<T> WhileThenRepeatSequence(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(TimeSpan.Zero);
        var descriptor = new ConditionalActionSequenceDescriptor<T>(condition, sequence);
        RepeatedConditionalActionSequences.Add(descriptor);

        return this;
    }

    public ScriptBuilder<T> WhenThenDoActionOnce(Func<T, bool> condition, Action<T> action)
    {
        ConditionalActions.Add(new ConditionalActionDescriptor<T>(condition, action));

        return this;
    }

    public ScriptBuilder<T> WhenThenDoSequenceOnce(Func<T, bool> condition, TimedActionSequenceBuilder<T> builder)
    {
        ConditionalActionSequences.Add(new ConditionalActionSequenceDescriptor<T>(condition, builder.Build()));

        return this;
    }
    #endregion

    #region Timed
    public ScriptBuilder<T> AfterTimeDoActionOnce(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        TimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public ScriptBuilder<T> AfterTimeDoSequenceOnce(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    public ScriptBuilder<T> AfterTimeRepeatAction(TimeSpan time, Action<T> action, bool startAsElapsed = false)
    {
        RepeatedTimedActions.Add(new TimedActionDescriptor<T>(time, action, startAsElapsed));

        return this;
    }

    public ScriptBuilder<T> AfterTimeRepeatSequence(TimeSpan time, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(time);
        RepeatedTimedActionSequences.Add(sequence);

        return this;
    }
    #endregion

    #region Threshold
    public ScriptBuilder<T> AtThresholdDoActionOnce(int threshold, Action<T> action)
    {
        ThresholdActions.Add(new ThresholdActionDescriptor<T>(threshold, action));

        return this;
    }

    public ScriptBuilder<T> AtThresholdDoSequenceOnce(int threshold, TimedActionSequenceBuilder<T> builder)
    {
        ThresholdActionSequences.Add(new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build()));

        return this;
    }

    public ScriptBuilder<T> AtThresholdRepeatAction(int threshold, Action<T> action)
    {
        RepeatedThresholdActions.Add(new ThresholdActionDescriptor<T>(threshold, action));

        return this;
    }

    public ScriptBuilder<T> AtThresholdRepeatSequence(int threshold, TimedActionSequenceBuilder<T> builder)
    {
        RepeatedThresholdActionSequences.Add(new ThresholdActionSequenceDescriptor<T>(threshold, builder.Build()));

        return this;
    }
    #endregion

    #region Complex
    public ScriptBuilder<T> AtThresholdThenAfterTimeDoActionOnce(
        TimeSpan time,
        int threshold,
        Action<T> action,
        bool startAsElapsed = false)
    {
        TimedActions.Add(
            new TimedActionDescriptor<T>(time, action, startAsElapsed)
            {
                StartingAtHealthPercent = threshold
            });

        return this;
    }

    public ScriptBuilder<T> AtThresholdThenAfterTimeDoSequenceOnce(TimeSpan time, int threshold, TimedActionSequenceBuilder<T> builder)
    {
        var sequence = builder.Build(threshold, time);
        TimedActionSequences.Add(sequence);

        return this;
    }

    //blah blah fill them in as needed, you can combine any of the 3 things
    #endregion
}