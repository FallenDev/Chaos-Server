#region
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Extensions;

public static class CreatureTypeExtensions
{
    public static bool WillCollideWith(this CreatureType type, CreatureType otherType)
        => type switch
        {
            CreatureType.Normal      => true,
            CreatureType.WalkThrough => otherType is not CreatureType.Aisling,
            CreatureType.Merchant    => true,
            CreatureType.WhiteSquare => true,
            CreatureType.Aisling     => otherType is not CreatureType.WalkThrough,
            _                        => throw new ArgumentOutOfRangeException()
        };

    public static bool WillCollideWith(this CreatureType type, Creature creature)
    {
        if (creature is Aisling { IsAdmin: true, Visibility: VisibilityType.GmHidden })
            return false;

        return type.WillCollideWith(creature.Type);
    }

    public static bool WillCollideWith(this Creature creature, Creature other)
        =>

            //for the most specific creature v creature interactions
            creature.Type.WillCollideWith(other);
}