# GitHub Information — v0.6.1

## Branch

`bugfix/v0.6.1-save-continuity-and-spawn-placement`

## Commit title

`v0.6.1 - add stable save location and improve exit spawn placement`

## Commit description

Adds a fixed save-data location and non-destructive migration support while improving building exit placement.

### Added
- Fixed custom Godot user-data directory shared by future versions
- One-time search for compatible saves in older Brighthollow app_userdata folders
- Validation before a legacy save is copied
- Preservation of the original legacy save
- Journal History milestone in the roadmap

### Changed
- HOME and ALDER LAB exterior spawns now sit closer to their visible doorways
- Save version metadata now comes from BuildInfo

### Compatibility
- Existing compatible save JSON remains readable
- No existing save is deleted or overwritten during migration

## Release title

`Brighthollow: First Trail v0.6.1 — Save Continuity and Spawn Placement`

## Tag

`v0.6.1`
