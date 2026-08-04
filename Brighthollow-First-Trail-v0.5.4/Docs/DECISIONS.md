# Project Decision Log

## 2026-08 — Separate Game and Forge

Brighthollow: First Trail remains a standalone Godot game. Brighthollow Forge remains a separate Avalonia/.NET desktop application. Players do not need Forge installed to run the game.

## 2026-08 — Data Contract Instead of Shared Runtime Dependency

Forge may read and write documented JSON formats, but the game must not require Forge binaries at runtime. This keeps Forge reusable for other projects.

## 2026-08 — 32×32 Art Standard

Future overworld tiles and character sprite standards will target 32×32 pixels. Programmatic placeholders remain until core gameplay systems stabilize.

## 2026-08 — Developer Academy Challenges Stay Optional

Learning challenges must be isolated from release features and must never block a milestone or place the main branch at risk.
