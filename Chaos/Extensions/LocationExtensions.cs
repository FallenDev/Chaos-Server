using Chaos.Geometry.Interfaces;

namespace Chaos.Extensions;

public static class LocationExtensions
{
    public static bool WithinRange(this ILocation location, ILocation other, int distance = 13)
    {
        var ret = location.WithinRange((IPoint)other, distance);

        if (!location.OnSameMapAs(other))
            return false;

        return ret;
    }
}