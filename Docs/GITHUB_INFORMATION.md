# GitHub Information — v0.6.0

## Branch

`feature/v0.6.0-world-transition-framework`

## Commit title

`v0.6.0 - add reusable world transition framework`

## Commit description

Replaces hardcoded doorway checks with reusable Godot transition scenes and named spawn points.

### Added
- Reusable TransitionDoor scene using Area2D
- Reusable named SpawnPoint scene
- Generic destination map and spawn metadata
- Centralized transition handling and movement locking
- Building collision generation with actual doorway openings

### Changed
- Player collision now represents the character's feet
- HOME and ALDER LAB now use the same generic transition system
- Spawn facing direction is applied consistently

### Removed
- Per-frame hardcoded doorway rectangle polling
- Manual foot-probe offsets

### Compatibility
- Existing v0.5.x saves remain compatible
- No save schema changes

## Release title

`Brighthollow: First Trail v0.6.0 — World Transition Framework`

## Tag

`v0.6.0`
