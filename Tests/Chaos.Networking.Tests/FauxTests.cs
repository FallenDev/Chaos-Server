#region
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class FauxTests
{
    [Test]
    public void FakeTest()
        => true.Should()
               .BeTrue();
}