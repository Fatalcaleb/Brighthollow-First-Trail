using Godot;

/// <summary>
/// Named destination point used by reusable world doors.
/// </summary>
public partial class WorldSpawnPoint : Marker2D
{
    public string SpawnId { get; private set; } = string.Empty;
    public Vector2 FacingDirection { get; private set; } = Vector2.Down;

    public void Configure(string spawnId, Vector2 position, Vector2 facingDirection)
    {
        SpawnId = spawnId;
        Position = position;
        FacingDirection = facingDirection;
        Name = $"Spawn_{spawnId}";
    }
}
