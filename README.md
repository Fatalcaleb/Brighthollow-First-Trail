# Brighthollow: First Trail

Original single-player monster-catching RPG built with **Godot 4.7.1 .NET** and **C#**.

## Milestone 0.3.0 — Creature Database and Editor

This milestone adds the first data-driven creature system:

- Eight original creature definitions stored in JSON
- Three starters: Spriglet, Cindercub, and Ripplefin
- Five early wild creatures
- Base stats, elemental categories, ability, traits, capture difficulty, description, and level moves
- Built-in developer creature editor
- Validation for missing names, duplicate IDs, invalid IDs, and stat limits
- Custom data saved separately in Godot's user-data folder
- One-click reset to bundled creature data

## Opening the creature editor

1. Run the game.
2. Press **Esc**.
3. Select **Creature Editor**.
4. Edit a creature or create a new one.
5. Select **Save Custom Override**.

The editor never overwrites the bundled `Data/Creatures/creatures.json` file. It writes a custom override to `user://creatures.custom.json`. The game loads that override first when it exists.

## Requirements

- Godot 4.7.1 .NET/Mono edition
- .NET 8 SDK

## Controls

- Move: WASD or arrow keys
- Pause menu: Esc
- Interact/advance dialogue: E or Space
