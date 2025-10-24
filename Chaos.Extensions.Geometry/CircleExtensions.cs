#region
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
    ///     Generates a sequence of point along the circumference of this circle
    /// </summary>
    /// <param name="circle">
    ///     This circle.
    /// </param>
    /// <returns>
    ///     A sequence of point along the circumfnerence of the circle
    /// </returns>
    public static IEnumerable<Point> GetOutline<T>(this T circle) where T: ICircle, allows ref struct
    {
        return InnerGetOutline(circle.Center.X, circle.Center.Y, circle.Radius);

        static IEnumerable<Point> InnerGetOutline(int x, int y, int radius)
        {
            var set = new HashSet<Point>();
            var xOffset = radius;
            var yOffset = 0;
            var decisionOver2 = 1 - xOffset;

            while (yOffset <= xOffset)
            {
                var pt1 = new Point(x + xOffset, y + yOffset);
                var pt2 = new Point(x + yOffset, y + xOffset);
                var pt3 = new Point(x - yOffset, y + xOffset);
                var pt4 = new Point(x - xOffset, y + yOffset);
                var pt5 = new Point(x - xOffset, y - yOffset);
                var pt6 = new Point(x - yOffset, y - xOffset);
                var pt7 = new Point(x + yOffset, y - xOffset);
                var pt8 = new Point(x + xOffset, y - yOffset);

                if (set.Add(pt1))
                    yield return pt1;

                if (set.Add(pt2))
                    yield return pt2;

                if (set.Add(pt3))
                    yield return pt3;

                if (set.Add(pt4))
                    yield return pt4;

                if (set.Add(pt5))
                    yield return pt5;

                if (set.Add(pt6))
                    yield return pt6;

                if (set.Add(pt7))
                    yield return pt7;

                if (set.Add(pt8))
                    yield return pt8;

                yOffset++;

                if (decisionOver2 <= 0)
                    decisionOver2 += 2 * yOffset + 1;
                else
                {
                    xOffset--;
                    decisionOver2 += 2 * (yOffset - xOffset) + 1;
                }
            }
        }
    }
    #endregion

    #region Circle GetPoints
    /// <summary>
    ///     Lazily generates all points within this circle.
    /// </summary>
    /// <param name="circle">
    /// </param>
    /// <returns>
    ///     <see cref="IEnumerable{T}" /> of <see cref="Point" />
    /// </returns>
    public static IEnumerable<Point> GetPoints<T>(this T circle) where T: ICircle, allows ref struct
    {
        return InnerGetPoints(circle.Center.X, circle.Center.Y, circle.Radius);

        static IEnumerable<Point> InnerGetPoints(int centerX, int centerY, int radius)
        {
            var set = new HashSet<Point>();
            var radiusSqrd = radius * radius;

            for (var x = centerX - radius; x <= centerX; x++)
                for (var y = centerY - radius; y <= centerY; y++)
                {
                    var xdc = x - centerX;
                    var ydc = y - centerY;

                    if ((xdc * xdc + ydc * ydc) <= radiusSqrd)
                    {
                        var xS = centerX - xdc;
                        var yS = centerY - ydc;

                        var pt1 = new Point(x, y);
                        var pt2 = new Point(x, yS);
                        var pt3 = new Point(xS, y);
                        var pt4 = new Point(xS, yS);

                        if (set.Add(pt1))
                            yield return pt1;

                        if (set.Add(pt2))
                            yield return pt2;

                        if (set.Add(pt3))
                            yield return pt3;

                        if (set.Add(pt4))
                            yield return pt4;
                    }
                }
        }
    }
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
}