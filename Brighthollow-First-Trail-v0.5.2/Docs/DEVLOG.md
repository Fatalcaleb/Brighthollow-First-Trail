# Development Journal

## v0.5.0 — Identity

The title and player-identity flow was added before starter selection so profile data could become part of the save format early. This avoids retrofitting names and appearance choices after party, battle, and story systems already depend on them.

Appearance presets remain code-drawn placeholders. The profile stores only a stable preset index so future 32×32 sprite sheets can replace the placeholder drawing without changing save files.


## v0.5.2

Name suggestions were added as optional helpers rather than fixed lists. Custom entry remains the primary path. Limits and counters now follow the project-wide standards document.
