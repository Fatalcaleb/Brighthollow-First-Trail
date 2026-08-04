# GitHub Information — v0.5.3

## Branch

`bugfix/v0.5.3-doorway-transition-loop`

## Commit title

`v0.5.3 - fix automatic doorway transition loop`

## Commit description

Fixes repeated map transitions and screen flashing when leaving a building.

### Fixed
- Moved exterior doorway spawn points outside their activation zones
- Added doorway clearance protection after every map transition
- Prevented automatic doors from re-triggering until the player steps clear
- Preserved walk-through doorway behavior
- Preserved E/Space interactions for NPCs and objects

### Compatibility
- Existing v0.5.x saves remain compatible
- No save schema changes

## Release title

`Brighthollow: First Trail v0.5.3 — Doorway Loop Hotfix`

## Tag

`v0.5.3`
