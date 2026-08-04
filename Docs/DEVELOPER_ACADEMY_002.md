# Developer Academy 002 — Reusable Scenes and Events

## Concepts in this milestone

- `Area2D`: detects physics bodies entering or leaving an area.
- `CollisionShape2D`: defines the actual trigger boundary.
- Reusable scenes: one transition scene can be configured for hundreds of doors.
- Events: `WorldDoor` reports a request without loading maps itself.
- Separation of responsibilities: doors describe destinations; `Main` performs transitions.
- Named spawn points: doors reference stable names rather than raw destination coordinates.

## Files to study

- `Scripts/World/Transitions/WorldDoor.cs`
- `Scripts/World/Transitions/WorldSpawnPoint.cs`
- `Scenes/World/TransitionDoor.tscn`
- `Scenes/World/SpawnPoint.tscn`
- `Scripts/Core/Main.cs`

## Key lesson

A reusable object should describe what it needs, then notify a manager. It should not know every detail of the whole game.
