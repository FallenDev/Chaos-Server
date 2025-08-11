#region
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class EnumerableExtensionsTests
{
    [Test]
    public void ContainsI_Performs_Case_Insensitive_Search()
    {
        var src = new[]
        {
            "Alpha",
            "Bravo"
        };

        src.ContainsI("bravo")
           .Should()
           .BeTrue();

        src.ContainsI("charlie")
           .Should()
           .BeFalse();
    }

    [Test]
    public void ContainsI_Should_Return_False_When_Sequence_Does_Not_Contain_String()
    {
        // Arrange
        var enumerable = new[]
        {
            "apple",
            "banana",
            "cherry"
        };
        const string STR = "grape";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsI_Should_Return_False_When_Sequence_Is_Empty()
    {
        // Arrange
        var enumerable = Enumerable.Empty<string>();
        const string STR = "apple";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void ContainsI_Should_Return_True_When_Sequence_Contains_String()
    {
        // Arrange
        var enumerable = new[]
        {
            "apple",
            "banana",
            "cherry"
        };
        const string STR = "BaNaNa";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void NextHighest_Returns_Seed_When_No_Greater_Number()
    {
        var nums = new[]
        {
            1,
            2,
            3
        };

        nums.NextHighest(3)
            .Should()
            .Be(3);
    }

    // ReSharper disable once ArrangeAttributes
    [Test]
    [Arguments(
        new[]
        {
            3,
            8,
            5,
            2,
            6,
            1,
            9
        },
        5,
        6)]
    [Arguments(
        new[]
        {
            3,
            2,
            1
        },
        3,
        3)]
    public void NextHighest_ShouldReturnExpectedResult(int[] numbers, int seed, int expected)
    {
        // Arrange
        var listNumbers = numbers.ToList();

        // Act
        var result = listNumbers.NextHighest(seed);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void NextLowest_Returns_Seed_When_No_Lower_Number()
    {
        var nums = new[]
        {
            1,
            2,
            3
        };

        nums.NextLowest(1)
            .Should()
            .Be(1);
    }

    // ReSharper disable once ArrangeAttributes
    [Test]
    [Arguments(
        new[]
        {
            3,
            8,
            5,
            2,
            6,
            1,
            9
        },
        5,
        3)]
    [Arguments(
        new[]
        {
            3,
            2,
            1
        },
        3,
        2)]
    public void NextLowest_ShouldReturnExpectedResult(int[] numbers, int seed, int expected)
    {
        // Arrange
        var listNumbers = numbers.ToList();

        // Act
        var result = listNumbers.NextLowest(seed);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    public void TakeRandom_Returns_All_When_Count_GreaterOrEqual()
    {
        var src = new[]
        {
            1,
            2,
            3
        };

        var res = src.TakeRandom(5)
                     .ToArray();

        res.Should()
           .Equal(src);
    }

    [Test]
    public void TakeRandom_Throws_On_Negative_Count()
    {
        var src = new[]
        {
            1,
            2,
            3
        };

        Action act = () => src.TakeRandom(-1)
                              .ToArray();

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void TakeRandom_Throws_On_Null_Source()
    {
        IEnumerable<int> src = null!;

        Action act = () => src.TakeRandom(1)
                              .ToArray();

        act.Should()
           .Throw<ArgumentNullException>();
    }
}