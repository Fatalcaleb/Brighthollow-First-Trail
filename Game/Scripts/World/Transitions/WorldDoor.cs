using Godot;
using System;

/// <summary>
/// Reusable automatic map-transition trigger. The door owns its collision area
/// and reports a destination map and named spawn point to the world controller.
/// </summary>
public partial class WorldDoor : Area2D
{
    public event Action<WorldDoor>? TransitionRequested;

    public string DoorId { get; private set; } = string.Empty;
    public string DestinationMapId { get; private set; } = string.Empty;
    public string DestinationSpawnId { get; private set; } = string.Empty;
    public string RequiredStoryFlag { get; private set; } = string.Empty;
    public string LockedMessage { get; private set; } = "The way is currently blocked.";
    public bool Automatic { get; private set; } = true;

    private bool _armed = true;

    public void Configure(
        string doorId,
        Vector2 position,
        Vector2 triggerSize,
        string destinationMapId,
        string destinationSpawnId,
        bool automatic = true,
        string requiredStoryFlag = "",
        string lockedMessage = "The way is currently blocked.")
    {
        DoorId = doorId;
        Position = position;
        DestinationMapId = destinationMapId;
        DestinationSpawnId = destinationSpawnId;
        Automatic = automatic;
        RequiredStoryFlag = requiredStoryFlag;
        LockedMessage = lockedMessage;

        CollisionLayer = 0;
        CollisionMask = 1;
        Monitoring = true;
        Monitorable = true;

        CollisionShape2D collision = new()
        {
            Shape = new RectangleShape2D { Size = triggerSize }
        };
        AddChild(collision);
    }

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public void Disarm() => _armed = false;
    public void Arm() => _armed = true;

    private void OnBodyEntered(Node2D body)
    {
        if (!Automatic || !_armed || body is not PlayerController)
        {
            return;
        }

        _armed = false;
        TransitionRequested?.Invoke(this);
    }

    private void OnBodyExited(Node2D body)
    {
        if (body is PlayerController)
        {
            _armed = true;
        }
    }
}
