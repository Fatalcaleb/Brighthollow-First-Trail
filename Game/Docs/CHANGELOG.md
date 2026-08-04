# Changelog

## v0.6.0 — World Transition Framework

### Added
- Reusable `TransitionDoor.tscn` scene.
- Reusable `SpawnPoint.tscn` scene.
- Generic `WorldDoor` destination metadata and transition events.
- Named map spawn points with facing directions.
- Reusable building collision generation with visible doorway openings.

### Changed
- Automatic transitions now use Godot `Area2D` body detection.
- Player collision is now a foot-level footprint rather than a torso-sized rectangle.
- House and laboratory transitions use the same generic framework.
- Map transitions temporarily disable player movement during fades.

### Removed
- Hardcoded per-frame door rectangle polling.
- Manual foot-probe coordinate calculations.
