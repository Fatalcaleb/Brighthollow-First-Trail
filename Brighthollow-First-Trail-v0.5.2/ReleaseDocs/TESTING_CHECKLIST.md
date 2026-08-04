# Brighthollow v0.5.2 Testing Checklist

## Name entry

- Confirm player and rival fields accept custom names.
- Confirm both fields stop at 16 characters.
- Confirm the live counters update while typing and deleting.
- Confirm blank or whitespace-only names cannot begin the game.
- Confirm spaces inside a valid name are preserved after trimming leading/trailing spaces.

## Suggestions

- Press **Suggest Name** several times for the player.
- Press **Suggest Name** several times for the rival.
- Confirm each suggestion stays within 16 characters.
- Confirm a suggestion can be edited afterward.
- Confirm typing a fully custom name still works.

## Persistence

- Start a game with recognizable custom names.
- Save, close the game completely, reopen, and Continue.
- Confirm both names remain correct in the pause menu, dialogue, journal, and title metadata.

## Regression checks

- New Game, Continue, and overwrite warning work.
- Appearance presets work.
- Movement, dialogue, doors, fades, journal, save, and load still work.
- No setup-screen controls are cut off at the normal window size.
