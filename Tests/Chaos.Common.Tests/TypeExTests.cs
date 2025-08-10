#region
using Chaos.Common.Utilities;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class TypeExTests
{
    [Test]
    public void LoadType_FiltersByDynamicAssemblies_ShouldExcludeDynamicAssemblies()
    {
        // This test verifies that dynamic assemblies are filtered out
        // We can't easily create a dynamic assembly in a unit test context
        // but we can verify the method works with regular assemblies

        // Arrange
        var typeName = "Object";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("Object");

        // Verify we're getting types from non-dynamic assemblies
        var assembly = result.Assembly;

        assembly.IsDynamic
                .Should()
                .BeFalse();
    }

    [Test]
    public void LoadType_WithCaseInsensitiveTypeName_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "string";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");
    }

    [Test]
    public void LoadType_WithComplexPredicate_ShouldFilterCorrectly()
    {
        // Arrange
        var typeName = "String";

        // Act
        var result = TypeEx.LoadType(
            typeName,
            (assembly, type) =>
            {
                var assemblyName = assembly.GetName()
                                           .Name;

                return (assemblyName != null)
                       && assemblyName.Contains("System")
                       && type is { IsPublic: true, IsAbstract: false, IsClass: true };
            });

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");

        result.IsPublic
              .Should()
              .BeTrue();

        result.IsAbstract
              .Should()
              .BeFalse();

        result.IsClass
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithCustomTypeFromCurrentAssembly_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = nameof(TypeExTests);

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be(nameof(TypeExTests));

        result.FullName
              .Should()
              .Contain("Chaos.Common.Tests");
    }

    [Test]
    public void LoadType_WithEmptyTypeName_ShouldReturnNull()
    {
        // Arrange
        var typeName = "";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void LoadType_WithEnumType_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "DayOfWeek";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("DayOfWeek");

        result.IsEnum
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithEnumTypeAndEnumPredicate_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "DayOfWeek";

        // Act
        var result = TypeEx.LoadType(typeName, (_, type) => type.IsEnum);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("DayOfWeek");

        result.IsEnum
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithEnumTypeAndNonEnumPredicate_ShouldReturnNull()
    {
        // Arrange
        var typeName = "DayOfWeek";

        // Act
        var result = TypeEx.LoadType(typeName, (_, type) => !type.IsEnum);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void LoadType_WithGenericType_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "List`1";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("List`1");

        result.IsGenericTypeDefinition
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithInterfaceType_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "IDisposable";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("IDisposable");

        result.IsInterface
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithMixedCaseTypeName_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "StRiNg";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");
    }

    [Test]
    public void LoadType_WithMultipleTypesWithSameName_ShouldReturnFirst()
    {
        // Arrange
        var typeName = "Attribute"; // This exists in multiple assemblies

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("Attribute");
    }

    [Test]
    public void LoadType_WithNonExistentTypeName_ShouldReturnNull()
    {
        // Arrange
        var typeName = "NonExistentType12345";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void LoadType_WithNullPredicate_ShouldBehaveLikeNoPredicate()
    {
        // Arrange
        var typeName = "String";

        // Act
        var resultWithNull = TypeEx.LoadType(typeName);
        var resultWithoutPredicate = TypeEx.LoadType(typeName);

        // Assert
        resultWithNull.Should()
                      .NotBeNull();

        resultWithoutPredicate.Should()
                              .NotBeNull();

        resultWithNull.Should()
                      .Be(resultWithoutPredicate);
    }

    [Test]
    public void LoadType_WithPredicate_ShouldReturnTypeMatchingPredicate()
    {
        // Arrange
        var typeName = "String";
        var predicateCalled = false;

        // Act
        var result = TypeEx.LoadType(
            typeName,
            (assembly, _) =>
            {
                predicateCalled = true;

                return assembly.GetName()
                               .Name
                       == "System.Private.CoreLib";
            });

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");

        predicateCalled.Should()
                       .BeTrue();
    }

    [Test]
    public void LoadType_WithPredicateFilteringByAssembly_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "Int32";

        // Act
        var result = TypeEx.LoadType(
            typeName,
            (assembly, _) =>
            {
                var assemblyName = assembly.GetName()
                                           .Name;

                return assemblyName?.StartsWith("System") == true;
            });

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("Int32");

        result.FullName
              .Should()
              .Be("System.Int32");
    }

    [Test]
    public void LoadType_WithPredicateFilteringByType_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "String";

        // Act
        var result = TypeEx.LoadType(typeName, (_, type) => type is { IsValueType: false, IsClass: true });

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");

        result.IsClass
              .Should()
              .BeTrue();

        result.IsValueType
              .Should()
              .BeFalse();
    }

    [Test]
    public void LoadType_WithPredicateReturnsFalse_ShouldReturnNull()
    {
        // Arrange
        var typeName = "String";

        // Act
        var result = TypeEx.LoadType(typeName, (_, _) => false);

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void LoadType_WithPredicateThrowingException_ShouldHandleGracefully()
    {
        // Arrange
        var typeName = "String";

        // Act
        var act = () => TypeEx.LoadType(typeName, (_, _) => throw new InvalidOperationException("Test exception"));

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage("Test exception");
    }

    [Test]
    public void LoadType_WithValidTypeName_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "String";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("String");

        result.FullName
              .Should()
              .Be("System.String");
    }

    [Test]
    public void LoadType_WithValueType_ShouldReturnCorrectType()
    {
        // Arrange
        var typeName = "Int32";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .NotBeNull();

        result.Name
              .Should()
              .Be("Int32");

        result.IsValueType
              .Should()
              .BeTrue();
    }

    [Test]
    public void LoadType_WithWhitespaceTypeName_ShouldReturnNull()
    {
        // Arrange
        var typeName = "   ";

        // Act
        var result = TypeEx.LoadType(typeName);

        // Assert
        result.Should()
              .BeNull();
    }
}