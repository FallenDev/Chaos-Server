# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Chaos-Server is a .NET 10 monorepo for a Dark Ages game server. The server runs on Kestrel (port 5000) with optional Razor Pages site and supports multiple server instances (Lobby: 4200, Login: 4201, World: 4202).

## Essential Commands

### Build and Run
```powershell
# Build solution
dotnet build Chaos.sln

# Run server
dotnet run --project Chaos/Chaos.csproj

# Run with specific configuration
dotnet run --project Chaos/Chaos.csproj --launch-profile "Chaos - Prod"
```

### Testing
```powershell
# Run all tests
dotnet test Chaos.sln --nologo

# Run single test project
dotnet test Tests/Chaos.Common.Tests/Chaos.Common.Tests.csproj --nologo

# Run specific test by name
dotnet test Chaos.sln --filter "FullyQualifiedName~TestName" --nologo

# Run tests from specific class
dotnet test Chaos.sln --filter "FullyQualifiedName~ClassName" --nologo
```

### Code Coverage
```powershell
# Generate Cobertura XML coverage (output: **/TestResults/coverage.cobertura.xml)
dotnet build Chaos.sln /p:CreateCoverageReport=true

# Generate HTML report (output: Tests/Reports/CoverageReport)
Tests/Reports/generateCoverageReport-auto.bat
```

### Tools
```powershell
# Seq configurator for logging setup
dotnet run --project Tools/SeqConfigurator/SeqConfigurator.csproj

# ChaosTool - WPF content editor for game data
dotnet run --project Tools/ChaosTool/ChaosTool.csproj
```

## Architecture & Structure

### Solution Layout
- **Chaos/** - Main server application (Program.cs entrypoint)
- **Chaos.*** - Core libraries and abstractions
- **Tests/** - TUnit test projects with FluentAssertions
- **Data/** - Game content and configuration (JSON templates, scripts)
- **Tools/** - Utility applications (SeqConfigurator, ChaosTool)
- **docs/** - DocFX documentation

### Key Architectural Components

#### Content System
Templates are JSON files in `Data/Configuration/Templates/` that define game content:
- Each template type has a Schema class (e.g., `ItemTemplateSchema`) and Factory interface (e.g., `IItemFactory`)
- Templates support `scriptKeys` (array of script class names without "Script" suffix) and `scriptVars` (configuration object)
- Content is created via factories, never directly instantiated

#### Scripting Architecture
- **Scripts** (`Chaos.Scripting`): Implement game logic, attached via `scriptKeys` in templates
- **Components** (`Chaos.Scripting.Components`): Reusable script logic implementing `IComponent` or `IConditionalComponent`
- **Functional Scripts**: Cross-cutting concerns accessed via `FunctionalScriptRegistry`
- **Formulae** (`Chaos.Formulae`): Swappable calculation logic for damage, experience, etc.

#### Entity Hierarchy
```
WorldEntity
└── MapEntity
    └── InteractableEntity
        └── VisibleEntity
            ├── Door
            └── NamedEntity
                ├── GroundEntity
                │   ├── GroundItem
                │   └── Money
                └── Creature
                    ├── Aisling (Player)
                    ├── Monster
                    └── Merchant
```

#### Configuration System
Configuration files cascade based on environment:
- **Development**: `appsettings.json` → `appsettings.logging.json` → `appsettings.local.json`
- **Production**: `appsettings.json` → `appsettings.logging.json` → `appsettings.prod.json`
- **Seq Logging**: If `Logging:UseSeq=true`, also loads `appsettings.seq.json`

Key configuration sections in `appsettings.json`:
- `Options:ChaosOptions` - Core server settings, staging directory, debug flags
- `Options:LobbyOptions/LoginOptions/WorldOptions` - Server instance ports
- `Options:AccessManagerOptions` - Authentication rules, lockouts, whitelisting
- `Options:SiteOptions` - Built-in website configuration (`EnableSite`, `ShowItems`, `ShowSkills`, `ShowSpells`, `ShowMonsters`)
- `Options:AislingCommandInterceptorOptions` - In-game command settings

#### In-Game Commands
- Commands implement `ICommand<T>` with `[Command("name", isAdmin: bool)]` attribute
- Command prefix configurable via `Options:AislingCommandInterceptorOptions:Prefix`
- Admin access: Set `"IsAdmin": true` in aisling's save JSON file

## Testing Conventions

- Framework: TUnit with FluentAssertions
- Mock support: Moq with helpers in `Tests/Chaos.Testing.Infrastructure`
- Use `[Arguments]` for parameterized tests
- Wrap attribute stacks with `//formatter:off` and `//formatter:on`
- Focus on testing one method at a time in large test files

## C# Coding Standards

- Target: .NET 10.0, C# 14 language version
- Nullable reference types enabled, implicit usings enabled
- Write high-verbosity code: descriptive names, explicit types, early returns
- Handle edge cases first
- Keep comments concise, explain "why" not "what"
- Follow existing patterns in neighboring code
- Respect package versions pinned in `Directory.Packages.props`

## Development Tips

- **Local Configuration**: Use `appsettings.local.json` for local overrides (gitignored)
- **Staging Directory**: Default `Data/` folder, override via `Options:ChaosOptions:StagingDirectory`
- **Debug Logging**: Enable packet logging with `LogRawPackets`, `LogReceivePacketCode`, `LogSendPacketCode` in `Options:ChaosOptions`
- **Coverage Reports**: HTML reports generate in `Tests/Reports/CoverageReport/`, XML in `**/TestResults/`
- **Script Development**: Script keys are class names without "Script" suffix, use `scriptVars` for configuration

## Guardrails

- Use PowerShell for commands; do not append `| cat` to commands
- Do not introduce interactive prompts in scripts or commands
- Do not add commentary inside code solely to explain actions
- Prefer semantic code search over full directory scans

## Important Paths to Avoid Full Scans

- `Data/**` - Large game content directory
- `**/bin/**`, `**/obj/**` - Build outputs
- `docs/_site/**` - Generated documentation
- `Tests/Reports/**` - Generated coverage reports
