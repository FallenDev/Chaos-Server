#region
using Chaos.Models.World.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public class ScriptedSequence<T> : IDeltaUpdatable
{
    protected readonly T Entity;
    protected readonly IIntervalTimer ScriptTimer;
    protected readonly TimeSpan ScriptUpdateInterval;

    public ScriptedSequence(
        T entity,
        TimeSpan scriptUpdateInterval,
        List<ConditionalActionDescriptor<T>> repeatedConditionalActions,
        List<ConditionalActionSequenceDescriptor<T>> repeatedConditionalActionSequences,
        List<ConditionalActionDescriptor<T>> conditionalActions,
        List<ConditionalActionSequenceDescriptor<T>> conditionalActionSequences,
        List<TimedActionDescriptor<T>> repeatedTimedActions,
        List<TimedActionSequenceDescriptor<T>> repeatedTimedActionSequences,
        List<TimedActionDescriptor<T>> timedActions,
        List<TimedActionSequenceDescriptor<T>> timedActionSequences)
    {
        Entity = entity;
        ScriptUpdateInterval = scriptUpdateInterval;
        ScriptTimer = new IntervalTimer(scriptUpdateInterval);

        foreach (var action in repeatedConditionalActions)
            RepeatedConditionalActions.Add(new ConditionalAction<T>(action));

        foreach (var action in repeatedConditionalActionSequences)
            RepeatedConditionalActionSequences.Add(new ConditionalActionSequence<T>(action));

        foreach (var action in conditionalActions)
            ConditionalActions.Add(new ConditionalAction<T>(action));

        foreach (var action in conditionalActionSequences)
            ConditionalActionSequences.Add(new ConditionalActionSequence<T>(action));

        foreach (var action in repeatedTimedActions)
            RepeatedTimedActions.Add(new TimedAction<T>(action));

        foreach (var action in repeatedTimedActionSequences)
            RepeatedTimedActionSequences.Add(new TimedActionSequence<T>(action));

        foreach (var action in timedActions)
            TimedActions.Add(new TimedAction<T>(action));

        foreach (var action in timedActionSequences)
            TimedActionSequences.Add(new TimedActionSequence<T>(action));
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta)
    {
        ScriptTimer.Update(delta);

        if (!ScriptTimer.IntervalElapsed)
            return;

        foreach (var action in RepeatedConditionalActions.ToList())
            action.Update(Entity);

        foreach (var action in RepeatedConditionalActionSequences.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in ConditionalActions.ToList())
            if (action.Update(Entity))
                ConditionalActions.Remove(action);

        foreach (var action in ConditionalActionSequences.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                ConditionalActionSequences.Remove(action);

        foreach (var action in RepeatedTimedActions.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in RepeatedTimedActionSequences.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in TimedActions.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                TimedActions.Remove(action);

        foreach (var action in TimedActionSequences.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                TimedActionSequences.Remove(action);
    }

    #region Conditional
    protected readonly List<ConditionalAction<T>> RepeatedConditionalActions = [];
    protected readonly List<ConditionalActionSequence<T>> RepeatedConditionalActionSequences = [];
    protected readonly List<ConditionalAction<T>> ConditionalActions = [];
    protected readonly List<ConditionalActionSequence<T>> ConditionalActionSequences = [];
    #endregion

    #region Timed
    protected readonly List<TimedAction<T>> RepeatedTimedActions = [];
    protected readonly List<TimedActionSequence<T>> RepeatedTimedActionSequences = [];
    protected readonly List<TimedAction<T>> TimedActions = [];
    protected readonly List<TimedActionSequence<T>> TimedActionSequences = [];
    #endregion
}

public sealed class CreatureScriptedSequence<T> : ScriptedSequence<T> where T: Creature
{
    private readonly List<ThresholdAction<T>> RepeatedThresholdActions = [];
    private readonly List<ThresholdActionSequence<T>> RepeatedThresholdActionSequences = [];
    private readonly List<ThresholdAction<T>> ThresholdActions = [];
    private readonly List<ThresholdActionSequence<T>> ThresholdActionSequences = [];

    public CreatureScriptedSequence(
        T entity,
        TimeSpan scriptUpdateInterval,
        List<ConditionalActionDescriptor<T>> repeatedConditionalActions,
        List<ConditionalActionSequenceDescriptor<T>> repeatedConditionalActionSequences,
        List<ConditionalActionDescriptor<T>> conditionalActions,
        List<ConditionalActionSequenceDescriptor<T>> conditionalActionSequences,
        List<TimedActionDescriptor<T>> repeatedTimedActions,
        List<TimedActionSequenceDescriptor<T>> repeatedTimedActionSequences,
        List<TimedActionDescriptor<T>> timedActions,
        List<TimedActionSequenceDescriptor<T>> timedActionSequences,
        List<ThresholdActionDescriptor<T>> repeatedThresholdActions,
        List<ThresholdActionSequenceDescriptor<T>> repeatedThresholdActionSequences,
        List<ThresholdActionDescriptor<T>> thresholdActions,
        List<ThresholdActionSequenceDescriptor<T>> thresholdActionSequences)
        : base(
            entity,
            scriptUpdateInterval,
            repeatedConditionalActions,
            repeatedConditionalActionSequences,
            conditionalActions,
            conditionalActionSequences,
            repeatedTimedActions,
            repeatedTimedActionSequences,
            timedActions,
            timedActionSequences)
    {
        foreach (var action in repeatedThresholdActions)
            RepeatedThresholdActions.Add(new ThresholdAction<T>(action));

        foreach (var action in repeatedThresholdActionSequences)
            RepeatedThresholdActionSequences.Add(new ThresholdActionSequence<T>(action));

        foreach (var action in thresholdActions)
            ThresholdActions.Add(new ThresholdAction<T>(action));

        foreach (var action in thresholdActionSequences)
            ThresholdActionSequences.Add(new ThresholdActionSequence<T>(action));
    }

    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (!ScriptTimer.IntervalElapsed)
            return;

        foreach (var action in RepeatedThresholdActions.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in RepeatedThresholdActionSequences.ToList())
            action.Update(Entity, ScriptUpdateInterval);

        foreach (var action in ThresholdActions.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                ThresholdActions.Remove(action);

        foreach (var action in ThresholdActionSequences.ToList())
            if (action.Update(Entity, ScriptUpdateInterval))
                ThresholdActionSequences.Remove(action);
    }
}