# Changelog

## [0.7.2] - 2026-08-04

### Fixed
- Refreshed the active laboratory scene immediately after starter selection.
- Rival NPC now appears as soon as the rival receives a starter.
- Player and rival starter habitat markers disappear immediately.
- The remaining unchosen starter stays visible without requiring a map reload.


## [0.7.1] - 2026-08-04

### Added
- Visible rival NPC and starter-confirmation dialogue.
- Persistent starter habitat state.
- Rival starter information in the journal.
- F3 development debug screen.

### Fixed
- Godot project version now matches the release.

# Changelog

## [0.7.0] - 2026-08-04

### Added
- Starter selection, party instances, party UI, rival starter assignment, and save persistence.


## v0.6.2 — Save Migration Compile Fix

### Fixed
- Resolved the ambiguous `Environment` reference in `SaveManager.cs` by explicitly using `System.Environment`.
- Enabled nullable reference types in the C# project so nullable annotations compile in their intended context.
- Updated runtime and project version labels to v0.6.2.


## v0.6.1 — Save Continuity and Spawn Placement

### Added
- Fixed custom Godot user-data directory for future releases
- One-time migration search for compatible saves from older version-specific project folders
- Non-destructive migration that preserves the original save
- v0.7.0 journal-history milestone in the roadmap

### Changed
- HOME and ALDER LAB exterior spawn points now place the player closer to the door
- Save files record the current BuildInfo version

## v0.6.0 — World Transition Framework
- Added reusable Area2D doors and named spawn points
- Replaced hardcoded doorway polling
