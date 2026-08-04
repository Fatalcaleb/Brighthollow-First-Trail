# v0.7.3 Testing Checklist

## Build and version
- Build the C# project with no red errors.
- Confirm v0.7.3 appears in Godot, title screen, pause menu, F3 screen, and Output.

## NPC collision
- Confirm the player cannot walk through Mara.
- Confirm the player cannot walk through the guardian.
- Confirm the player cannot walk through Professor Alder.
- Choose a starter and confirm the rival immediately becomes solid.
- Confirm each NPC can still be spoken to from nearby.

## Object collision
- Confirm the bed and TV are solid.
- Confirm the lab research terminal and starter table are solid.
- Confirm border tree trunks are solid while the canopy does not create an oversized invisible wall.
- Confirm doors and exits remain reachable.

## Debug tools
- Press F3 and click Copy All Debug Data.
- Paste into Notepad and confirm the data is readable.
- Toggle Collision OFF, close F3, and confirm the player can pass through solid NPCs and objects.
- Confirm automatic doors still activate with collision off.
- Reopen F3, toggle Collision ON, and confirm solid collision returns.
- Change maps with collision off and confirm the setting remains off until changed.

## Regression
- Test movement, doors, dialogue, journal, party, starter choice, rival state, saving, closing, reopening, and Continue.
