using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Node2D
{
    private const string Mossmere = "mossmere";
    private const string PlayerHouse = "player_house";
    private const string AlderLab = "alder_lab";

    private static readonly Vector2 MaraPosition = new(930, 500);
    private static readonly Vector2 GuardianPosition = new(520, 300);
    private static readonly Vector2 AlderPosition = new(530, 235);

    private readonly Dictionary<string, bool> _storyFlags = new();
    private readonly List<Node> _mapCollisionNodes = new();

    private CharacterBody2D _player = null!;
    private PauseMenu _interface = null!;
    private string _currentMapId = Mossmere;
    private PlayerProfileData _profile = new();
    private bool _sessionActive;
    private bool _transitionInProgress;
    private readonly List<WorldDoor> _mapDoors = new();
    private readonly Dictionary<string, WorldSpawnPoint> _spawnPoints = new();
    private PackedScene _doorScene = null!;
    private PackedScene _spawnPointScene = null!;

    public string CurrentMapId => _currentMapId;
    public IReadOnlyDictionary<string, bool> StoryFlags => _storyFlags;
    public PlayerProfileData Profile => _profile;
    public bool SessionActive => _sessionActive;

    public override void _Ready()
    {
        _doorScene = GD.Load<PackedScene>("res://Scenes/World/TransitionDoor.tscn");
        _spawnPointScene = GD.Load<PackedScene>("res://Scenes/World/SpawnPoint.tscn");
        _player = GetNode<CharacterBody2D>("Player");
        _interface = GetNode<PauseMenu>("Interface");
        LoadMap(Mossmere, new Vector2(480, 370));
        _player.Visible = false;

        var creatures = CreatureDatabase.LoadAll();
        GD.Print($"Loaded {creatures.Count} creature definitions.");
        GD.Print($"Brighthollow Milestone {BuildInfo.Version} started successfully.");

        Label instructions = GetNode<Label>("Interface/Instructions");
        instructions.Text = $"{BuildInfo.MilestoneLabel}\nMove: WASD / Arrows   Menu: Esc   Interact: E or Space";
    }

    public override void _Input(InputEvent @event)
    {
        if (!_sessionActive || !IsInteractPress(@event) || GetTree().Paused)
        {
            return;
        }

        if (TryHandleInteraction())
        {
            GetViewport().SetInputAsHandled();
        }
    }

    public GameSaveData CreateSaveData(double playTimeSeconds) => new()
    {
        PlayerPosition = _player.GlobalPosition,
        MapId = _currentMapId,
        PlayTimeSeconds = playTimeSeconds,
        StoryFlags = new Dictionary<string, bool>(_storyFlags),
        Profile = new PlayerProfileData
        {
            PlayerName = _profile.PlayerName,
            RivalName = _profile.RivalName,
            AppearancePreset = _profile.AppearancePreset
        }
    };

    public void BeginNewGame(PlayerProfileData profile)
    {
        _profile = new PlayerProfileData
        {
            PlayerName = profile.PlayerName,
            RivalName = profile.RivalName,
            AppearancePreset = profile.AppearancePreset
        };
        _storyFlags.Clear();
        SetFlag("arrived_in_mossmere", true);
        _sessionActive = true;
        _player.Visible = true;
        GetNode<PlayerController>("Player").SetAppearancePreset(_profile.AppearancePreset);
        LoadMapAtSpawn(PlayerHouse, "entrance");
    }

    public void ApplySaveData(GameSaveData save)
    {
        _profile = save.Profile;
        _sessionActive = true;
        _player.Visible = true;
        GetNode<PlayerController>("Player").SetAppearancePreset(_profile.AppearancePreset);
        _storyFlags.Clear();
        foreach ((string key, bool value) in save.StoryFlags)
        {
            _storyFlags[key] = value;
        }

        LoadMap(save.MapId, save.PlayerPosition);
    }

    public string GetLocationDisplayName() => _currentMapId switch
    {
        PlayerHouse => "Your House",
        AlderLab => "Professor Alder's Laboratory",
        _ => "Mossmere"
    };

    public string GetJournalText()
    {
        if (!HasFlag("spoke_to_guardian"))
        {
            return $"Mossmere Arrival\n\n{_profile.PlayerName}, you have settled into your new home in Mossmere. Talk to your guardian before exploring town.";
        }

        if (!HasFlag("visited_alder_lab"))
        {
            return "A Visit to the Professor\n\nYour guardian asked you to visit Professor Alder's laboratory in northern Mossmere.";
        }

        if (!HasFlag("met_professor_alder"))
        {
            return "Professor Alder's Laboratory\n\nYou reached the laboratory. Speak with Professor Alder near the research table.";
        }

        return $"A New Trail\n\nProfessor Alder is preparing three young creatures for {_profile.PlayerName}. {_profile.RivalName} is also expected at the laboratory soon.";
    }

    public override void _Draw()
    {
        switch (_currentMapId)
        {
            case PlayerHouse:
                DrawHouseInterior();
                break;
            case AlderLab:
                DrawLabInterior();
                break;
            default:
                DrawMossmere();
                break;
        }
    }

    private void OnDoorTransitionRequested(WorldDoor door)
    {
        if (_transitionInProgress)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(door.RequiredStoryFlag) && !HasFlag(door.RequiredStoryFlag))
        {
            _interface.ShowDialogue(door.LockedMessage);
            door.Arm();
            return;
        }

        if (door.DoorId == "mossmere_alder_lab")
        {
            SetFlag("visited_alder_lab", true);
        }

        BeginWorldTransition(door.DestinationMapId, door.DestinationSpawnId);
    }

    private void BeginWorldTransition(string mapId, string spawnId)
    {
        if (_transitionInProgress)
        {
            return;
        }

        _transitionInProgress = true;
        SetDoorsArmed(false);
        GetNode<PlayerController>("Player").SetMovementEnabled(false);

        _interface.PlayTransition(() =>
        {
            LoadMapAtSpawn(mapId, spawnId);
            GetNode<PlayerController>("Player").SetMovementEnabled(true);
            _transitionInProgress = false;
        });
    }

    private void SetDoorsArmed(bool armed)
    {
        foreach (WorldDoor door in _mapDoors)
        {
            if (!GodotObject.IsInstanceValid(door)) continue;
            if (armed) door.Arm(); else door.Disarm();
        }
    }

    private bool TryHandleInteraction()
    {
        Vector2 position = _player.GlobalPosition;

        if (_currentMapId == Mossmere)
        {
            if (position.DistanceTo(MaraPosition) <= 85)
            {
                _interface.ShowDialogue("Mara: Welcome to Mossmere! Professor Alder's lab is the blue building north of the crossroads.");
                return true;
            }

        }
        else if (_currentMapId == PlayerHouse)
        {
            if (position.DistanceTo(GuardianPosition) <= 85)
            {
                SetFlag("spoke_to_guardian", true);
                _interface.ShowDialogue($"Guardian: { _profile.PlayerName }, Professor Alder stopped by earlier. He asked you to visit the laboratory when you're ready.");
                return true;
            }

            if (position.DistanceTo(new Vector2(250, 210)) <= 70)
            {
                _interface.ShowDialogue("Your bed is neatly made. Resting will become available when party healing is added.");
                return true;
            }
        }
        else if (_currentMapId == AlderLab)
        {
            if (position.DistanceTo(AlderPosition) <= 90)
            {
                SetFlag("met_professor_alder", true);
                _interface.ShowDialogue("Professor Alder: Excellent timing! I'm preparing three young creatures for field study. Come back when the starter habitats are ready.");
                return true;
            }

            if (position.DistanceTo(new Vector2(300, 220)) <= 70)
            {
                _interface.ShowDialogue("Research Terminal: Habitat observations are synchronized with the regional creature archive.");
                return true;
            }
        }

        return false;
    }

    private void LoadMap(string mapId, Vector2 spawn)
    {
        PrepareMap(mapId);
        _player.GlobalPosition = spawn;
    }

    private void LoadMapAtSpawn(string mapId, string spawnId)
    {
        PrepareMap(mapId);

        if (!_spawnPoints.TryGetValue(spawnId, out WorldSpawnPoint? spawnPoint))
        {
            GD.PushWarning($"Spawn point '{spawnId}' was not found on map '{_currentMapId}'. Using map default.");
            _player.GlobalPosition = GetDefaultSpawn(_currentMapId);
            return;
        }

        _player.GlobalPosition = spawnPoint.GlobalPosition;
        GetNode<PlayerController>("Player").SetFacingDirection(spawnPoint.FacingDirection);
    }

    private void PrepareMap(string mapId)
    {
        _currentMapId = mapId is PlayerHouse or AlderLab ? mapId : Mossmere;
        ClearMapRuntimeNodes();
        BuildMapCollisions();
        BuildWorldTransitions();
        _interface.UpdateLocation(GetLocationDisplayName());
        QueueRedraw();
    }

    private static Vector2 GetDefaultSpawn(string mapId) => mapId switch
    {
        PlayerHouse or AlderLab => new Vector2(480, 410),
        _ => new Vector2(480, 370)
    };

    private void BuildMapCollisions()
    {
        if (_currentMapId == Mossmere)
        {
            CreateBoundary(new Vector2(800, -16), new Vector2(1600, 32));
            CreateBoundary(new Vector2(800, 1016), new Vector2(1600, 32));
            CreateBoundary(new Vector2(-16, 500), new Vector2(32, 1000));
            CreateBoundary(new Vector2(1616, 500), new Vector2(32, 1000));

            // HOME: solid building collision with a 48-pixel doorway opening.
            CreateBuildingCollisionWithDoor(new Rect2(170, 150, 220, 150), 48);
            // ALDER LAB: same reusable collision layout at a different size.
            CreateBuildingCollisionWithDoor(new Rect2(470, 120, 260, 180), 48);

            CreateBoundary(new Vector2(305, 735), new Vector2(250, 170));
            CreateBoundary(new Vector2(650, 735), new Vector2(300, 190));
            CreateBoundary(new Vector2(1220, 235), new Vector2(360, 230));
        }
        else
        {
            CreateBoundary(new Vector2(480, 75), new Vector2(760, 30));
            CreateBoundary(new Vector2(85, 280), new Vector2(30, 410));
            CreateBoundary(new Vector2(875, 280), new Vector2(30, 410));

            // Bottom wall split around the reusable 70-pixel exit opening.
            CreateBoundary(new Vector2(272.5f, 485), new Vector2(345, 30));
            CreateBoundary(new Vector2(687.5f, 485), new Vector2(345, 30));
        }
    }

    private void BuildWorldTransitions()
    {
        if (_currentMapId == Mossmere)
        {
            CreateSpawnPoint("start", new Vector2(480, 370), Vector2.Down);
            CreateSpawnPoint("home_exit", new Vector2(280, 350), Vector2.Down);
            CreateSpawnPoint("lab_exit", new Vector2(600, 350), Vector2.Down);

            CreateDoor("mossmere_home", new Vector2(280, 292), new Vector2(44, 16), PlayerHouse, "entrance");
            CreateDoor("mossmere_alder_lab", new Vector2(600, 292), new Vector2(44, 16), AlderLab, "entrance");
        }
        else if (_currentMapId == PlayerHouse)
        {
            CreateSpawnPoint("entrance", new Vector2(480, 410), Vector2.Up);
            CreateDoor("house_exit", new Vector2(480, 462), new Vector2(56, 16), Mossmere, "home_exit");
        }
        else if (_currentMapId == AlderLab)
        {
            CreateSpawnPoint("entrance", new Vector2(480, 410), Vector2.Up);
            CreateDoor("lab_exit", new Vector2(480, 462), new Vector2(56, 16), Mossmere, "lab_exit");
        }
    }

    private void CreateDoor(
        string doorId,
        Vector2 position,
        Vector2 triggerSize,
        string destinationMapId,
        string destinationSpawnId,
        string requiredStoryFlag = "",
        string lockedMessage = "The way is currently blocked.")
    {
        WorldDoor door = _doorScene.Instantiate<WorldDoor>();
        door.Configure(doorId, position, triggerSize, destinationMapId, destinationSpawnId, true, requiredStoryFlag, lockedMessage);
        door.TransitionRequested += OnDoorTransitionRequested;
        AddChild(door);
        _mapDoors.Add(door);
        _mapCollisionNodes.Add(door);
    }

    private void CreateSpawnPoint(string spawnId, Vector2 position, Vector2 facingDirection)
    {
        WorldSpawnPoint spawnPoint = _spawnPointScene.Instantiate<WorldSpawnPoint>();
        spawnPoint.Configure(spawnId, position, facingDirection);
        AddChild(spawnPoint);
        _spawnPoints[spawnId] = spawnPoint;
        _mapCollisionNodes.Add(spawnPoint);
    }

    private void CreateBuildingCollisionWithDoor(Rect2 body, float doorWidth)
    {
        float halfDoor = doorWidth / 2.0f;
        float doorLeft = body.Position.X + body.Size.X / 2.0f - halfDoor;
        float doorRight = doorLeft + doorWidth;
        float doorTop = body.End.Y - 55.0f;

        float leftWidth = doorLeft - body.Position.X;
        float rightWidth = body.End.X - doorRight;
        float upperHeight = doorTop - body.Position.Y;

        CreateBoundary(new Vector2(body.Position.X + leftWidth / 2.0f, body.Position.Y + body.Size.Y / 2.0f), new Vector2(leftWidth, body.Size.Y));
        CreateBoundary(new Vector2(doorRight + rightWidth / 2.0f, body.Position.Y + body.Size.Y / 2.0f), new Vector2(rightWidth, body.Size.Y));
        CreateBoundary(new Vector2(body.Position.X + body.Size.X / 2.0f, body.Position.Y + upperHeight / 2.0f), new Vector2(doorWidth, upperHeight));
    }

    private void ClearMapRuntimeNodes()
    {
        foreach (Node node in _mapCollisionNodes)
        {
            if (GodotObject.IsInstanceValid(node))
            {
                node.QueueFree();
            }
        }

        _mapCollisionNodes.Clear();
        _mapDoors.Clear();
        _spawnPoints.Clear();
    }

    private void CreateBoundary(Vector2 position, Vector2 size)
    {
        if (size.X <= 0 || size.Y <= 0) return;

        StaticBody2D body = new() { Position = position };
        CollisionShape2D collision = new();
        collision.Shape = new RectangleShape2D { Size = size };
        body.AddChild(collision);
        AddChild(body);
        _mapCollisionNodes.Add(body);
    }

    private void SetFlag(string id, bool value) => _storyFlags[id] = value;
    private bool HasFlag(string id) => _storyFlags.TryGetValue(id, out bool value) && value;
    private static bool IsInteractPress(InputEvent inputEvent)
    {
        if (inputEvent.IsActionPressed("interact")) return true;
        return inputEvent is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo
            && (keyEvent.Keycode == Key.E || keyEvent.Keycode == Key.Space);
    }

    private void DrawMossmere()
    {
        DrawRect(new Rect2(0, 0, 1600, 1000), new Color("#77b85a"));
        DrawRect(new Rect2(0, 430, 1600, 140), new Color("#d6b66f"));
        DrawRect(new Rect2(705, 0, 190, 1000), new Color("#d6b66f"));
        DrawRect(new Rect2(1040, 120, 360, 230), new Color("#3d91c9"));
        DrawRect(new Rect2(1060, 140, 320, 190), new Color("#58add8"));

        DrawBuilding(new Rect2(170, 150, 220, 150), new Color("#c85f4b"), "HOME");
        DrawBuilding(new Rect2(470, 120, 260, 180), new Color("#4a83b8"), "ALDER LAB");
        DrawBuilding(new Rect2(180, 650, 250, 170), new Color("#d8983d"), "SHOP");
        DrawBuilding(new Rect2(500, 640, 300, 190), new Color("#cf5f6f"), "CLINIC");

        for (int x = 40; x < 1560; x += 80) { DrawTree(new Vector2(x, 55)); DrawTree(new Vector2(x, 930)); }
        for (int y = 130; y < 900; y += 90) { DrawTree(new Vector2(55, y)); DrawTree(new Vector2(1530, y)); }

        DrawNpc(MaraPosition, new Color("#7a4fa3"));
        DrawString(ThemeDB.FallbackFont, MaraPosition + new Vector2(-54, -45), "MARA", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawString(ThemeDB.FallbackFont, new Vector2(140, 355), "Walk into a doorway", HorizontalAlignment.Left, -1, 18, Colors.White);
    }

    private void DrawHouseInterior()
    {
        DrawInteriorFloor(new Color("#d9c09b"), "YOUR HOUSE");
        DrawRect(new Rect2(170, 150, 170, 105), new Color("#7187a8"));
        DrawRect(new Rect2(185, 165, 140, 72), new Color("#d9e2f1"));
        DrawString(ThemeDB.FallbackFont, new Vector2(205, 205), "BED", HorizontalAlignment.Left, -1, 20, Colors.Black);
        DrawRect(new Rect2(650, 145, 120, 75), new Color("#4c5668"));
        DrawString(ThemeDB.FallbackFont, new Vector2(685, 188), "TV", HorizontalAlignment.Left, -1, 20, Colors.White);
        DrawNpc(GuardianPosition, new Color("#3b8070"));
        DrawString(ThemeDB.FallbackFont, GuardianPosition + new Vector2(-55, -45), "GUARDIAN", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawDoor(new Vector2(480, 450));
    }

    private void DrawLabInterior()
    {
        DrawInteriorFloor(new Color("#b9d4db"), "PROFESSOR ALDER'S LAB");
        DrawRect(new Rect2(195, 150, 210, 95), new Color("#657786"));
        DrawString(ThemeDB.FallbackFont, new Vector2(235, 205), "RESEARCH TERMINAL", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawRect(new Rect2(460, 145, 250, 90), new Color("#795d45"));
        DrawString(ThemeDB.FallbackFont, new Vector2(535, 195), "HABITAT TABLE", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawNpc(AlderPosition, new Color("#e8e8e8"));
        DrawString(ThemeDB.FallbackFont, AlderPosition + new Vector2(-82, -45), "PROFESSOR ALDER", HorizontalAlignment.Left, -1, 18, Colors.White);
        DrawDoor(new Vector2(480, 450));
    }

    private void DrawInteriorFloor(Color floor, string title)
    {
        DrawRect(new Rect2(100, 90, 760, 380), floor);
        DrawRect(new Rect2(100, 90, 760, 28), floor.Darkened(0.35f));
        DrawString(ThemeDB.FallbackFont, new Vector2(130, 112), title, HorizontalAlignment.Left, -1, 22, Colors.White);
    }

    private void DrawDoor(Vector2 center)
    {
        DrawRect(new Rect2(center - new Vector2(35, 20), new Vector2(70, 40)), new Color("#68472f"));
        DrawString(ThemeDB.FallbackFont, center + new Vector2(-20, 5), "EXIT", HorizontalAlignment.Left, -1, 16, Colors.White);
    }

    private void DrawNpc(Vector2 position, Color shirt)
    {
        DrawCircle(position + new Vector2(0, -14), 10, new Color("#e8b98d"));
        DrawRect(new Rect2(position + new Vector2(-12, -4), new Vector2(24, 28)), shirt);
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
}
