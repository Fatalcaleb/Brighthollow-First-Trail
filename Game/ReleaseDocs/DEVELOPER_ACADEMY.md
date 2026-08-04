# Developer Academy — Redraws and Runtime State

Godot only calls a custom `_Draw()` again when the node is queued for redraw. Changing game data does not automatically redraw custom shapes. `QueueRedraw()` tells Godot that the visual representation is stale and should be rebuilt on the next frame.
