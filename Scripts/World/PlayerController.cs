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
        Vector2 input = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (input.LengthSquared() > 0.0f)
        {
            _lastDirection = input.Normalized();
        }

        Velocity = input * MoveSpeed;
        MoveAndSlide();
        QueueRedraw();
    }

    public override void _Draw()
    {
        // Original placeholder trainer sprite built from primitive pixel-like shapes.
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
