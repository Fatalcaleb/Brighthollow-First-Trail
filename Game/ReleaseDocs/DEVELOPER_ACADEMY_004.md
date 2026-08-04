# Developer Academy 004 — Definitions and Instances

A `CreatureDefinition` describes a species shared by every creature of that species: base stats, types, ability, and description.

A `CreatureInstanceData` describes one individual companion: level, current HP, experience, nickname, and equipped moves.

Keeping these separate prevents duplicated species data and lets two creatures of the same species grow differently.

## Files to study
- `Scripts/Creatures/CreatureDefinition.cs`
- `Scripts/Saving/SaveManager.cs`
- `Scripts/Core/Main.cs`

## Questions
1. Which values belong to the species definition?
2. Which values can change during play?
3. Why does the save file store a species ID instead of copying all base stats?
