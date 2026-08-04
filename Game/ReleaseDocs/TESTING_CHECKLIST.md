# v0.7.1 Testing Checklist

## Version
- Godot project name shows v0.7.1.
- Title screen and pause menu show v0.7.1.

## Starter outcomes
Test with a new save for each row:

| Player chooses | Rival receives | Remains visible |
|---|---|---|
| Spriglet | Cindercub | Ripplefin |
| Cindercub | Ripplefin | Spriglet |
| Ripplefin | Spriglet | Cindercub |

For each test:
- Select the starter.
- Confirm the player starter and rival starter disappear from the table.
- Confirm only the third starter remains.
- Confirm the rival NPC appears in the lab.
- Speak to the rival and confirm the dialogue names the correct starter.
- Speak to Professor Alder and confirm starter selection cannot reopen.
- Open the Journal and confirm the rival name and starter are listed.
- Save in the lab, close the game completely, reopen, and Continue.
- Confirm the rival, journal information, and remaining starter are restored.

## Debug screen
- Press F3 during gameplay.
- Confirm game version, map, coordinates, player/rival names, starter IDs, party count, and story flags display.
- Press Esc to close it.

## Regression
- Movement, doors, dialogue, party, journal, save/load, and title-screen Continue still work.
