# Brighthollow: First Trail

Original single-player monster-catching RPG built with **Godot 4.7.1 .NET** and **C#**.

## Milestone 0.4.0 — Mossmere Interiors and Story Foundation

This milestone adds the first multi-map story framework:

- Mossmere overworld
- Player-house interior
- Professor Alder's laboratory interior
- Door interaction and fade transitions
- Guardian and Professor Alder conversations
- Story/event flags
- Journal objectives that update with progress
- Save/load of current map, player position, play time, and story flags
- Removed the temporary in-game Creature Editor from the player pause menu

The creature data remains bundled in `Data/Creatures/creatures.json`. Development editing now belongs in the separate Brighthollow Forge application.

## Controls

- Move: WASD or arrow keys
- Interact / enter doors: E or Space
- Pause menu: Esc

## Test path

1. Enter the red HOME building and speak to your guardian.
2. Exit to Mossmere.
3. Enter ALDER LAB and speak with Professor Alder.
4. Open the Journal from the pause menu after each step.
5. Save while indoors, move elsewhere, then load. The correct map and story progress should return.
