# Brighthollow: First Trail

Original single-player monster-catching RPG built with **Godot 4.7.1 .NET** and **C#**.

## Milestone 0.2.0 — Pause, Save, and Interaction

This milestone adds the first real RPG interface systems:

- Pause menu opened with **Esc**
- Save and load from slot 1
- Player position and play time persistence
- NPC interaction using **E** or **Space**
- Dialogue box that pauses the game
- Resume, settings notice, and quit controls

## Requirements

- Godot 4.7.1 .NET/Mono edition
- .NET 8 SDK

## Controls

- Move: WASD or arrow keys
- Pause menu: Esc
- Interact/advance dialogue: E or Space

## Testing save/load

1. Walk somewhere memorable.
2. Press Esc.
3. Choose **Save Game**.
4. Resume and walk elsewhere.
5. Press Esc and choose **Load Game**.
6. The player should return to the saved position.

Godot stores the save in its per-user application data folder as `save_slot_1.json`.
