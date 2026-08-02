using Godot;

public partial class Main : Node2D
{
    private static readonly Vector2 WorldSize = new(1600, 1000);
    private static readonly Vector2 GuideNpcPosition = new(930, 500);

    private CharacterBody2D _player = null!;
    private PauseMenu _interface = null!;

    public override void _Ready()
    {
        _player = GetNode<CharacterBody2D>("Player");
        _interface = GetNode<PauseMenu>("Interface");
        CreateWorldBoundaries();
        QueueRedraw();
        GD.Print("Brighthollow Milestone 0.2.1 started successfully.");
    }

    public override void _Input(InputEvent @event)
    {
        if (IsInteractPress(@event) && !GetTree().Paused)
        {
            if (_player.GlobalPosition.DistanceTo(GuideNpcPosition) <= 85.0f)
            {
                _interface.ShowDialogue("Mara: Welcome to Mossmere! Press Esc whenever you need to save your journey.");
                GetViewport().SetInputAsHandled();
            }
        }
    }

    private static bool IsInteractPress(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("interact"))
        {
            return true;
        }

        return inputEvent is InputEventKey keyEvent
            && keyEvent.Pressed
            && !keyEvent.Echo
            && (keyEvent.Keycode == Key.E || keyEvent.Keycode == Key.Space);
    }

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, WorldSize), new Color("#77b85a"));
        DrawRect(new Rect2(0, 430, WorldSize.X, 140), new Color("#d6b66f"));
        DrawRect(new Rect2(705, 0, 190, WorldSize.Y), new Color("#d6b66f"));

        DrawRect(new Rect2(1040, 120, 360, 230), new Color("#3d91c9"));
        DrawRect(new Rect2(1060, 140, 320, 190), new Color("#58add8"));

        DrawBuilding(new Rect2(170, 150, 220, 150), new Color("#c85f4b"), "HOME");
        DrawBuilding(new Rect2(470, 120, 260, 180), new Color("#4a83b8"), "LAB");
        DrawBuilding(new Rect2(180, 650, 250, 170), new Color("#d8983d"), "SHOP");
        DrawBuilding(new Rect2(500, 640, 300, 190), new Color("#cf5f6f"), "CLINIC");

        for (int x = 40; x < 1560; x += 80)
        {
            DrawTree(new Vector2(x, 55));
            DrawTree(new Vector2(x, 930));
        }

        for (int y = 130; y < 900; y += 90)
        {
            DrawTree(new Vector2(55, y));
            DrawTree(new Vector2(1530, y));
        }

        DrawNpc(GuideNpcPosition);
        DrawString(ThemeDB.FallbackFont, GuideNpcPosition + new Vector2(-54, -45), "MARA", HorizontalAlignment.Left, -1, 18, Colors.White);
    }

    private void DrawNpc(Vector2 position)
    {
        DrawCircle(position + new Vector2(0, -14), 10, new Color("#e8b98d"));
        DrawRect(new Rect2(position + new Vector2(-12, -4), new Vector2(24, 28)), new Color("#7a4fa3"));
        DrawRect(new Rect2(position + new Vector2(-10, 24), new Vector2(7, 12)), new Color("#303645"));
        DrawRect(new Rect2(position + new Vector2(3, 24), new Vector2(7, 12)), new Color("#303645"));
    }

    private void DrawBuilding(Rect2 body, Color wallColor, string label)
    {
        Rect2 roof = new(body.Position - new Vector2(12, 35), body.Size + new Vector2(24, 45));
        DrawRect(roof, wallColor.Darkened(0.25f));
        DrawRect(body, wallColor);
        DrawRect(new Rect2(body.Position + new Vector2(body.Size.X / 2 - 24, body.Size.Y - 55), new Vector2(48, 55)), new Color("#5c3c2a"));
        DrawString(ThemeDB.FallbackFont, body.Position + new Vector2(18, 34), label, HorizontalAlignment.Left, -1, 24, Colors.White);
    }

    private void DrawTree(Vector2 position)
    {
        DrawRect(new Rect2(position + new Vector2(-7, 13), new Vector2(14, 25)), new Color("#76502e"));
        DrawCircle(position, 27, new Color("#2e7b45"));
        DrawCircle(position + new Vector2(-12, 7), 18, new Color("#3d9554"));
    }

    private void CreateWorldBoundaries()
    {
        CreateBoundary(new Vector2(WorldSize.X / 2, -16), new Vector2(WorldSize.X, 32));
        CreateBoundary(new Vector2(WorldSize.X / 2, WorldSize.Y + 16), new Vector2(WorldSize.X, 32));
        CreateBoundary(new Vector2(-16, WorldSize.Y / 2), new Vector2(32, WorldSize.Y));
        CreateBoundary(new Vector2(WorldSize.X + 16, WorldSize.Y / 2), new Vector2(32, WorldSize.Y));
        CreateBoundary(new Vector2(280, 225), new Vector2(220, 150));
        CreateBoundary(new Vector2(600, 210), new Vector2(260, 180));
        CreateBoundary(new Vector2(305, 735), new Vector2(250, 170));
        CreateBoundary(new Vector2(650, 735), new Vector2(300, 190));
        CreateBoundary(new Vector2(1220, 235), new Vector2(360, 230));
    }

    private void CreateBoundary(Vector2 position, Vector2 size)
    {
        StaticBody2D body = new();
        CollisionShape2D collision = new();
        RectangleShape2D shape = new() { Size = size };
        collision.Shape = shape;
        body.Position = position;
        body.AddChild(collision);
        AddChild(body);
    }
}
