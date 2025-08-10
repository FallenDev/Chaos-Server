#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Pathfinding;
using FluentAssertions;
#endregion

namespace Chaos.Pathfinding.Tests;

public sealed class PathfinderTests
{
    [Test]
    public void FindPath_LimitRadiusTooSmall_FallbackPushesNextStep()
    {
        var grid = MakeGrid(5, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 0);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                LimitRadius = 0
            });

        path.Count
            .Should()
            .Be(1);

        path.Pop()
            .Should()
            .Be(new Point(1, 0));
    }

    [Test]
    public void FindPath_StraightLine_NoObstacles()
    {
        var grid = MakeGrid(5, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(4, 0);

        var path = pf.FindPath(start, end);

        path.Should()
            .NotBeEmpty();

        path.Peek()
            .Should()
            .Be(new Point(1, 0));
    }

    [Test]
    public void FindPath_WhenStartEqualsEnd_AttemptsToStepAway_ElseEmpty()
    {
        var grid = MakeGrid(3, 3);
        var pf = new Pathfinder(grid);
        var start = new Point(1, 1);

        var path = pf.FindPath(start, start);

        path.Count
            .Should()
            .BeLessThanOrEqualTo(1);
    }

    [Test]
    public void FindPath_WithWalls_BlocksAndFindsAlternate()
    {
        var walls = new[]
        {
            new Point(1, 0),
            new Point(1, 1),
            new Point(1, 2)
        };
        var grid = MakeGrid(3, 3, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 1);
        var end = new Point(2, 1);

        var path = pf.FindPath(
            start,
            end,
            new PathOptions
            {
                IgnoreWalls = false
            });

        path.Count
            .Should()
            .BeGreaterThan(0);
    }

    [Test]
    public void FindRandomDirection_ReturnsInvalid_WhenNoWalkable()
    {
        var walls = new[]
        {
            new Point(0, 0),
            new Point(1, 0),
            new Point(0, 1),
            new Point(1, 1)
        };
        var grid = MakeGrid(2, 2, walls.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var dir = pf.FindRandomDirection(new Point(0, 0));

        dir.Should()
           .Be(Direction.Invalid);
    }

    //@formatter:off
    [Test,Arguments(false, Direction.Invalid),Arguments(true,  Direction.Down)]
    //@formatter:on
    public void FindRandomDirection_WithBlockingReactor_TogglesBasedOnOption(bool ignoreBlockingReactors, Direction expected)
    {
        var reactors = new[]
        {
            new Point(0, 1)
        };
        var grid = MakeGrid(1, 2, reactors: reactors.Cast<IPoint>());
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);

        var dir = pf.FindRandomDirection(
            start,
            new PathOptions
            {
                IgnoreBlockingReactors = ignoreBlockingReactors
            });

        dir.Should()
           .Be(expected);
    }

    [Test]
    public void FindSimpleDirection_PrefersBiasAndSkipsBlocked()
    {
        var grid = MakeGrid(3, 1);
        var pf = new Pathfinder(grid);
        var start = new Point(0, 0);
        var end = new Point(2, 0);

        var blocked = new[]
        {
            new Point(1, 0)
        };

        var dir = pf.FindSimpleDirection(
            start,
            end,
            new PathOptions
            {
                BlockedPoints = blocked.Cast<IPoint>()
                                       .ToArray()
            });

        dir.Should()
           .Be(Direction.Invalid);
    }

    private static GridDetails MakeGrid(
        int w,
        int h,
        IEnumerable<IPoint>? walls = null,
        IEnumerable<IPoint>? reactors = null)
        => new()
        {
            Width = w,
            Height = h,
            Walls = (walls ?? Array.Empty<IPoint>()).ToList(),
            BlockingReactors = (reactors ?? Array.Empty<IPoint>()).ToList()
        };
}