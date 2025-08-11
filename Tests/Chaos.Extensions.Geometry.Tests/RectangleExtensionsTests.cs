#region
using FluentAssertions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Geometry.Tests;

public sealed class RectangleExtensionsTests
{
    [Test]
    public void ContainsPoint_IRect_IPoint_Throws_When_Point_Null()
    {
        var rect = new Rectangle(
            0,
            0,
            2,
            2);
        IPoint pt = null!;

        var act = () => rect.ContainsPoint(pt);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ContainsPoint_IRect_Point_Throws_When_Rect_Null()
    {
        Action act = () => ((IRectangle)null!).ContainsPoint(new Point(0, 0));

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ContainsPoint_IRect_ValuePoint_Throws_When_Rect_Null()
    {
        Action act = () => ((IRectangle)null!).ContainsPoint(new ValuePoint(0, 0));

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ContainsPoint_IRectangle_IPoint_Outside()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            4,
            4);
        IPoint pt = new Point(10, 10);

        rect.ContainsPoint(pt)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsPoint_IRectangle_ValuePoint_MixedTypes()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            4,
            4);
        var pt = new ValuePoint(2, 2);

        rect.ContainsPoint(pt)
            .Should()
            .BeTrue();
    }

    [Test]
    public void ContainsPoint_ValueRect_IPoint_Throws_When_Point_Null()
    {
        Action act = () => new ValueRectangle(
            0,
            0,
            2,
            2).ContainsPoint(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        3,
        3,
        1,
        1,
        true)]
    [Arguments(
        0,
        0,
        3,
        3,
        0,
        0,
        true)]
    [Arguments(
        0,
        0,
        3,
        3,
        2,
        0,
        true)]
    [Arguments(
        0,
        0,
        3,
        3,
        -1,
        0,
        false)]

    //formatter:on
    public void ContainsPoint_ValueRect_Point_ReturnsExpected(
        int left,
        int top,
        int width,
        int height,
        int px,
        int py,
        bool expected)
    {
        var rect = new ValueRectangle(
            left,
            top,
            width,
            height);
        var pt = new Point(px, py);

        rect.ContainsPoint(pt)
            .Should()
            .Be(expected);
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        3,
        3,
        1,
        1,
        true)] // inside
    [Arguments(
        0,
        0,
        3,
        3,
        0,
        0,
        true)] // corner
    [Arguments(
        0,
        0,
        3,
        3,
        2,
        0,
        true)] // edge
    [Arguments(
        0,
        0,
        3,
        3,
        3,
        3,
        false)] // outside
    //formatter:on
    public void ContainsPoint_ValueRect_ValuePoint_ReturnsExpected(
        int left,
        int top,
        int width,
        int height,
        int px,
        int py,
        bool expected)
    {
        var rect = new ValueRectangle(
            left,
            top,
            width,
            height);
        var pt = new ValuePoint(px, py);

        rect.ContainsPoint(pt)
            .Should()
            .Be(expected);
    }

    [Test]
    public void ContainsPoint_ValueRectangle_IPoint_Outside()
    {
        var rect = new ValueRectangle(
            0,
            0,
            4,
            4);
        IPoint pt = new Point(-1, -1);

        rect.ContainsPoint(pt)
            .Should()
            .BeFalse();
    }

    [Test]
    public void ContainsRectangle_IRect_IRect_Throws_On_Nulls()
    {
        // both nulls are checked
        IRectangle rect = null!;
        IRectangle other = null!;

        Action act1 = () => rect.ContainsRectangle(
            new Rectangle(
                0,
                0,
                1,
                1));

        Action act2 = () => new Rectangle(
            0,
            0,
            1,
            1).ContainsRectangle(other);

        act1.Should()
            .Throw<ArgumentNullException>();

        act2.Should()
            .Throw<ArgumentNullException>();
    }

    [Test]
    public void ContainsRectangle_IRect_ValueRect_Throws_When_Rect_Null()
    {
        Action act = () => ((IRectangle)null!).ContainsRectangle(
            new ValueRectangle(
                0,
                0,
                1,
                1));

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void ContainsRectangle_IRectangle_IRectangle_False()
    {
        IRectangle outer = new Rectangle(
            0,
            0,
            2,
            2);

        IRectangle inner = new Rectangle(
            3,
            3,
            5,
            5);

        outer.ContainsRectangle(inner)
             .Should()
             .BeFalse();
    }

    [Test]
    public void ContainsRectangle_IRectangle_IRectangle_True()
    {
        IRectangle outer = new Rectangle(
            0,
            0,
            4,
            4);

        IRectangle inner = new Rectangle(
            1,
            1,
            3,
            3);

        outer.ContainsRectangle(inner)
             .Should()
             .BeTrue();
    }

    [Test]
    public void ContainsRectangle_IRectangle_ValueRectangle_False_When_Not_Contained()
    {
        IRectangle outer = new Rectangle(
            0,
            0,
            2,
            2);

        var inner = new ValueRectangle(
            1,
            1,
            4,
            4);

        outer.ContainsRectangle(inner)
             .Should()
             .BeFalse();
    }

    [Test]
    public void ContainsRectangle_IRectangle_ValueRectangle_MixedTypes()
    {
        IRectangle outer = new Rectangle(
            0,
            0,
            4,
            4);

        var inner = new ValueRectangle(
            1,
            1,
            3,
            3);

        outer.ContainsRectangle(inner)
             .Should()
             .BeTrue();
    }

    [Test]
    public void ContainsRectangle_ValueRect_IRect_Throws_When_Other_Null()
    {
        Action act = () => new ValueRectangle(
            0,
            0,
            2,
            2).ContainsRectangle(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        5,
        5,
        1,
        1,
        3,
        3,
        true)] // inner fully inside
    [Arguments(
        0,
        0,
        5,
        5,
        0,
        0,
        5,
        5,
        true)] // equal
    [Arguments(
        0,
        0,
        5,
        5,
        -1,
        -1,
        3,
        3,
        false)] // partially outside
    //formatter:on
    public void ContainsRectangle_ValueRect_ValueRect_ReturnsExpected(
        int outerLeft,
        int outerTop,
        int outerWidth,
        int outerHeight,
        int innerLeft,
        int innerTop,
        int innerWidth,
        int innerHeight,
        bool expected)
    {
        var outer = new ValueRectangle(
            outerLeft,
            outerTop,
            outerWidth,
            outerHeight);

        var inner = new ValueRectangle(
            innerLeft,
            innerTop,
            innerWidth,
            innerHeight);

        var result = outer.ContainsRectangle(inner);

        result.Should()
              .Be(expected);
    }

    [Test]
    public void ContainsRectangle_ValueRectangle_IRectangle_False_When_Not_Contained()
    {
        var outer = new ValueRectangle(
            0,
            0,
            2,
            2);

        IRectangle inner = new Rectangle(
            1,
            1,
            4,
            4);

        outer.ContainsRectangle(inner)
             .Should()
             .BeFalse();
    }

    [Test]
    public void ContainsRectangle_ValueRectangle_IRectangle_MixedTypes()
    {
        var outer = new ValueRectangle(
            0,
            0,
            4,
            4);

        IRectangle inner = new Rectangle(
            1,
            1,
            3,
            3);

        outer.ContainsRectangle(inner)
             .Should()
             .BeTrue();
    }

    [Test]
    public void GenerateMaze_All_Yielded_Points_Are_Inside_And_Start_End_Not_Walls()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            6,
            6);
        var start = new Point(0, 0);
        var end = new Point(5, 5);

        var walls = rect.GenerateMaze(start, end)
                        .ToList();

        walls.Should()
             .OnlyContain(p => rect.ContainsPoint(p));

        walls.Should()
             .NotContain(start);

        walls.Should()
             .NotContain(end);
    }

    [Test]
    public void GenerateMaze_IPoint_Overload_Throws_On_Null_Start_Or_End()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            3,
            3);

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        #pragma warning disable CA1806
        Action act1 = () => rect.GenerateMaze(null!, new Point(0, 0))
                                .ToList();

        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
        Action act2 = () => rect.GenerateMaze(new Point(0, 0), null!)
                                .ToList();
        #pragma warning restore CA1806

        act1.Should()
            .Throw<ArgumentNullException>();

        act2.Should()
            .Throw<ArgumentNullException>();
    }

    [Test]
    public void GetOutline_IRect_Returns_Expected_Perimeter_In_Order()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            3,
            2);

        var result = rect.GetOutline()
                         .ToList();

        result.Should()
              .ContainInOrder(
                  new List<Point>
                  {
                      new(0, 0),
                      new(1, 0),
                      new(2, 0),
                      new(2, 1),
                      new(1, 1),
                      new(0, 1)
                  });
    }

    [Test]
    public void GetOutline_ValueRect_Returns_Expected_Perimeter_In_Order()
    {
        var rect = new ValueRectangle(
            0,
            0,
            3,
            2); // right=2, bottom=1

        var result = rect.GetOutline()
                         .ToList();

        result.Should()
              .ContainInOrder(
                  new List<Point>
                  {
                      new(0, 0),
                      new(1, 0), // top excluding (2,0)
                      new(2, 0), // right excluding (2,1)
                      new(2, 1),
                      new(1, 1), // bottom excluding (0,1)
                      new(0, 1) // left excluding (0,0)
                  });
    }

    [Test]
    public void GetPoints_IRect_Throws_When_Rect_Null()
    {
        IRectangle rect = null!;

        var act = () => rect.GetPoints()
                            .ToList();

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void GetPoints_ValueRect_Yields_All_Points_Inclusive()
    {
        var rect = new ValueRectangle(
            0,
            0,
            2,
            2); // right=1,bottom=1

        var points = rect.GetPoints()
                         .ToList();

        points.Should()
              .BeEquivalentTo(
                  new List<Point>
                  {
                      new(0, 0),
                      new(0, 1),
                      new(1, 0),
                      new(1, 1)
                  },
                  o => o.WithoutStrictOrdering());
    }

    [Test]
    public void GetRandomPoint_IRect_Is_Inside_Rect()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            5,
            5);

        var pt = rect.GetRandomPoint();

        rect.ContainsPoint(pt)
            .Should()
            .BeTrue();
    }

    [Test]
    public void GetRandomPoint_ValueRect_Is_Inside_Rect()
    {
        var rect = new ValueRectangle(
            0,
            0,
            5,
            5);

        var pt = rect.GetRandomPoint();

        rect.ContainsPoint(pt)
            .Should()
            .BeTrue();
    }

    [Test]
    public void Intersects_IRect_IRect_Throws_On_Nulls()
    {
        IRectangle rect = null!;
        IRectangle other = null!;

        Action act1 = () => rect.Intersects(
            new Rectangle(
                0,
                0,
                1,
                1));

        Action act2 = () => new Rectangle(
            0,
            0,
            1,
            1).Intersects(other);

        act1.Should()
            .Throw<ArgumentNullException>();

        act2.Should()
            .Throw<ArgumentNullException>();
    }

    [Test]
    public void Intersects_IRect_ValueRect_Throws_When_Rect_Null()
    {
        Action act = () => ((IRectangle)null!).Intersects(
            new ValueRectangle(
                0,
                0,
                1,
                1));

        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Test]
    public void Intersects_IRectangle_IRectangle_False()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            1,
            1);

        IRectangle b = new Rectangle(
            3,
            3,
            4,
            4);

        a.Intersects(b)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Intersects_IRectangle_IRectangle_True()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            4,
            4);

        IRectangle b = new Rectangle(
            3,
            3,
            6,
            6);

        a.Intersects(b)
         .Should()
         .BeTrue();
    }

    [Test]
    public void Intersects_IRectangle_ValueRectangle_False_When_Separated()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            1,
            1);

        var b = new ValueRectangle(
            3,
            3,
            4,
            4);

        a.Intersects(b)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Intersects_IRectangle_ValueRectangle_MixedTypes()
    {
        IRectangle a = new Rectangle(
            0,
            0,
            4,
            4);

        var b = new ValueRectangle(
            3,
            3,
            6,
            6);

        a.Intersects(b)
         .Should()
         .BeTrue();
    }

    [Test]
    public void Intersects_ValueRect_IRect_Throws_When_Other_Null()
    {
        Action act = () => new ValueRectangle(
            0,
            0,
            1,
            1).Intersects(null!);

        act.Should()
           .Throw<ArgumentNullException>();
    }

    //formatter:off
    [Test]
    [Arguments(
        0,
        0,
        2,
        2,
        5,
        5,
        2,
        2,
        false)] // separated
    [Arguments(
        0,
        0,
        3,
        3,
        3,
        0,
        2,
        3,
        false)] // touching edge (adjacent, no overlap)
    [Arguments(
        0,
        0,
        3,
        3,
        1,
        1,
        1,
        1,
        true)] // inside
    //formatter:on
    public void Intersects_ValueRect_ValueRect_ReturnsExpected(
        int l1,
        int t1,
        int w1,
        int h1,
        int l2,
        int t2,
        int w2,
        int h2,
        bool expected)
    {
        var a = new ValueRectangle(
            l1,
            t1,
            w1,
            h1);

        var b = new ValueRectangle(
            l2,
            t2,
            w2,
            h2);

        a.Intersects(b)
         .Should()
         .Be(expected);
    }

    [Test]
    public void Intersects_ValueRectangle_IRectangle_False_When_Separated()
    {
        var a = new ValueRectangle(
            0,
            0,
            1,
            1);

        IRectangle b = new Rectangle(
            3,
            3,
            4,
            4);

        a.Intersects(b)
         .Should()
         .BeFalse();
    }

    [Test]
    public void Intersects_ValueRectangle_IRectangle_MixedTypes()
    {
        var a = new ValueRectangle(
            0,
            0,
            4,
            4);

        IRectangle b = new Rectangle(
            3,
            3,
            6,
            6);

        a.Intersects(b)
         .Should()
         .BeTrue();
    }

    [Test]
    public void IntersectsCircle_IRect_ICircle_Switch_Works_And_Invalid_Throws()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            10,
            10);
        ICircle inside = new Circle(new Point(5, 5), 1);
        ICircle diffCase = new Circle(new Point(3, 4), 6);

        rect.Intersects(inside)
            .Should()
            .BeTrue();

        rect.Intersects(inside, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        Action act = () => rect.Intersects(diffCase, (DistanceType)42);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void IntersectsCircle_IRect_ValueCircle_Switch_Works_And_Invalid_Throws()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            10,
            10);
        var inside = new ValueCircle(new Point(5, 5), 1);
        var diffCase = new ValueCircle(new Point(3, 4), 6);

        rect.Intersects(inside)
            .Should()
            .BeTrue();

        rect.Intersects(inside, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        Action act = () => rect.Intersects(new ValueCircle(new Point(3, 4), 6), (DistanceType)42);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void IntersectsCircle_ValueRect_ICircle_Switch_Works_And_Invalid_Throws()
    {
        var rect = new ValueRectangle(
            0,
            0,
            10,
            10);
        ICircle inside = new Circle(new Point(5, 5), 1);
        ICircle diffCase = new Circle(new Point(3, 4), 6);

        rect.Intersects(inside)
            .Should()
            .BeTrue();

        rect.Intersects(inside, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        Action act = () => new ValueRectangle(
            0,
            0,
            10,
            10).Intersects(diffCase, (DistanceType)42);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void IntersectsCircle_ValueRect_ValueCircle_Switch_Works_And_Invalid_Throws()
    {
        var rect = new ValueRectangle(
            0,
            0,
            10,
            10); // [0..9]x[0..9]
        var inside = new ValueCircle(new Point(5, 5), 1);
        var tangent = new ValueCircle(new Point(10, 5), 1); // distance == r => false
        var diffCase = new ValueCircle(new Point(3, 4), 6); // Euclidean 5 < 6, Manhattan 7 < 6? false -> still true, adjust expectations

        rect.Intersects(inside)
            .Should()
            .BeTrue();

        rect.Intersects(inside, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        rect.Intersects(tangent)
            .Should()
            .BeFalse();

        rect.Intersects(tangent, DistanceType.Manhattan)
            .Should()
            .BeFalse();

        rect.Intersects(diffCase)
            .Should()
            .BeTrue();

        rect.Intersects(diffCase, DistanceType.Manhattan)
            .Should()
            .BeTrue();

        Action act = () => new ValueRectangle(
            0,
            0,
            10,
            10).Intersects(new ValueCircle(new Point(3, 4), 6), (DistanceType)42);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TryGetRandomPoint_IRect_Returns_False_When_No_Match()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            2,
            2);

        var success = rect.TryGetRandomPoint(_ => false, out var point);

        success.Should()
               .BeFalse();

        point.Should()
             .BeNull();
    }

    [Test]
    public void TryGetRandomPoint_IRect_Returns_True_When_Match()
    {
        IRectangle rect = new Rectangle(
            0,
            0,
            2,
            2);

        var success = rect.TryGetRandomPoint(_ => true, out var point);

        success.Should()
               .BeTrue();

        point.Should()
             .NotBeNull();

        rect.ContainsPoint(point)
            .Should()
            .BeTrue();
    }

    [Test]
    public void TryGetRandomPoint_ValueRect_Returns_False_When_No_Match()
    {
        var rect = new ValueRectangle(
            0,
            0,
            2,
            2);

        var success = rect.TryGetRandomPoint(_ => false, out var point);

        success.Should()
               .BeFalse();

        point.Should()
             .BeNull();
    }

    [Test]
    public void TryGetRandomPoint_ValueRect_Returns_True_When_Match()
    {
        var rect = new ValueRectangle(
            0,
            0,
            2,
            2);

        var success = rect.TryGetRandomPoint(_ => true, out var point);

        success.Should()
               .BeTrue();

        point.Should()
             .NotBeNull();

        rect.ContainsPoint(point)
            .Should()
            .BeTrue();
    }
}