#region
using Chaos.Models.World.Abstractions;
using Chaos.Utilities.SequenceScripter.Builder;
#endregion

namespace Chaos.Utilities.SequenceScripter;

public sealed class ThresholdActionSequence<T> where T: Creature
{
    private readonly TimedActionSequence<T> Sequence;
    private readonly int Threshold;
    private bool Activated;
    private decimal PreviousValue;

    public ThresholdActionSequence(ThresholdActionSequenceDescriptor<T> descriptor)
    {
        Threshold = descriptor.Threshold;
        Sequence = new TimedActionSequence<T>(descriptor.Sequence);
        PreviousValue = 100.0m;
    }

    public bool Update(T entity, TimeSpan delta)
    {
        var previousHealthPercent = PreviousValue;
        var currentHealthPercent = entity.StatSheet.HealthPercent;
        PreviousValue = currentHealthPercent;

        if ((previousHealthPercent > Threshold) && (Threshold >= currentHealthPercent))
            Activated = true;

        if (!Activated)
            return false;

        Sequence.Update(entity, delta);

        return true;
    }
}