# GitHub Information — Brighthollow v0.5.0

## Suggested Branch

`feature/v0.5.0-identity`

## Commit Title

`v0.5.0 - add title screen, player identity, and new game flow`

## Commit Description

Introduces the first complete new-game and player identity flow.

New Features
------------
- Added title screen
- Added New Game and Continue options
- Added player name entry
- Added rival name entry
- Added four appearance presets
- Added existing-save warning before new-game setup
- Added title-screen save metadata
- Added profile information to the pause menu

Technical
---------
- Expanded save serialization with player profile data
- Added title/setup/session UI states
- Preserved compatibility with older saves through defaults
- Prepared the project for starter selection and party data

Documentation
-------------
- Updated README, changelog, roadmap, decision log, devlog, build guide, code style, and known issues
- Added testing checklist
- Added Developer Academy lesson and optional challenge

## Release Title

`Brighthollow: First Trail v0.5.0 — Identity`

## Tag

`v0.5.0`

## Release Notes

# Brighthollow: First Trail v0.5.0 — Identity

This release introduces Brighthollow's title screen and player identity flow.

### Highlights

- Begin a new game from a proper title screen
- Continue an existing save across sessions
- Choose player and rival names
- Select one of four temporary appearance presets
- View save metadata before continuing
- Preserve identity, location, story progress, and play time in the save file

### Developer Notes

The appearance system deliberately stores a preset index rather than tying saves to placeholder artwork. This allows the future 32×32 sprite system to replace the current drawings without breaking save files.

### Coming Next

- Starter selection
- Party foundation
- Rival introduction
- First basic battle
