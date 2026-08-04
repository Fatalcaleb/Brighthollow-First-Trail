# v0.6.1 Testing Checklist

## Save continuity

- Launch v0.6.1 after having created a save in an older milestone.
- Confirm Continue or Load Game appears when a compatible save is found.
- Load it and verify player name, rival name, appearance, map, position, flags, and play time.
- Check Godot Output for a migration message.
- Confirm the original older save remains untouched.
- Save again, close the game completely, reopen, and load.

If no older save is discovered, create a new save and confirm it remains available after extracting/opening a later copy of v0.6.1.

## Door spawn placement

- Enter and leave HOME.
- Confirm the player appears close to, but not inside, the exterior door trigger.
- Enter and leave ALDER LAB.
- Confirm the same placement.
- Hold a movement key during both exits and verify there is no loop or flashing.
- Walk back into each building normally.

## Regression

- WASD and arrow movement
- E/Space interactions
- Pause menu and current Journal
- New Game, Continue, Save, and Load
- Version displays v0.6.1
