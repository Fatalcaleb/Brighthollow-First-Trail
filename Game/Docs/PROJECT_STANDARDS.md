# Project Brighthollow Standards

## Project identity

- Game: **Brighthollow: First Trail**
- Game engine: Godot 4.7.x .NET with C#
- Development suite: Brighthollow Forge with .NET 8 and Avalonia
- Final content must be original and must not use copyrighted franchise assets.

## Visual standards

- World tile size: **32 x 32 pixels**
- Base overworld character frame: **32 x 32 pixels**
- Four-direction movement
- Pixel art should use consistent nearest-neighbor scaling
- Placeholder assets must be clearly identified and replaced before final release

## Text limits

| Content | Maximum characters |
|---|---:|
| Player name | 16 |
| Rival name | 16 |
| Creature name | 20 |
| NPC name | 20 |
| Town or location name | 24 |
| Move name | 24 |
| Ability name | 24 |
| Item name | 24 |
| Dialogue choice | 32 |
| Journal title | 40 |

Player and rival names require at least one non-whitespace character. Text-entry screens should show a live character counter.

## C# naming

- PascalCase: classes, methods, properties, public members
- camelCase: parameters and local variables
- `_camelCase`: private fields
- One public class per file
- Prefer named constants over unexplained numeric values

## Data and save standards

- Game content is data-driven where practical
- Development JSON should remain human-readable
- Save data and content schemas must carry version information when migration becomes necessary
- Forge must validate and back up bundled data before permanent replacement

## Release requirements

Every milestone must include:

- Downloadable project ZIP
- GitHub branch, commit title, and commit description
- Release title and tag
- Release notes
- Testing checklist
- Known issues
- Changelog update
- Developer Academy lesson
- Optional sandbox challenge

## World transition standard

- New map transitions must use reusable transition scenes.
- Destinations must reference a map ID and named spawn ID.
- Player collision represents the character's feet/ground footprint.
- Visual door openings and physical collision openings must match.
- Map-specific transition checks must not be added to the player controller.
- Transition systems should support the 500th use without new code.
