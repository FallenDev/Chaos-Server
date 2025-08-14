#region
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
#endregion

namespace Chaos.Scripting.Abstractions.Tests;

public sealed class ScriptBaseTests
{
    [Test]
    public void Equals_ReturnsFalse_ForDifferentObjects()
    {
        // Arrange
        var scriptA = MockScript.Create()
                                .Object;

        var scriptB = MockCompositeScript.Create()
                                         .Object;

        // Assert
        scriptA.Equals(scriptB)
               .Should()
               .BeFalse();
    }

    [Test]
    public void Equals_ReturnsTrue_ForObjectsWithSameScriptKey()
    {
        // Arrange
        var script1 = new MockScriptBase();
        var script2 = new MockScriptBase();

        // Assert
        script1.Equals(script2)
               .Should()
               .BeTrue();
    }

    [Test]
    public void Equals_ReturnsTrue_ForSameObjects()
    {
        // Arrange
        var script = new MockScriptBase();

        // Assert
        script.Equals(script)
              .Should()
              .BeTrue();
    }

    [Test]
    public void GetHashCode_ReturnsSameValue_ForObjectsWithSameScriptKey()
    {
        // Arrange
        var script1 = new MockScriptBase();
        var script2 = new MockScriptBase();

        // Assert
        script1.GetHashCode()
               .Should()
               .Be(script2.GetHashCode());
    }
}