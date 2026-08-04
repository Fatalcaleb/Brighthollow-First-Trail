# Brighthollow v0.5.0 Testing Checklist

## Title Screen

- [ ] Game opens to the title screen without errors.
- [ ] Continue is disabled when no save exists.
- [ ] New Game opens trainer setup.
- [ ] Quit closes the game.
- [ ] Existing save metadata shows the player name, location, play time, and timestamp.

## New Game Setup

- [ ] Blank player name is rejected.
- [ ] Blank rival name is rejected.
- [ ] Player name accepts normal text up to the limit.
- [ ] Rival name accepts normal text up to the limit.
- [ ] All four appearance presets can be selected.
- [ ] Back returns to the title screen.
- [ ] Starting a new game with an existing save displays a warning.

## Game World

- [ ] New game starts inside the player house.
- [ ] Selected appearance is visible on the player.
- [ ] Pause menu displays the player and rival names.
- [ ] Guardian dialogue uses the player name.
- [ ] Journal displays the player name and updates normally.
- [ ] Movement works with WASD and all four arrow keys.
- [ ] E and Space interactions work.
- [ ] Doors and fades work.

## Save and Continue

- [ ] Save from the pause menu.
- [ ] Close the game completely.
- [ ] Reopen the game and verify Continue is enabled.
- [ ] Continue restores player name.
- [ ] Continue restores rival name.
- [ ] Continue restores appearance.
- [ ] Continue restores map and position.
- [ ] Continue restores journal/story flags.
- [ ] Continue restores play time.
- [ ] Load Game from the pause menu still works.

## Regression Checks

- [ ] Mara dialogue works.
- [ ] Guardian dialogue works.
- [ ] Professor Alder dialogue works.
- [ ] House and laboratory transitions work.
- [ ] Pause menu is not cut off.
- [ ] No new red debugger errors appear.
