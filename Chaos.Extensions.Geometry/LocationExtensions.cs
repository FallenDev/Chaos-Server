#region
using System.Runtime.CompilerServices;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ILocation" />.
/// </summary>
public static class LocationExtensions
{
    #region Location EnsureSameMap
    /// <summary>
    ///     Ensures both locations are on the same map
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// </exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void EnsureSameMap<T1, T2>(T1 location1, T2 location2) where T1: ILocation, allows ref struct
                                                                         where T2: ILocation, allows ref struct
    {
        if (!location1.OnSameMapAs(location2))
            throw new InvalidOperationException($"{location1.ToString()} is not on the same map as {location2.ToString()}");
    }
    #endregion

    #region Location OnSameMapAs
    /// <summary>
    ///     Determines whether two <see cref="Chaos.Geometry.Abstractions.ILocation" /> are on the same map
    /// </summary>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if both <see cref="Chaos.Geometry.Abstractions.ILocation" />s are on the same map, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool OnSameMapAs<T1, T2>(this T1 location, T2 other) where T1: ILocation, allows ref struct
                                                                       where T2: ILocation, allows ref struct
        => location.Map.Equals(other.Map, StringComparison.OrdinalIgnoreCase);
    #endregion
}