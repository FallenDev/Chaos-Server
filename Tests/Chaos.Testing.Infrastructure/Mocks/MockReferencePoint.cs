#region
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public class MockReferencePoint : IPoint
{
    /// <inheritdoc />
    public int X { get; init; }

    /// <inheritdoc />
    public int Y { get; init; }

    public MockReferencePoint(int x, int y)
    {
        X = x;
        Y = y;
    }
}