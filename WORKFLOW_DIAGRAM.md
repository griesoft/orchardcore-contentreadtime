# CI/CD Workflow Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         Repository Branches                          │
└─────────────────────────────────────────────────────────────────────┘

    feature/xyz                 dev                    main
         │                       │                       │
         │                       │                       │
         ▼                       ▼                       ▼
    ┌────────┐             ┌────────┐             ┌────────┐
    │   CI   │             │   CI   │             │   CI   │
    │ Build  │             │ Build  │             │ Build  │
    │  Test  │             │  Test  │             │  Test  │
    └────────┘             └────┬───┘             └────────┘
         │                      │                       │
         │                      ▼                       │
         │                ┌──────────┐                 │
         │                │CD-Preview│                 │
    PR to dev             │  Build   │            Manual Trigger
         │                │   Test   │                 │
         │                │  Pack    │                 ▼
         │                │ Publish  │           ┌──────────┐
         │                │ Preview  │           │   CD     │
         │                └──────────┘           │ Release  │
         │                      │                │  Update  │
         │                      ▼                │ Version  │
         │                 NuGet.org             │  Build   │
         │            (preview packages)         │   Test   │
         ▼                                       │   Pack   │
    Merged to dev                                │ Publish  │
                                                 │   Tag    │
                                                 │ Release  │
                                                 └──────────┘
                                                       │
                                                       ▼
                                                  NuGet.org
                                              (stable packages)


┌─────────────────────────────────────────────────────────────────────┐
│                         Workflow Details                             │
└─────────────────────────────────────────────────────────────────────┘

╔═══════════════════════════════════════════════════════════════════╗
║  CI Workflow (ci.yml)                                             ║
╠═══════════════════════════════════════════════════════════════════╣
║  Trigger:  Push/PR to main or dev                                ║
║  Purpose:  Quality gate - build and test                         ║
║  Actions:  Checkout → Setup .NET → Restore → Build → Test        ║
║  Output:   Test results (no artifacts)                           ║
║  Time:     ~2-3 minutes                                           ║
╚═══════════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════════╗
║  CD - Preview Workflow (cd-preview.yml)                           ║
╠═══════════════════════════════════════════════════════════════════╣
║  Trigger:  Auto on push to dev                                   ║
║  Purpose:  Preview releases for testing                          ║
║  Version:  1.0.0-preview.20260214135812+abc1234                  ║
║  Actions:  Generate version → Build → Test → Pack → Publish      ║
║  Output:   Preview package on NuGet.org                          ║
║  Time:     ~3-4 minutes                                           ║
╚═══════════════════════════════════════════════════════════════════╝

╔═══════════════════════════════════════════════════════════════════╗
║  CD - Release Workflow (cd.yml)                                   ║
╠═══════════════════════════════════════════════════════════════════╣
║  Trigger:  Manual from main branch only                          ║
║  Purpose:  Production releases                                    ║
║  Version:  1.0.0, 1.2.3, 2.0.0 (semantic)                        ║
║  Actions:  Update version → Commit → Build → Test → Pack →       ║
║            Publish → Tag → GitHub Release                         ║
║  Output:   Stable package + Git tag + GitHub release             ║
║  Time:     ~4-5 minutes                                           ║
╚═══════════════════════════════════════════════════════════════════╝


┌─────────────────────────────────────────────────────────────────────┐
│                      Version Management                              │
└─────────────────────────────────────────────────────────────────────┘

Version.props (Source of Truth)
      │
      ├──→ CI: Uses version as-is for build
      │
      ├──→ CD-Preview: Appends preview suffix
      │    Example: 1.0.0 → 1.0.0-preview.20260214135812+abc1234
      │
      └──→ CD-Release: Updates version, commits, then uses
           Example: 1.0.0 → 1.0.1 (patch increment)


┌─────────────────────────────────────────────────────────────────────┐
│                       Developer Flow                                 │
└─────────────────────────────────────────────────────────────────────┘

1. Create feature branch from dev
   $ git checkout dev
   $ git checkout -b feature/my-feature

2. Make changes and commit
   $ git add .
   $ git commit -m "feat: add new feature"

3. Push and create PR to dev
   $ git push origin feature/my-feature
   
   → CI runs automatically
   → Review required
   → Tests must pass

4. Merge to dev
   → CI runs on dev
   → CD-Preview publishes automatically
   → Preview package available for testing

5. When ready for release, merge dev to main
   $ git checkout main
   $ git merge dev
   $ git push

6. Trigger release from GitHub Actions UI
   → Go to Actions → CD - Release
   → Click "Run workflow"
   → Select increment or set version
   → Stable release published


┌─────────────────────────────────────────────────────────────────────┐
│                      Package Versions                                │
└─────────────────────────────────────────────────────────────────────┘

From main (stable):
  ├── 1.0.0      (initial release)
  ├── 1.0.1      (patch - bug fix)
  ├── 1.1.0      (minor - new feature)
  └── 2.0.0      (major - breaking change)

From dev (preview):
  ├── 1.0.0-preview.20260214120000+a1b2c3d
  ├── 1.0.0-preview.20260214130000+e4f5g6h
  └── 1.1.0-preview.20260214140000+i7j8k9l

NuGet users:
  - Stable by default (main releases)
  - Add "Include prerelease" to see preview versions
```
