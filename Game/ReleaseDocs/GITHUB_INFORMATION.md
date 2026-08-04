# GitHub Information — v0.6.2

## Branch
`bugfix/v0.6.2-save-migration-compile-fix`

## Commit title
`v0.6.2 - fix save migration compilation`

## Commit description
Fixes the C# build failure introduced by the legacy-save migration code.

### Fixed
- Qualified `Environment.GetEnvironmentVariable` as `System.Environment.GetEnvironmentVariable`
- Enabled nullable reference types in the project
- Updated runtime and project version labels to v0.6.2

### Compatibility
- No save schema changes
- Existing compatible saves remain readable

## Release title
`Brighthollow: First Trail v0.6.2 — Save Migration Compile Fix`

## Tag
`v0.6.2`
