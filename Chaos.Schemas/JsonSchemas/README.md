# JSON Schemas

This directory contains JSON Schema definitions for validating Chaos Server configuration files.

## Available Schemas

- `item-template.schema.json` - Schema for item template definitions

## Usage in Templates

Add a `$schema` reference to your JSON template files for IDE support:

```json
{
  "$schema": "../../Chaos.Schemas/JsonSchemas/item-template.schema.json",
  "name": "Apple",
  "templateKey": "apple",
  // ... rest of template
}
```

## Benefits

- **IntelliSense/Autocomplete** in VS Code and Visual Studio
- **Validation** of required fields and value constraints
- **Documentation** via property descriptions on hover
- **Type Safety** ensuring JSON matches C# model expectations

## Relative Path from Data Files

From files in `Data/Configuration/Templates/Items/`:

- Use: `"$schema": "../../../../Chaos.Schemas/JsonSchemas/item-template.schema.json"`

## Programmatic Validation

```csharp
using NJsonSchema;

var schema = await JsonSchema.FromFileAsync("Chaos.Schemas/JsonSchemas/item-template.schema.json");
var validation = schema.Validate(jsonContent);

if (validation.Any())
{
    // Handle validation errors
}
```