# GitHub Information — v0.5.5

## Branch

`bugfix/v0.5.5-foot-door-probe-and-versioning`

## Commit title

`v0.5.5 - use foot-based doorway detection and centralize runtime version`

## Commit description

Fixes doorway activation feeling premature and corrects stale version labels.

### Fixed

- Door transitions now test a point at the player's feet instead of the player origin
- Removed oversized doorway-zone growth from automatic transition checks
- Updated the application title and milestone labels to v0.5.5
- Replaced the pause-menu hardcoded version with centralized BuildInfo data
- Updated startup logging and instructions from the same runtime version source

### Compatibility

- Existing v0.5.x saves remain compatible
- No save schema changes

## Release title

`Brighthollow: First Trail v0.5.5 — Doorway Foot Probe and Versioning Hotfix`

## Tag

`v0.5.5`
