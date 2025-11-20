#region
using System.Text.Json;
using System.Text.Json.Serialization;
using Chaos.Collections.Common;
using Chaos.Common.CustomTypes;
#endregion

namespace Chaos.Common.Converters;

/// <summary>
///     JSON converter for BigFlagsCollection that serializes to a dictionary of type names to comma-delimited flag names
/// </summary>
public sealed class BigFlagsCollectionJsonConverter : JsonConverter<BigFlagsCollection>
{
    /// <inheritdoc />
    public override BigFlagsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var collection = new BigFlagsCollection();

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected start of object");

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
                return collection;

            if (reader.TokenType != JsonTokenType.PropertyName)
                throw new JsonException("Expected property name");

            var typeName = reader.GetString();

            if (string.IsNullOrEmpty(typeName))
                throw new JsonException("Type name cannot be null or empty");

            // Resolve the marker type by name (search through loaded assemblies)
            Type? markerType = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    markerType = assembly.GetTypes()
                                         .FirstOrDefault(t => t.Name == typeName);

                    if (markerType != null)
                        break;
                } catch
                {
                    // Skip assemblies that can't be loaded or inspected
                }
            }

            if (markerType == null)
                throw new JsonException($"Could not resolve type: {typeName}");

            // Read the value
            reader.Read();

            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Expected string value for flags");

            var flagsString = reader.GetString();

            if (string.IsNullOrEmpty(flagsString))
            {
                // Add None value
                collection.AddFlag(markerType, BigFlags.GetNone(markerType));

                continue;
            }

            // Parse comma-delimited flag names
            var flagNames = flagsString.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            var combinedValue = BigFlags.GetNone(markerType);

            foreach (var flagName in flagNames)
            {
                // Skip "None" as it represents an empty flags value
                if (string.Equals(flagName, "None", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (BigFlags.TryParse(
                        markerType,
                        flagName,
                        false,
                        out var flagValue))
                {
                    // Combine flags using OR
                    var combined = combinedValue.Value | flagValue.Value;
                    combinedValue = BigFlags.Create(markerType, combined);
                } else
                    throw new JsonException($"Unknown flag name '{flagName}' for type {markerType.Name}");
            }

            collection.AddFlag(markerType, combinedValue);
        }

        throw new JsonException("Unexpected end of JSON");
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, BigFlagsCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach ((var markerType, var flagValue) in value)
        {
            var typeName = markerType.Name;
            var flagsString = BigFlags.ToString(markerType, flagValue);

            writer.WriteString(typeName, flagsString);
        }

        writer.WriteEndObject();
    }
}