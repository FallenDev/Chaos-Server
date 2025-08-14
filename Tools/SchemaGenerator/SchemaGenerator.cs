#region
using System.Reflection;
using System.Text;
using Chaos.Extensions.Common;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
#endregion

namespace SchemaGenerator;

public static class JsonSchemaGenerator
{
    private static string ConvertToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];

            if (char.IsUpper(ch))
            {
                if (((i > 0) && !char.IsUpper(input[i - 1])) || ((i > 0) && (i < (input.Length - 1)) && !char.IsUpper(input[i + 1])))
                    result.Append('-');

                result.Append(char.ToLowerInvariant(ch));
            } else
                result.Append(ch);
        }

        return result.ToString();
    }

    public static async Task GenerateAllSchemasAsync(string? schemasProjectPath = null, string? outputPath = null)
    {
        schemasProjectPath ??= AppDomain.CurrentDomain.BaseDirectory;
        outputPath ??= schemasProjectPath;

        // Load the Chaos.Schemas assembly
        var schemasAssemblyPath = Path.Combine(
            schemasProjectPath,
            "bin",
            "Debug",
            "net9.0",
            "Chaos.Schemas.dll");
        var assembly = Assembly.LoadFrom(schemasAssemblyPath);

        var schemaTypes = assembly.GetTypes()
                                  .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: not null }
                                              && t.Namespace.StartsWithI("Chaos.Schemas.Templates")
                                              && t.Name.EndsWithI("Schema"))
                                  .ToList();

        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            FlattenInheritanceHierarchy = false,
            GenerateAbstractSchemas = true,
            DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
            DefaultDictionaryValueReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
            GenerateEnumMappingDescription = true,
            TypeNameGenerator = new DefaultTypeNameGenerator(),
            SchemaNameGenerator = new DefaultSchemaNameGenerator(),
            GenerateExamples = false,
            ExcludedTypeNames = [],
            GenerateCustomNullableProperties = false,
            SchemaType = SchemaType.JsonSchema,
            GenerateAbstractProperties = false,
            IgnoreObsoleteProperties = false,
            AllowReferencesWithProperties = false,
            GenerateXmlObjects = false
        };

        foreach (var schemaType in schemaTypes)
            try
            {
                await GenerateSchemaForTypeAsync(schemaType, outputPath, settings);
            } catch (Exception ex)
            {
                Console.WriteLine($"Failed to generate schema for {schemaType.Name}: {ex.Message}");
            }
    }

    private static async Task GenerateSchemaForTypeAsync(Type type, string outputPath, NewtonsoftJsonSchemaGeneratorSettings settings)
    {
        var schema = JsonSchema.FromType(type, settings);

        // Create schemas directory in the output path
        var schemasDir = Path.Combine(outputPath, "schemas");
        Directory.CreateDirectory(schemasDir);

        // Generate the file name (convert PascalCase to kebab-case)
        var fileName = ConvertToKebabCase(type.Name.Replace("Schema", "")) + ".schema.json";
        var filePath = Path.Combine(schemasDir, fileName);

        // Generate JSON with proper formatting
        var json = schema.ToJson();
        await File.WriteAllTextAsync(filePath, json);

        Console.WriteLine($"Generated schema: {filePath}");
    }
}