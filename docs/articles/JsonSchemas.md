# JSON Schemas

Chaos Server provides comprehensive JSON Schema validation for all template types. These schemas enable IDE
autocomplete, validation, and documentation directly in your editor when working with game content templates.

## Available Schemas

The following schemas are available for Chaos Server templates:

- **Bulletin Board Template** - `bulletin-board-template.schema.json`
- **Dialog Template** - `dialog-template.schema.json`
- **Item Template** - `item-template.schema.json`
- **Map Template** - `map-template.schema.json`
- **Merchant Template** - `merchant-template.schema.json`
- **Monster Template** - `monster-template.schema.json`
- **Reactor Tile Template** - `reactor-tile-template.schema.json`
- **Skill Template** - `skill-template.schema.json`
- **Spell Template** - `spell-template.schema.json`

## Documentation Site Integration

All schemas are automatically published to the Chaos Server documentation site at:

```
https://docs.chaos-server.net/schemas/
```

You can reference any schema directly using this URL pattern:

```json
{
   "$schema": "https://docs.chaos-server.net/schemas/item-template.schema.json"
}
```

## JetBrains Rider Setup

### Pre-configured Templates

For JetBrains Rider users, schemas are already pre-configured in the repository! The `.idea` folder contains file
templates for all schema types that are automatically available when you clone the project.

**To use the templates:**

1. Right-click in Solution Explorer → Add
2. Look for templates like "Bulletin Board Template", "Item Template", etc.
3. Each template creates a JSON file with the correct `$schema` reference

**Available templates:**

- Bulletin Board Template
- Dialog Template
- Item Template
- Map Template
- Merchant Template
- Monster Template
- Reactor Tile Template
- Skill Template
- Spell Template

The templates are located in `.idea/.idea.Chaos/.idea/fileTemplates/` and each contains the appropriate schema URL
reference.

### Manual Schema Mapping (if needed)

If the automatic schema detection isn't working:

1. Go to File → Settings → Languages & Frameworks → Schemas and DTDs → JSON Schema Mappings
2. Add a new schema mapping for each template type
3. Set the schema URL to: `https://docs.chaos-server.net/schemas/{template-name}.schema.json`
4. Map to file patterns like: `**Configuration/Templates/{TemplateName}/**/*.json`

## Other IDEs

### Visual Studio Code

1. Install the "JSON" extension (usually pre-installed)
2. Add schema references directly in your JSON files:
   ```json
   {
     "$schema": "https://docs.chaos-server.net/schemas/item-template.schema.json",
     "templateKey": "MyItem"
     // ... rest of template
   }
   ```

3. Alternatively, configure workspace settings in `.vscode/settings.json`:
   ```json
   {
     "json.schemas": [
       {
         "fileMatch": ["**Configuration/Templates/Items/**/*.json"],
         "url": "https://docs.chaos-server.net/schemas/item-template.schema.json"
       },
       {
         "fileMatch": ["**Configuration/Templates/Monsters/**/*.json"],
         "url": "https://docs.chaos-server.net/schemas/monster-template.schema.json"
       }
       // ... add more mappings as needed
     ]
   }
   ```

### Visual Studio

1. Install the "JSON Schema" extension from Visual Studio Marketplace
2. Add schema references in your JSON files using the `$schema` property
3. Or configure project settings to map file patterns to schema URLs

### IntelliJ IDEA

1. Go to File → Settings → Languages & Frameworks → Schemas and DTDs → JSON Schema Mappings
2. Add mappings similar to the Rider setup above
3. The schema URLs remain the same: `https://docs.chaos-server.net/schemas/{template-name}.schema.json`

## Relocated Data Folder Configuration

If you've moved your `Data/` folder outside the main Chaos project directory, you'll need to recreate the project-level
schema mappings in your IDE to ensure templates in the new location have proper validation and autocomplete support.

The schema URLs remain the same (`https://docs.chaos-server.net/schemas/{template-name}.schema.json`), but you'll need
to configure new file pattern mappings that point to your relocated Data folder structure.

### JetBrains Rider with Relocated Data

If you're using JetBrains Rider and want to keep the convenient file templates:

1. Copy the file templates from the original Chaos Server project:
    - Source: `Chaos-Server/.idea/.idea.Chaos/.idea/fileTemplates/`
    - Destination: `YourDataProject/.idea/fileTemplates/`

2. The templates will then be available in your new project via Right-click → Add

Note: The Chaos Server uses nested `.idea` folders because it's a .NET solution. Your standalone Data folder project
will use the simpler `.idea/fileTemplates/` structure.