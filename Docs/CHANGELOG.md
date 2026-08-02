# Changelog

## v0.3.1

- Fixed the creature editor footer being clipped at the 960x540 internal resolution.
- Added vertical scrolling to the editor fields.
- Kept Save Custom Override, Reset to Bundled Data, and Close Editor visible.


## 0.2.0 — Pause, Save, and Interaction

- Added an Esc pause menu.
- Added slot-one JSON save and load.
- Save data includes player position, location, play time, version, and timestamp.
- Added Mara, the first interactable NPC.
- Added a dialogue box and game pausing during dialogue.
- Added menu controls for resume, settings notice, and quitting.

## 0.1.0 — Foundation

- Initial Godot 4.7.1 .NET project.
- Four-direction player movement.
- Smooth camera follow.
- Prototype Mossmere map and collisions.

## 0.2.1 - Input and dialogue hotfix

- Fixed left and right arrow-key movement by adding direct arrow-key input fallback.
- Fixed Mara's dialogue opening and closing in the same frame.
- Interaction now recognizes E and Space directly as well as the configured action.

## 0.3.0 — Creature Database and Editor

- Added a JSON-backed creature database.
- Added three original starters and five early wild creatures.
- Added base stats, elemental categories, abilities, traits, descriptions, capture difficulty, and level-up move references.
- Added a creature editor accessible from the pause menu.
- Added create, duplicate, delete, edit, save-override, and reset-to-bundled operations.
- Added validation for IDs, duplicate records, names, and stat ranges.
