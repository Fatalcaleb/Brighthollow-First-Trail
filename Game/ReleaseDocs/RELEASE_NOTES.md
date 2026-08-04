# Brighthollow: First Trail v0.6.2 — Save Migration Compile Fix

This patch fixes the C# compilation error introduced in v0.6.1.

## Fixed
- `Environment` now explicitly resolves to `System.Environment` instead of conflicting with `Godot.Environment`.
- Nullable reference types are enabled for the project, removing the nullable-annotation context warnings.
- Version labels now report v0.6.2.

## Compatibility
- Save format is unchanged.
- Existing compatible saves remain readable.
