# Developer Academy 001 — Classes, Methods, and Godot Input

## Goal

Understand how a Godot node is connected to a C# class and how code runs every frame.

## Key Concepts

### Class

`PlayerController` is a C# class. It describes the behavior of the player node.

### Inheritance

`PlayerController : CharacterBody2D` means the class receives movement and collision features from Godot's `CharacterBody2D`.

### Method

A method is a named block of code. Godot calls special override methods automatically.

- `_Ready()` runs when the node enters the scene tree.
- `_PhysicsProcess(double delta)` runs repeatedly at the physics rate.
- `_Draw()` draws the current placeholder character.

### Variables and Properties

`MoveSpeed` stores how fast the player travels. `_lastDirection` remembers which way the player last faced.

### Input

`Input.IsKeyPressed(Key.Left)` checks whether the left arrow is currently held. `Input.GetAxis` combines two actions into a value between -1 and 1.

### Vector2

A `Vector2` contains X and Y values. It can represent position, speed, or direction.

## Files to Explore

- `Scripts/World/PlayerController.cs`
- `Scripts/Core/Main.cs`
- `Scripts/Saving/SaveManager.cs`
- `Scripts/UI/PauseMenu.cs`

Do not worry about understanding every line at once. Follow one behavior from input to visible result.


## v0.5.1 observation

Open `PauseMenu.cs` and find the suggestion arrays, button callbacks, `MaxLength`, and `TextChanged` handlers. These demonstrate arrays, event subscriptions, methods, and UI state updates.
