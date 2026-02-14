# Orchard Core Content Read Time Module

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

- Build: `dotnet build`
- Test: `dotnet test`

## License

This project is licensed under the MIT license.
