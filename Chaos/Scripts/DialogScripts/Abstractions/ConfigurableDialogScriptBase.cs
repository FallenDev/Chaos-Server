using Chaos.Objects.Menu;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;

namespace Chaos.Scripts.DialogScripts.Abstractions;

public abstract class ConfigurableDialogScriptBase : ConfigurableScriptBase<Dialog>, IDialogScript
{
    /// <inheritdoc />
    protected ConfigurableDialogScriptBase(Dialog subject)
        : base(subject, scriptKey => subject.Template.ScriptVars[scriptKey]) { }

    /// <inheritdoc />
    public virtual void OnDisplayed(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnDisplaying(Aisling source) { }

    /// <inheritdoc />
    public virtual void OnNext(Aisling source, byte? optionIndex = null) { }

    /// <inheritdoc />
    public virtual void OnPrevious(Aisling source) { }
}