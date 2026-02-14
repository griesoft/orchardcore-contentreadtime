# Orchard Core Content Read Time Module

[![CI](https://github.com/griesoft/orchardcore-contentreadtime/actions/workflows/ci.yml/badge.svg)](https://github.com/griesoft/orchardcore-contentreadtime/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Griesoft.OrchardCore.ContentReadTime.svg)](https://www.nuget.org/packages/Griesoft.OrchardCore.ContentReadTime/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Code Coverage](https://img.shields.io/badge/Coverage-100%25-brightgreen.svg)](https://github.com/griesoft/orchardcore-contentreadtime/actions/workflows/ci.yml)

An Orchard Core module that calculates and stores the estimated read time for content items.

## Features

- Calculates an estimated read time in minutes when content is published.
- Stores the result on `ContentReadTimePart` for display in templates or queries.
- Supports text from common body parts and text-bearing fields.

## Requirements

- .NET 8
- Orchard Core 2.2.1 (the version referenced by this module)

## Installation

Install the NuGet package:

`dotnet add package Griesoft.OrchardCore.ContentReadTime`

## Getting started

1. Enable the **Content Read Time** feature in the Orchard Core admin dashboard.
2. Edit a content type and attach the `ContentReadTimePart`.
3. Choose the source text and words-per-minute setting (defaults to `200`).
4. Use the stored minutes in templates, queries, or custom code.

## Configuration

When you attach `ContentReadTimePart`, you can configure the source text and reading speed:

- **Source content**: select a part or a field to read from.
  - Body parts: `HtmlBodyPart`, `MarkdownBodyPart`
  - Field types: `HtmlField`, `TextField`, `MarkdownField`
- **Words per minute**: defaults to `200` if not set or invalid.

## Usage

The module calculates read time during publish and stores the value in `ContentReadTimePart.Minutes`. Use that value anywhere you render or query content items.

## Development

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

### Code Coverage

Code coverage is automatically collected during CI runs. To generate a coverage report locally:

```bash
# Run tests with coverage collection
dotnet test --collect:"XPlat Code Coverage" --settings tests/coverlet.runsettings --results-directory ./coverage

# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"./coverage/**/coverage.cobertura.xml" -targetdir:"./coverage/report" -reporttypes:"Html"

# Open the report
open ./coverage/report/index.html  # macOS
xdg-open ./coverage/report/index.html  # Linux
start ./coverage/report/index.html  # Windows
```

The project maintains 100% code coverage across all testable code.

### Versioning

Version is managed in `Version.props`. Use the `update-version.ps1` script to update:

```powershell
# Set specific version
./update-version.ps1 -Version "1.2.3"

# Increment version
./update-version.ps1 -IncrementPatch  # 1.0.0 -> 1.0.1
./update-version.ps1 -IncrementMinor  # 1.0.0 -> 1.1.0
./update-version.ps1 -IncrementMajor  # 1.0.0 -> 2.0.0
```

### Branch Strategy

- **main** - Production releases (stable versions)
- **dev** - Active development (preview releases)
- **feature/** - Feature branches (CI only)

### Contributing

Contributions are welcome! Please submit pull requests with clear descriptions of changes.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
