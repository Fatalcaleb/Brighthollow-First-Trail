# Developer Academy — Collision Layers and Object Footprints

This lesson introduces CollisionObject2D, collision layers, collision masks, and why top-down games normally collide with an object's footprint rather than its entire drawing.

The player uses a small collider around the feet. Trees use a small trunk collider, allowing the canopy to visually overlap the player without behaving like a giant invisible wall.

The debug collision toggle works by temporarily disabling solid collision layers while leaving doorway trigger areas active.
