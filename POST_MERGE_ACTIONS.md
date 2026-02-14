# Post-Merge Actions

After this PR is merged to main, please perform the following actions:

## 1. Create the dev branch

```bash
git checkout main
git pull
git checkout -b dev
git push -u origin dev
```

## 2. Remove old publish.yml workflow (if it exists on main)

The old `publish.yml` workflow from main should be replaced by the new separated workflows:
- `ci.yml` - CI only
- `cd.yml` - CD for stable releases
- `cd-preview.yml` - CD for preview releases

If `publish.yml` still exists on main after merge, delete it:

```bash
git checkout main
git pull
git rm .github/workflows/publish.yml
git commit -m "chore: remove old publish workflow, replaced by separate CI/CD workflows"
git push
```

## 3. Configure Repository Secrets

Ensure the following secret is configured in GitHub repository settings:
- Go to: Settings → Secrets and variables → Actions
- Add: `NUGET_API_KEY` with your NuGet.org API key

## 4. Set Branch Protection Rules (Optional but Recommended)

For the `main` branch:
- Require pull request reviews before merging
- Require status checks to pass (CI workflow)
- Require branches to be up to date before merging

For the `dev` branch:
- Optional: Require status checks to pass (CI workflow)

## 5. Test the Workflows

### Test CI Workflow
1. Create a feature branch from dev
2. Make a small change
3. Push and create a PR to dev
4. Verify CI runs and passes

### Test Preview CD Workflow
1. Push a change to dev branch
2. Verify cd-preview workflow runs automatically
3. Check that preview package is created

### Test Release CD Workflow
1. Go to Actions → CD - Release
2. Run workflow from main branch
3. Use "Increment Patch" option
4. Verify package is published to NuGet

## 6. Update CONTRIBUTING.md (Optional)

Consider adding a section about the branch strategy and workflow processes to help contributors understand how to contribute effectively.
