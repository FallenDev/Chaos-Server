using Chaos.Extensions;
using Chaos.Extensions.Geometry;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class AggroTargetingScript : MonsterScriptBase
{
    private readonly IIntervalTimer TargetUpdateTimer;
    private int InitialAggro = 10;

    /// <inheritdoc />
    public AggroTargetingScript(Monster subject)
        : base(subject) =>
        TargetUpdateTimer =
            new IntervalTimer(TimeSpan.FromMilliseconds(Math.Min(250, Subject.Template.SkillIntervalMs)));

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (source.Equals(Subject))
            return;

        var aggro = aggroOverride ?? damage;

        if (aggro == 0)
            return;

        AggroList.AddOrUpdate(source.Id, _ => aggro, (_, currentAggro) => currentAggro + aggro);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        TargetUpdateTimer.Update(delta);

        if ((Target != null) && (!Target.IsAlive || !Target.OnSameMapAs(Subject)))
        {
            AggroList.Remove(Target.Id, out _);
            Target = null;
        }

        if (!TargetUpdateTimer.IntervalElapsed)
            return;

        Target = null;

        if (!Map.GetEntitiesWithinRange<Aisling>(Subject).Any())
            return;

        //first try to get target via aggro list
        //if something is already aggro, ignore aggro range
        foreach (var kvp in AggroList.OrderByDescending(kvp => kvp.Value))
        {
            if (!Map.TryGetObject<Creature>(kvp.Key, out var possibleTarget))
                continue;

            if (!possibleTarget.IsAlive || !possibleTarget.IsVisibleTo(Subject) || !possibleTarget.WithinRange(Subject))
                continue;

            Target = possibleTarget;

            break;
        }

        if (Target != null)
            return;

        //if we failed to get a target via aggroList, grab the closest aisling within aggro range
        Target ??= Map.GetEntitiesWithinRange<Aisling>(Subject, AggroRange)
                      .ThatAreVisibleTo(Subject)
                      .Where(
                          obj => !obj.Equals(Subject)
                                 && obj.IsAlive
                                 && Subject.ApproachTime.TryGetValue(obj.Id, out var time)
                                 && ((DateTime.UtcNow - time).TotalSeconds >= 1.5))
                      .ClosestOrDefault(Subject);

        //since we grabbed a new target, give them some initial aggro so we stick to them
        if (Target != null)
            AggroList[Target.Id] = InitialAggro++;
    }
}