using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripting.MerchantScripts.Abstractions;

public abstract class ConfigurableMerchantScriptBase : ConfigurableScriptBase<Merchant>, IMerchantScript
{
    /// <inheritdoc />
    protected ConfigurableMerchantScriptBase(Merchant subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual bool CanMove() => true;

    /// <inheritdoc />
    public virtual bool CanSee(VisibleEntity entity) => true;

    /// <inheritdoc />
    public virtual bool CanTalk() => true;

    /// <inheritdoc />
    public virtual bool CanTurn() => true;

    /// <inheritdoc />
    public virtual bool CanUseSkill(Skill skill) => true;

    /// <inheritdoc />
    public virtual bool CanUseSpell(Spell spell) => true;

    /// <inheritdoc />
    public virtual void OnApproached(Creature source) { }

    /// <inheritdoc />
    public virtual void OnAttacked(Creature source, int damage) { }

    /// <inheritdoc />
    public virtual void OnClicked(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDeparture(Creature source) { }

    /// <inheritdoc />
    public virtual void OnGoldDroppedOn(Aisling source, int amount) { }

    /// <inheritdoc />
    public virtual void OnHealed(Creature source, int healing) { }

    /// <inheritdoc />
    public virtual void OnItemDroppedOn(Aisling source, byte slot, byte count) { }

    /// <inheritdoc />
    public virtual void OnPublicMessage(Creature source, string message) { }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) { }
}