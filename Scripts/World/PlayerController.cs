using Godot;

public partial class PlayerController : CharacterBody2D
{
    [Export] public float MoveSpeed { get; set; } = 190.0f;

    private Vector2 _lastDirection = Vector2.Down;

    public override void _Ready()
    {
        QueueRedraw();
    }

    public override void _PhysicsProcess(double delta)
    {
        // Read the configured actions first (WASD), then explicitly support the
        // arrow keys as a fallback. This avoids differences in imported InputMap
        // key codes between Godot versions and keyboard layouts.
        float horizontal = Input.GetAxis("move_left", "move_right");
        float vertical = Input.GetAxis("move_up", "move_down");

        if (Input.IsKeyPressed(Key.Left))
        {
            horizontal = -1.0f;
        }
        else if (Input.IsKeyPressed(Key.Right))
        {
            horizontal = 1.0f;
        }

        if (Input.IsKeyPressed(Key.Up))
        {
            vertical = -1.0f;
        }
        else if (Input.IsKeyPressed(Key.Down))
        {
            vertical = 1.0f;
        }

        Vector2 input = new(horizontal, vertical);
        if (input.LengthSquared() > 1.0f)
        {
            input = input.Normalized();
        }

        if (input.LengthSquared() > 0.0f)
        {
            _lastDirection = input;
        }

        Velocity = input * MoveSpeed;
        MoveAndSlide();
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(new Vector2(0, -15), 10, new Color("#f0c49a"));
        DrawRect(new Rect2(-12, -28, 24, 8), new Color("#263d66"));
        DrawRect(new Rect2(-13, -5, 26, 25), new Color("#e8e5d8"));
        DrawRect(new Rect2(-13, 2, 26, 8), new Color("#4e79a7"));
        DrawRect(new Rect2(-11, 20, 8, 12), new Color("#303645"));
        DrawRect(new Rect2(3, 20, 8, 12), new Color("#303645"));

        Vector2 facing = _lastDirection * 5.0f;
        DrawCircle(new Vector2(-4, -16) + facing * 0.2f, 1.5f, Colors.Black);
        DrawCircle(new Vector2(4, -16) + facing * 0.2f, 1.5f, Colors.Black);
    }
}
