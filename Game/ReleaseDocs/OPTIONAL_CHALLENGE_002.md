# Optional Challenge 002 — Transition Debug Message

This challenge does not advance the next milestone and should be completed on an Academy branch.

## Goal

When a transition door activates, print its door ID and destination to Godot's Output panel.

Example:

`Door mossmere_home -> player_house / entrance`

## Suggested branch

`academy/challenge-002-door-debug-message`

## Hints

- Find `OnDoorTransitionRequested` in `Main.cs`.
- Read `door.DoorId`, `door.DestinationMapId`, and `door.DestinationSpawnId`.
- Use `GD.Print(...)`.

## Suggested commit

`academy: log activated world transition doors`
