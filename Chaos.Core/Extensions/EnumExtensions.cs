namespace Chaos.Core.Extensions;

public static class EnumExtensions
{
    public static IEnumerable<T> Flatten<T>(this T[,] map)
    {
        for (var x = 0; x < map.GetLength(0); x++)
            for (var y = 0; y < map.GetLength(1); y++)
                yield return map[x, y];
    }

    public static IEnumerable<T> Flatten<T>(this T[][] map)
    {
        for (var x = 0; x < map.Length; x++)
        {
            var arr = map[x];

            for (var y = 0; y < arr.Length; y++)
                yield return arr[y];
        }
    }

    public static IEnumerable<EquipmentSlot> ToEquipmentSlots(this EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.NotEquipment:
                yield return EquipmentSlot.None;

                yield break;
            case EquipmentType.Weapon:
                yield return EquipmentSlot.Weapon;

                yield break;
            case EquipmentType.Armor:
                yield return EquipmentSlot.Armor;

                yield break;
            case EquipmentType.OverArmor:
                yield return EquipmentSlot.Overcoat;

                yield break;
            case EquipmentType.Shield:
                yield return EquipmentSlot.Shield;

                yield break;
            case EquipmentType.Helmet:
                yield return EquipmentSlot.Helmet;

                yield break;
            case EquipmentType.OverHelmet:
                yield return EquipmentSlot.OverHelm;

                yield break;
            case EquipmentType.Earrings:
                yield return EquipmentSlot.Earrings;

                yield break;
            case EquipmentType.Necklace:
                yield return EquipmentSlot.Necklace;

                yield break;
            case EquipmentType.Ring:
                yield return EquipmentSlot.LeftRing;
                yield return EquipmentSlot.RightRing;

                yield break;
            case EquipmentType.Gauntlet:
                yield return EquipmentSlot.LeftGaunt;
                yield return EquipmentSlot.RightGaunt;

                yield break;
            case EquipmentType.Belt:
                yield return EquipmentSlot.Belt;

                yield break;
            case EquipmentType.Greaves:
                yield return EquipmentSlot.Greaves;

                yield break;
            case EquipmentType.Boots:
                yield return EquipmentSlot.Boots;

                yield break;
            case EquipmentType.Accessory:
                yield return EquipmentSlot.Accessory1;
                yield return EquipmentSlot.Accessory2;
                yield return EquipmentSlot.Accessory3;

                yield break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}