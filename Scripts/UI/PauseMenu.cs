using Godot;

public partial class PauseMenu : CanvasLayer
{
    private Control _menuRoot = null!;
    private Control _dialogueRoot = null!;
    private Label _statusLabel = null!;
    private Label _dialogueLabel = null!;
    private Button _loadButton = null!;
    private CharacterBody2D _player = null!;
    private CreatureEditor _creatureEditor = null!;
    private double _playTimeSeconds;
    private bool _dialogueOpen;
    private ulong _dialogueOpenedFrame;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _player = GetNode<CharacterBody2D>("../Player");
        _creatureEditor = GetNode<CreatureEditor>("../CreatureEditor");
        BuildPauseMenu();
        BuildDialogueBox();
        _menuRoot.Visible = false;
        _dialogueRoot.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (!GetTree().Paused)
        {
            _playTimeSeconds += delta;
        }

        if (Input.IsActionJustPressed("pause_menu"))
        {
            if (_dialogueOpen)
            {
                CloseDialogue();
            }
            else
            {
                ToggleMenu();
            }
        }

        if (_dialogueOpen
            && Engine.GetProcessFrames() > _dialogueOpenedFrame
            && Input.IsActionJustPressed("interact"))
        {
            CloseDialogue();
        }
    }

    public void ShowDialogue(string text)
    {
        if (_menuRoot.Visible)
        {
            return;
        }

        _dialogueLabel.Text = text + "\n\n[E / Space] Continue";
        _dialogueRoot.Visible = true;
        _dialogueOpen = true;
        _dialogueOpenedFrame = Engine.GetProcessFrames();
        GetTree().Paused = true;
    }

    private void ToggleMenu()
    {
        bool opening = !_menuRoot.Visible;
        _menuRoot.Visible = opening;
        _loadButton.Disabled = !SaveManager.HasSave();
        _statusLabel.Text = opening ? "" : _statusLabel.Text;
        GetTree().Paused = opening;

        if (opening)
        {
            GetViewport().SetInputAsHandled();
        }
    }

    private void ResumeGame()
    {
        _menuRoot.Visible = false;
        GetTree().Paused = false;
    }

    private void SaveGame()
    {
        bool success = SaveManager.Save(_player.GlobalPosition, "Mossmere", _playTimeSeconds);
        _statusLabel.Text = success
            ? $"Game saved!  Position: {Mathf.Round(_player.GlobalPosition.X)}, {Mathf.Round(_player.GlobalPosition.Y)}"
            : "Save failed. Check the Godot Output panel.";
        _loadButton.Disabled = !SaveManager.HasSave();
    }

    private void LoadGame()
    {
        if (!SaveManager.TryLoad(out Vector2 position, out string location, out double playTime))
        {
            _statusLabel.Text = "No valid save file was found.";
            return;
        }

        _player.GlobalPosition = position;
        _playTimeSeconds = playTime;
        _statusLabel.Text = $"Loaded {location}.";
        ResumeGame();
    }

    private void OpenCreatureEditor()
    {
        _menuRoot.Visible = false;
        _creatureEditor.Open();
    }

    public void ReturnFromCreatureEditor()
    {
        _menuRoot.Visible = true;
        _loadButton.Disabled = !SaveManager.HasSave();
        _statusLabel.Text = "Returned from the creature editor.";
        GetTree().Paused = true;
    }

    private void ShowSettingsNotice()
    {
        _statusLabel.Text = "Settings controls arrive in a later milestone.";
    }

    private void QuitGame()
    {
        GetTree().Quit();
    }

    private void CloseDialogue()
    {
        _dialogueOpen = false;
        _dialogueRoot.Visible = false;
        GetTree().Paused = false;
    }

    private void BuildPauseMenu()
    {
        _menuRoot = new Control
        {
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        _menuRoot.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        AddChild(_menuRoot);

        ColorRect shade = new()
        {
            Color = new Color(0, 0, 0, 0.58f)
        };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _menuRoot.AddChild(shade);

        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);
        panel.Position = new Vector2(-180, -260);
        panel.Size = new Vector2(360, 520);
        _menuRoot.AddChild(panel);

        VBoxContainer box = new();
        box.AddThemeConstantOverride("separation", 12);
        panel.AddChild(box);

        Label title = new()
        {
            Text = "BRIGHTHOLLOW",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        title.AddThemeFontSizeOverride("font_size", 30);
        box.AddChild(title);

        Label subtitle = new()
        {
            Text = "Mossmere — Pause Menu",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        subtitle.AddThemeFontSizeOverride("font_size", 17);
        box.AddChild(subtitle);

        box.AddChild(CreateButton("Resume", ResumeGame));
        box.AddChild(CreateButton("Save Game", SaveGame));
        _loadButton = CreateButton("Load Game", LoadGame);
        box.AddChild(_loadButton);
        box.AddChild(CreateButton("Creature Editor", OpenCreatureEditor));
        box.AddChild(CreateButton("Settings", ShowSettingsNotice));
        box.AddChild(CreateButton("Quit to Desktop", QuitGame));

        _statusLabel = new()
        {
            Text = "",
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            HorizontalAlignment = HorizontalAlignment.Center,
            CustomMinimumSize = new Vector2(320, 60)
        };
        box.AddChild(_statusLabel);

        Label help = new()
        {
            Text = "Esc closes the menu",
            HorizontalAlignment = HorizontalAlignment.Center
        };
        box.AddChild(help);
    }

    private void BuildDialogueBox()
    {
        _dialogueRoot = new Control
        {
            MouseFilter = Control.MouseFilterEnum.Stop
        };
        _dialogueRoot.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        AddChild(_dialogueRoot);

        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        panel.OffsetLeft = 80;
        panel.OffsetTop = -170;
        panel.OffsetRight = -80;
        panel.OffsetBottom = -30;
        _dialogueRoot.AddChild(panel);

        _dialogueLabel = new Label
        {
            Text = "",
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            VerticalAlignment = VerticalAlignment.Center
        };
        _dialogueLabel.AddThemeFontSizeOverride("font_size", 22);
        panel.AddChild(_dialogueLabel);
    }

    private static Button CreateButton(string text, System.Action action)
    {
        Button button = new()
        {
            Text = text,
            CustomMinimumSize = new Vector2(320, 48)
        };
        button.AddThemeFontSizeOverride("font_size", 20);
        button.Pressed += action;
        return button;
    }
}
