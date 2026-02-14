# CI/CD Workflows

This document describes the automated workflows used in this repository.

## Workflows Overview

### 1. CI Workflow (`ci.yml`)

**Purpose**: Continuous Integration - Build and test code on every push and pull request.

**Triggers**:
- Push to `main` or `dev` branches
- Pull requests to `main` or `dev` branches
- Manual dispatch

**What it does**:
- Checks out code
- Sets up .NET environment
- Restores dependencies
- Builds the project
- Runs all tests

**No publishing** - This workflow only validates code quality.

### 2. CD - Release Workflow (`cd.yml`)

**Purpose**: Continuous Deployment - Publish stable releases to NuGet.

**Triggers**:
- Manual dispatch only (workflow_dispatch)
- **Only runs from `main` branch**

**Inputs**:
- `version`: Explicit version to publish (e.g., "1.2.3")
- `increment`: Version increment type (patch/minor/major) - used if version is empty

**What it does**:
1. Updates version in `Version.props`
2. Commits the version change
3. Builds and tests the project
4. Creates NuGet package
5. Publishes to NuGet.org
6. Creates Git tag (e.g., `v1.2.3`)
7. Creates GitHub Release

**Requirements**:
- `NUGET_API_KEY` secret must be configured in repository settings

### 3. CD - Preview Workflow (`cd-preview.yml`)

**Purpose**: Continuous Deployment - Publish preview/beta releases to NuGet.

**Triggers**:
- Push to `dev` branch
- Manual dispatch
- **Only runs from `dev` branch**

**What it does**:
1. Gets base version from `Version.props`
2. Creates preview version: `{base}-preview.{timestamp}+{shortSha}`
   - Example: `1.0.0-preview.20260214135812+abc1234`
3. Builds and tests with preview version
4. Creates preview NuGet package
5. Publishes to NuGet.org as preview
6. Uploads artifacts to GitHub Actions

**No version commits or tags** - Preview versions are ephemeral.

## Branch Strategy

### main
- **Protected branch** (recommended)
- Only stable, production-ready code
- Releases publish as stable versions (e.g., `1.0.0`, `1.2.3`)
- Manual deployment via CD workflow

### dev
- Active development branch
- Automatic preview releases on every push
- Preview versions for testing (e.g., `1.0.0-preview.20260214135812+abc1234`)

### Feature branches (feature/*, bugfix/*, etc.)
- CI only - builds and tests
- No publishing
- Merge to `dev` via pull request

## Versioning

Version is stored in `Version.props` at the repository root:

```xml
<Project>
  <PropertyGroup>
    <Version>1.0.0</Version>
  </PropertyGroup>
</Project>
```

### Manual Version Updates

Use the `update-version.ps1` PowerShell script:

```powershell
# Set explicit version
./update-version.ps1 -Version "1.2.3"

# Increment versions
./update-version.ps1 -IncrementPatch  # 1.0.0 -> 1.0.1
./update-version.ps1 -IncrementMinor  # 1.0.0 -> 1.1.0
./update-version.ps1 -IncrementMajor  # 1.0.0 -> 2.0.0
```

### Automated Version Updates

The CD workflow handles version updates automatically:
- Updates `Version.props`
- Commits the change
- Creates a Git tag
- Pushes to the repository

## Release Process

### Stable Release (from main)

1. Ensure all changes are merged to `main`
2. Go to Actions → CD - Release
3. Click "Run workflow"
4. Choose version strategy:
   - Enter explicit version (e.g., "1.2.3"), OR
   - Select increment type (patch/minor/major)
5. Click "Run workflow"
6. Workflow will:
   - Update version
   - Build and test
   - Publish to NuGet
   - Create release tag and GitHub release

### Preview Release (from dev)

Preview releases happen automatically:
1. Merge changes to `dev` branch
2. Push triggers automatic workflow
3. Preview package is published with timestamp version
4. Use for testing before stable release

## Required Secrets

Configure these in repository settings (Settings → Secrets and variables → Actions):

- `NUGET_API_KEY`: API key for publishing to NuGet.org
  - Get from: https://www.nuget.org/account/apikeys
  - Scope: Push new packages and package versions

## Troubleshooting

### Workflow not running
- Check branch name matches trigger conditions
- For CD workflows, ensure running from correct branch

### Version not updating
- Check `Version.props` exists in repository root
- Verify PowerShell script has execute permissions

### NuGet publish fails
- Verify `NUGET_API_KEY` secret is set
- Check API key has correct permissions
- Ensure version doesn't already exist on NuGet.org

### Preview versions not appearing
- Check if running from `dev` branch
- Verify workflow completed successfully
- Check NuGet.org for preview versions (may take a few minutes)
