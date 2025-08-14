#region
using Chaos.NLog.Logging.Abstractions;
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Moq;
using NLog.Config;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class SetupSerializationBuilderExtensionsTests
{
    [Test]
    public void RegisterCollectionTransformations_WhenRegisteredAndTransformed_ShouldWork()
    {
        // Arrange
        var mockBuilder = new Mock<ISetupSerializationBuilder>();

        var testCollection = new IsolatedTestTransformableCollection
        {
            Value = "test"
        };

        // Act - Register and immediately test transformation
        mockBuilder.Object.RegisterCollectionTransformations(TestTransform);
        var result = SetupSerializationBuilderExtensions.Transform(testCollection);

        // Assert
        result.Should()
              .NotBeNull();
        var resultType = result.GetType();

        var transformedValueProperty = resultType.GetProperty("TransformedValue");
        var typeProperty = resultType.GetProperty("Type");

        transformedValueProperty.Should()
                                .NotBeNull();

        typeProperty.Should()
                    .NotBeNull();

        transformedValueProperty.GetValue(result)
                                .Should()
                                .Be("TEST");

        typeProperty.GetValue(result)
                    .Should()
                    .Be("transformed");

        return;

        static object TestTransform(IsolatedTestTransformableCollection collection)
            => new
            {
                TransformedValue = collection.Value.ToUpper(),
                Type = "transformed"
            };
    }

    private sealed class IsolatedTestTransformableCollection : ITransformableCollection
    {
        public string Value { get; init; } = "";
    }
}