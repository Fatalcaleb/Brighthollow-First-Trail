# v0.5.5 Testing Checklist

## Foot-based doorway activation

- Stand with only the player's head/body overlapping the HOME doorway; it should not activate.
- Walk forward until the player's feet cross the visible threshold; HOME should load.
- Repeat with ALDER LAB.
- Inside both buildings, approach the exit slowly and confirm transition occurs at the feet/threshold.
- Stand beside each doorway and confirm no transition occurs.
- Hold movement during entry and exit and confirm there is no flashing or loop.

## Version display

Confirm `0.5.5` appears in:

- Godot/window application title
- Top-left milestone label
- Main/title menu
- Pause menu version label
- Godot Output startup message

## Regression

- WASD and all arrow keys work.
- E/Space still interact with NPCs and objects.
- Save, close, reopen, and Continue/Load restore the correct map and position.
- Existing v0.5.x saves still load.
