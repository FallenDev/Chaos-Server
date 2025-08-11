#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using FluentAssertions;
#endregion

namespace Chaos.DarkAges.Tests;

public sealed class MessageColorExtensionsTests
{
    [Test]
    public void ToPrefix_Default_Returns_Empty()
        => MessageColor.Default
                       .ToPrefix()
                       .Should()
                       .BeEmpty();

    [Test]
    public void ToPrefix_NonDefault_Returns_Prefix()
        => MessageColor.Blue
                       .ToPrefix()
                       .Should()
                       .Be("{=" + (char)MessageColor.Blue);
}