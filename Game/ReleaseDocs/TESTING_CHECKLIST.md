# Testing Checklist — v0.6.0

## Build and launch
- [ ] Godot builds the C# project without errors.
- [ ] The title and milestone displays show v0.6.0.
- [ ] Existing v0.5.x save data loads.

## Exterior doors
- [ ] Walk directly into the HOME door and enter the house.
- [ ] Stand beside the HOME door without triggering it.
- [ ] Walk directly into ALDER LAB and enter the laboratory.
- [ ] Stand beside the laboratory door without triggering it.
- [ ] The player's head or torso touching a building does not trigger entry.
- [ ] Entry occurs when the player's feet cross the threshold.

## Interior exits
- [ ] Walk onto the house EXIT threshold and return to Mossmere.
- [ ] Walk onto the laboratory EXIT threshold and return to Mossmere.
- [ ] Standing beside either EXIT does not trigger it.
- [ ] No repeated fading or transition loop occurs.
- [ ] Holding a movement key during a transition does not retrigger the door.

## Spawn points
- [ ] Leaving HOME places the player below the HOME doorway.
- [ ] Leaving ALDER LAB places the player below the laboratory doorway.
- [ ] Entering either interior places the player safely inside and facing upward.

## Regression
- [ ] WASD and arrow movement work.
- [ ] E/Space interaction works for Mara, Guardian, Alder, bed, and terminal.
- [ ] Pause menu, journal, save, and load still work.
- [ ] Save inside an interior, close the game, reopen, and load successfully.
