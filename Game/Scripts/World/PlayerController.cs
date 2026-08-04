using Godot;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed { get; set; } = 190.0f;

    private Vector2 _lastDirection = Vector2.Down;
    private int _appearancePreset;
    private bool _movementEnabled = true;
    private uint _normalCollisionMask = 1;

    public override void _Ready()
    {
        _normalCollisionMask = CollisionMask == 0 ? 1u : CollisionMask;
        QueueRedraw();
    }

    public void SetWorldCollisionEnabled(bool enabled)
    {
        CollisionMask = enabled ? _normalCollisionMask : 0u;
    }

    public void SetAppearancePreset(int preset)
    {
        _appearancePreset = Mathf.Clamp(preset, 0, 3);
        QueueRedraw();
    }

    public void SetMovementEnabled(bool enabled)
    {
        _movementEnabled = enabled;
        if (!enabled) Velocity = Vector2.Zero;
    }

    public void SetFacingDirection(Vector2 direction)
    {
        if (direction.LengthSquared() > 0.0f)
        {
            _lastDirection = direction.Normalized();
            QueueRedraw();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_movementEnabled)
        {
            Velocity = Vector2.Zero;
            return;
        }

        float horizontal = Input.GetAxis("move_left", "move_right");
        float vertical = Input.GetAxis("move_up", "move_down");

        if (Input.IsKeyPressed(Key.Left)) horizontal = -1.0f;
        else if (Input.IsKeyPressed(Key.Right)) horizontal = 1.0f;

        if (Input.IsKeyPressed(Key.Up)) vertical = -1.0f;
        else if (Input.IsKeyPressed(Key.Down)) vertical = 1.0f;

        Vector2 input = new(horizontal, vertical);
        if (input.LengthSquared() > 1.0f) input = input.Normalized();
        if (input.LengthSquared() > 0.0f) _lastDirection = input;

        Velocity = input * MoveSpeed;
        MoveAndSlide();
        QueueRedraw();
    }

    public override void _Draw()
    {
        (Color hair, Color shirt, Color pants, Color skin) = _appearancePreset switch
        {
            1 => (new Color("#4b2e24"), new Color("#a04b68"), new Color("#344f73"), new Color("#d99b73")),
            2 => (new Color("#d0a34a"), new Color("#3d8b77"), new Color("#3c3c4f"), new Color("#f0c49a")),
            3 => (new Color("#1f2028"), new Color("#8b5eb8"), new Color("#263d66"), new Color("#8f5f47")),
            _ => (new Color("#263d66"), new Color("#4e79a7"), new Color("#303645"), new Color("#f0c49a"))
        };

        DrawCircle(new Vector2(0, -15), 10, skin);
        DrawRect(new Rect2(-12, -28, 24, 8), hair);
        DrawRect(new Rect2(-13, -5, 26, 25), new Color("#e8e5d8"));
        DrawRect(new Rect2(-13, 2, 26, 8), shirt);
        DrawRect(new Rect2(-11, 20, 8, 12), pants);
        DrawRect(new Rect2(3, 20, 8, 12), pants);

        Vector2 facing = _lastDirection * 5.0f;
        DrawCircle(new Vector2(-4, -16) + facing * 0.2f, 1.5f, Colors.Black);
        DrawCircle(new Vector2(4, -16) + facing * 0.2f, 1.5f, Colors.Black);
    }
}
