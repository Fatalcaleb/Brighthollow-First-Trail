# Developer Academy 003 — Persistent Paths and Safe Migration

## Concepts

- `user://` is Godot's writable application-data location.
- A project name can change where `user://` points unless a custom user directory is fixed.
- Migration should copy, validate, log, and preserve the original.
- `BuildInfo.Version` is a single source of truth for runtime version metadata.

## Code-reading exercise

Open `Scripts/Saving/SaveManager.cs` and locate:

- `EnsureLegacySaveMigration()`
- `IsCompatibleLegacySave()`
- `ProjectSettings.GlobalizePath()`

Trace the conditions that prevent migration from overwriting a current save.
