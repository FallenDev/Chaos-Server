#region
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

namespace Chaos.Extensions.Geometry;

/// <summary>
///     Provides extension methods for <see cref="Chaos.Geometry.Abstractions.ICircle" />
/// </summary>
public static class CircleExtensions
{
    #region Circle CalculateIntersectionEntryPoint
    /// <summary>
    ///     Calculates the first point at which a line intersects a circle.
    /// </summary>
    /// <param name="circle">
    ///     The circle.
    /// </param>
    /// <param name="lineStart">
    ///     The start point of the line.
    /// </param>
    /// <param name="lineEnd">
    ///     The end point of the line.
    /// </param>
    /// <returns>
    ///     The first point of intersection between the line and the circle, or null if they do not intersect.
    /// </returns>
    public static Point? CalculateIntersectionEntryPoint<TCircle, TStart, TEnd>(this TCircle circle, TStart lineStart, TEnd lineEnd)
        where TCircle: ICircle, allows ref struct
        where TStart: IPoint, allows ref struct
        where TEnd: IPoint, allows ref struct
    {
        var xDiff = Math.Abs(lineEnd.X - lineStart.X);
        var yDiff = Math.Abs(lineEnd.Y - lineStart.Y);

        var directionalX = lineStart.X < lineEnd.X ? 1 : -1;
        var directionalY = lineStart.Y < lineEnd.Y ? 1 : -1;

        var err = xDiff - yDiff;

        var retX = lineStart.X;
        var retY = lineStart.Y;

        while (true)
        {
            var distanceSquared = Math.Pow(retX - circle.Center.X, 2) + Math.Pow(retY - circle.Center.Y, 2);

            // If the current point is inside the circle, return it as the intersection point.
            if (distanceSquared <= Math.Pow(circle.Radius, 2))
                return new Point(retX, retY);

            // If the line has ended, return null.
            if ((retX == lineEnd.X) && (retY == lineEnd.Y))
                return null;

            var e2 = 2 * err;

            if (e2 > -yDiff)
            {
                err -= yDiff;
                retX += directionalX;
            }

            if (e2 < xDiff)
            {
                err += xDiff;
                retY += directionalY;
            }
        }
    }
    #endregion

    #region Circle Contains Circle
    /// <summary>
    ///     Determines whether this circle fully encompasses another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle fully encompasses the other (or edges touch); otherwise,
    ///     <c>
    ///         false
    ///     </c>
    ///     .
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsCircle<T1, T2>(this T1 circle, T2 other, DistanceType distanceType = DistanceType.Euclidean)
        where T1: ICircle, allows ref struct
        where T2: ICircle, allows ref struct
        => distanceType switch
        {
            DistanceType.Manhattan => circle.Radius >= (circle.Center.ManhattanDistanceFrom(other.Center) + other.Radius),
            DistanceType.Euclidean => circle.Radius >= (circle.Center.EuclideanDistanceFrom(other.Center) + other.Radius),
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    #endregion

    #region Circle Contains Point
    /// <summary>
    ///     Determines whether this circle contains the given point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="point">
    ///     A point.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle contains the point, otherwise
    ///     <c>
    ///         false
    ///     </c>
    ///     .
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ContainsPoint<TCircle, TPoint>(this TCircle circle, TPoint point, DistanceType distanceType = DistanceType.Euclidean)
        where TCircle: ICircle, allows ref struct
        where TPoint: IPoint, allows ref struct
        => distanceType switch
        {
            DistanceType.Manhattan => point.ManhattanDistanceFrom(circle.Center) <= circle.Radius,
            DistanceType.Euclidean => point.EuclideanDistanceFrom(circle.Center) <= circle.Radius,
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    #endregion

    #region Circle EuclideanEdgeDistanceFrom
    /// <summary>
    ///     Calculates the edge-to-center euclidean distance to some center-point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     A center-point of some entity.
    /// </param>
    /// <returns>
    ///     The euclidean distance between the center-point of this circle and the some other point, minus this circle's
    ///     radius. Value can not be negative.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanEdgeDistanceFrom<TCircle, TPoint>(this TCircle circle, TPoint other)
        where TCircle: ICircle, allows ref struct
        where TPoint: IPoint, allows ref struct
        => Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other) - circle.Radius);
    #endregion

    #region Circle EuclideanEdgeToEdgeDistanceFrom
    /// <summary>
    ///     Calculates the edge-to-edge euclidean distance to another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <returns>
    ///     The euclidean distance between the centerpoints of two circles, minus the sum of their radi. Value can not be
    ///     negative.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double EuclideanEdgeToEdgeDistanceFrom<T1, T2>(this T1 circle, T2 other) where T1: ICircle, allows ref struct
                                                                                           where T2: ICircle, allows ref struct
        => Math.Max(0.0f, circle.Center.EuclideanDistanceFrom(other.Center) - circle.Radius - other.Radius);
    #endregion

    #region Circle GetOutline
    /// <summary>
    ///     Generates a sequence of points along the circumference of this circle
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <returns>
    ///     A sequence of point along the circumference of the circle
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> GetOutline<T>(this T circle) where T: ICircle, allows ref struct
        => new CircleOutlineIterator(circle.Center.X, circle.Center.Y, circle.Radius);
    #endregion

    #region Circle Intersects Circle
    /// <summary>
    ///     Determines whether this circle intersects with another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <param name="distanceType">
    ///     The distance type to use for calculations.
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if this circle intersects the <paramref name="other" />,
    ///     <c>
    ///         false
    ///     </c>
    ///     otherwise.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Intersects<T1, T2>(this T1 circle, T2 other, DistanceType distanceType = DistanceType.Euclidean)
        where T1: ICircle, allows ref struct
        where T2: ICircle, allows ref struct
        => distanceType switch
        {
            DistanceType.Manhattan => circle.Center.ManhattanDistanceFrom(other.Center) <= (circle.Radius + other.Radius),
            DistanceType.Euclidean => circle.Center.EuclideanDistanceFrom(other.Center) <= (circle.Radius + other.Radius + double.Epsilon),
            _                      => throw new ArgumentOutOfRangeException(nameof(distanceType), distanceType, null)
        };
    #endregion

    #region Circle ManhattanEdgeDistanceFrom
    /// <summary>
    ///     Calculates the edge-to-center manhattan distance to some center-point.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     A center-point of some entity.
    /// </param>
    /// <returns>
    ///     The manhattan distance between the center-point of this circle and the some other point, minus this circle's
    ///     radius. Value can not be negative.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ManhattanEdgeDistanceFrom<TCircle, TPoint>(this TCircle circle, TPoint other)
        where TCircle: ICircle, allows ref struct
        where TPoint: IPoint, allows ref struct
        => Math.Max(0, circle.Center.ManhattanDistanceFrom(other) - circle.Radius);
    #endregion

    #region Circle ManhattanEdgeToEdgeDistanceFrom
    /// <summary>
    ///     Calculates the edge-to-edge manhattan distance to another circle.
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <param name="other">
    ///     Another circle.
    /// </param>
    /// <returns>
    ///     The manhattan distance between the centerpoints of two circles, minus the sum of their radi. Value can not be
    ///     negative.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ManhattanEdgeToEdgeDistanceFrom<T1, T2>(this T1 circle, T2 other) where T1: ICircle, allows ref struct
                                                                                          where T2: ICircle, allows ref struct
        => Math.Max(0, circle.Center.ManhattanDistanceFrom(other.Center) - circle.Radius - other.Radius);
    #endregion

    #region Circle GetPoints
    /// <summary>
    ///     Generates a sequence of points along the circumference of this circle in order of angle
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <returns>
    ///     A sequence of point along the circumference of the circle
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    ///     circle
    /// </exception>
    public static IEnumerable<Point> GetOrderedOutline(this ICircle circle)
        => circle.GetOutline()
                 .OrderBy(p => Math.Atan2(p.Y - circle.Center.Y, p.X - circle.Center.X));

    /// <summary>
    ///     Lazily generates all points within this circle.
    /// </summary>
    /// <param name="circle">
    /// </param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" /> of <see cref="Point" />
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<Point> GetPoints<T>(this T circle) where T: ICircle, allows ref struct
        => new CirclePointIterator(circle.Center.X, circle.Center.Y, circle.Radius);
    #endregion

    #region Circle GetRandomPoint
    /// <summary>
    ///     Gets a random point within this circle.
    /// </summary>
    /// <param name="circle">
    ///     The circle
    /// </param>
    /// <returns>
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Point GetRandomPoint<T>(this T circle) where T: ICircle, allows ref struct
    {
        var rngA = Random.Shared.NextDouble();
        var rngR = Random.Shared.NextDouble();
        var rngAngle = rngA * 2 * Math.PI;
        var rngRadius = Math.Sqrt(rngR) * circle.Radius;
        var x = (int)Math.Round(rngRadius * Math.Cos(rngAngle) + circle.Center.X, MidpointRounding.AwayFromZero);
        var y = (int)Math.Round(rngRadius * Math.Sin(rngAngle) + circle.Center.Y, MidpointRounding.AwayFromZero);

        return new Point(x, y);
    }

    /// <inheritdoc cref="RectangleExtensions.TryGetRandomPoint{T}" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetRandomPoint<T>(this T circle, Func<Point, bool> predicate, [NotNullWhen(true)] out Point? point)
        where T: ICircle, allows ref struct
    {
        // Approximate total points in circle: π * r²
        var totalPoints = (int)(Math.PI * circle.Radius * circle.Radius);
        var maxAttempts = Math.Max(1, totalPoints / 10);

        // Try random points for up to 10% of possibilities
        for (var i = 0; i < maxAttempts; i++)
        {
            var randomPoint = circle.GetRandomPoint();

            if (predicate(randomPoint))
            {
                point = randomPoint;

                return true;
            }
        }

        // Fall back to brute force: get all valid points and pick randomly
        var validPoints = circle.GetPoints()
                                .Where(predicate)
                                .ToList();

        if (validPoints.Count == 0)
        {
            point = null;

            return false;
        }

        point = validPoints[Random.Shared.Next(validPoints.Count)];

        return true;
    }
    #endregion
}

/// <summary>
///     A struct-based iterator for enumerating points within a circle.
/// </summary>
public struct CirclePointIterator : IEnumerator<Point>, IEnumerable<Point>
{
    private readonly int CenterX;
    private readonly int CenterY;
    private readonly int RadiusSquared;
    private readonly int Left;
    private readonly int Top;
    private readonly int Right;
    private readonly int Bottom;
    private int X;
    private int Y;

    /// <summary>
    ///     Creates a new <see cref="CirclePointIterator" />
    /// </summary>
    public CirclePointIterator(int centerX, int centerY, int radius)
    {
        CenterX = centerX;
        CenterY = centerY;
        RadiusSquared = radius * radius;
        Left = centerX - radius;
        Top = centerY - radius;
        Right = centerX + radius;
        Bottom = centerY + radius;
        X = Left;
        Y = Top - 1;
    }

    /// <inheritdoc />
    public readonly void Dispose() { }

    /// <inheritdoc />
    public bool MoveNext()
    {
        while (true)
        {
            Y++;

            if (Y > Bottom)
            {
                Y = Top;
                X++;
            }

            if (X > Right)
                return false;

            var dx = X - CenterX;
            var dy = Y - CenterY;

            if ((dx * dx + dy * dy) <= RadiusSquared)
                return true;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        X = Left;
        Y = Top - 1;
    }

    /// <inheritdoc />
    public readonly Point Current => new(X, Y);

    /// <inheritdoc />
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc />
    public readonly IEnumerator<Point> GetEnumerator() => this;

    /// <inheritdoc />
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
///     A struct-based iterator for enumerating points along the outline of a circle using the Midpoint Circle Algorithm.
/// </summary>
public struct CircleOutlineIterator : IEnumerator<Point>, IEnumerable<Point>
{
    private readonly int CenterX;
    private readonly int CenterY;
    private readonly int Radius;
    private int XOffset;
    private int YOffset;
    private int DecisionOver2;
    private int Octant;

    /// <summary>
    ///     Creates a new <see cref="CircleOutlineIterator" />
    /// </summary>
    public CircleOutlineIterator(int centerX, int centerY, int radius)
    {
        CenterX = centerX;
        CenterY = centerY;
        Radius = radius;
        XOffset = radius;
        YOffset = 0;
        DecisionOver2 = 1 - radius;
        Octant = -1;
    }

    /// <inheritdoc />
    public readonly void Dispose() { }

    /// <inheritdoc />
    public bool MoveNext()
    {
        while (true)
        {
            Octant++;

            if (Octant >= 8)
            {
                YOffset++;

                if (DecisionOver2 <= 0)
                    DecisionOver2 += 2 * YOffset + 1;
                else
                {
                    XOffset--;
                    DecisionOver2 += 2 * (YOffset - XOffset) + 1;
                }

                if (YOffset > XOffset)
                    return false;

                Octant = 0;
            }

            // Skip duplicate octants:
            // When yOffset == 0: octants 2, 4, 6, 7 are duplicates
            // When xOffset == yOffset: octants 1, 3, 5, 7 are duplicates
            if ((YOffset == 0) && Octant is 2 or 4 or 6 or 7)
                continue;

            if ((XOffset == YOffset) && Octant is 1 or 3 or 5 or 7)
                continue;

            return true;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        XOffset = Radius;
        YOffset = 0;
        DecisionOver2 = 1 - Radius;
        Octant = -1;
    }

    /// <inheritdoc />
    public readonly Point Current
        => Octant switch
        {
            0 => new Point(CenterX + XOffset, CenterY + YOffset),
            1 => new Point(CenterX + YOffset, CenterY + XOffset),
            2 => new Point(CenterX - YOffset, CenterY + XOffset),
            3 => new Point(CenterX - XOffset, CenterY + YOffset),
            4 => new Point(CenterX - XOffset, CenterY - YOffset),
            5 => new Point(CenterX - YOffset, CenterY - XOffset),
            6 => new Point(CenterX + YOffset, CenterY - XOffset),
            7 => new Point(CenterX + XOffset, CenterY - YOffset),
            _ => default
        };

    /// <inheritdoc />
    readonly object IEnumerator.Current => Current;

    /// <inheritdoc />
    public readonly IEnumerator<Point> GetEnumerator() => this;

    /// <inheritdoc />
    readonly IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}