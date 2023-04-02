using Chaos.Objects.Panel;

namespace Chaos.Objects.Abstractions;

public interface ISpellTeacherSource : IDialogSourceEntity
{
    ICollection<Spell> SpellsToTeach { get; }
    bool TryGetSpell(string spellName, [MaybeNullWhen(false)] out Spell spell);
}