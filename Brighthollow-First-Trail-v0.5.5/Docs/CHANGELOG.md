# Changelog

## v0.5.5

- Doorway activation now uses a foot-level probe.
- Removed the oversized 45-pixel expansion around doorway triggers.
- Centralized runtime version labels through `BuildInfo`.
- Updated application, menu, milestone, and startup version displays.

## v0.5.4 — Doorway Trigger Precision Hotfix

- Reduced exterior doorway activation zones to match the visible door width.
- Reduced interior exit activation zones to a shallow threshold strip.
- Prevented transitions from triggering while merely standing near a building.
- Preserved automatic walk-through doorway behavior and transition-loop protection.

## v0.5.3 — Doorway Loop Hotfix

- Fixed repeated automatic doorway transitions when exiting interiors.
- Moved outside spawn points beyond doorway activation zones.
- Added a clearance requirement before a doorway can trigger again.
- No save compatibility changes.

# v0.5.3 - Automatic Doorways

- Enter usable buildings by walking into their doorways.
- Exit interiors by walking into the exit doorway.
- E/Space remain reserved for NPCs and other interactable objects.
- Added transition locking to prevent repeated doorway activation.

# Changelog

## v0.5.3 - Identity Naming Update

- Added player-name suggestions
- Added rival-name suggestions
- Preserved fully custom name entry
- Standardized player and rival limits at 16 characters
- Added live character counters
- Added `PROJECT_STANDARDS.md`


## v0.5.0 — Identity

### Added

- Title screen with New Game, Continue, and Quit
- Player and rival name entry
- Four temporary appearance presets
- Existing-save warning before opening new-game setup
- Save metadata preview on title screen
- Player profile serialization
- Player and rival identity in the pause menu and story text

### Changed

- The game now begins at a title screen instead of immediately entering Mossmere.
- New games begin inside the player house.
- Save format version advanced to 0.5.0.

### Preserved

- Four-direction movement
- NPC dialogue
- Map transitions
- Journal
- Story flags
- Persistent save/load across sessions
