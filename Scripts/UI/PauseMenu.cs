using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    private Control _menuRoot = null!;
    private Control _dialogueRoot = null!;
    private Control _journalRoot = null!;
    private ColorRect _fade = null!;
    private Label _statusLabel = null!;
    private Label _dialogueLabel = null!;
    private Label _journalLabel = null!;
    private Label _locationLabel = null!;
    private Button _loadButton = null!;
    private Main _main = null!;
    private double _playTimeSeconds;
    private bool _dialogueOpen;
    private bool _journalOpen;
    private ulong _dialogueOpenedFrame;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _main = GetNode<Main>("..");
        BuildPauseMenu();
        BuildDialogueBox();
        BuildJournal();
        BuildFade();
        _menuRoot.Visible = false;
        _dialogueRoot.Visible = false;
        _journalRoot.Visible = false;
        UpdateLocation("Mossmere");
    }

    public override void _Process(double delta)
    {
        if (!GetTree().Paused) _playTimeSeconds += delta;

        if (Input.IsActionJustPressed("pause_menu"))
        {
            if (_dialogueOpen) CloseDialogue();
            else if (_journalOpen) CloseJournal();
            else ToggleMenu();
        }

        if (_dialogueOpen && Engine.GetProcessFrames() > _dialogueOpenedFrame && Input.IsActionJustPressed("interact"))
        {
            CloseDialogue();
        }
    }

    public void UpdateLocation(string displayName)
    {
        if (_locationLabel is not null) _locationLabel.Text = $"Location: {displayName}";
    }

    public void ShowDialogue(string text)
    {
        if (_menuRoot.Visible) return;
        _dialogueLabel.Text = text + "\n\n[E / Space] Continue";
        _dialogueRoot.Visible = true;
        _dialogueOpen = true;
        _dialogueOpenedFrame = Engine.GetProcessFrames();
        GetTree().Paused = true;
    }

    public void PlayTransition(Action midpoint)
    {
        GetTree().Paused = true;
        _fade.Visible = true;
        _fade.Modulate = new Color(1, 1, 1, 0);
        Tween tween = CreateTween();
        tween.SetPauseMode(Tween.TweenPauseMode.Process);
        tween.TweenProperty(_fade, "modulate:a", 1.0, 0.22);
        tween.TweenCallback(Callable.From(midpoint));
        tween.TweenInterval(0.06);
        tween.TweenProperty(_fade, "modulate:a", 0.0, 0.22);
        tween.TweenCallback(Callable.From(() =>
        {
            _fade.Visible = false;
            GetTree().Paused = false;
        }));
    }

    private void ToggleMenu()
    {
        bool opening = !_menuRoot.Visible;
        _menuRoot.Visible = opening;
        _loadButton.Disabled = !SaveManager.HasSave();
        _statusLabel.Text = opening ? "" : _statusLabel.Text;
        GetTree().Paused = opening;
        if (opening) GetViewport().SetInputAsHandled();
    }

    private void ResumeGame()
    {
        _menuRoot.Visible = false;
        GetTree().Paused = false;
    }

    private void SaveGame()
    {
        bool success = SaveManager.Save(_main.CreateSaveData(_playTimeSeconds));
        _statusLabel.Text = success ? $"Saved in {_main.GetLocationDisplayName()}." : "Save failed. Check the Godot Output panel.";
        _loadButton.Disabled = !SaveManager.HasSave();
    }

    private void LoadGame()
    {
        if (!SaveManager.TryLoad(out GameSaveData save))
        {
            _statusLabel.Text = "No valid save file was found.";
            return;
        }

        _main.ApplySaveData(save);
        _playTimeSeconds = save.PlayTimeSeconds;
        _statusLabel.Text = $"Loaded {_main.GetLocationDisplayName()}.";
        ResumeGame();
    }

    private void OpenJournal()
    {
        _menuRoot.Visible = false;
        _journalLabel.Text = _main.GetJournalText();
        _journalRoot.Visible = true;
        _journalOpen = true;
    }

    private void CloseJournal()
    {
        _journalRoot.Visible = false;
        _journalOpen = false;
        _menuRoot.Visible = true;
    }

    private void ShowSettingsNotice() => _statusLabel.Text = "Settings controls arrive in a later milestone.";
    private void QuitGame() => GetTree().Quit();

    private void CloseDialogue()
    {
        _dialogueOpen = false;
        _dialogueRoot.Visible = false;
        GetTree().Paused = false;
    }

    private void BuildPauseMenu()
    {
        _menuRoot = FullScreenControl();
        AddChild(_menuRoot);
        ColorRect shade = new() { Color = new Color(0, 0, 0, 0.58f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _menuRoot.AddChild(shade);

        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);
        panel.Position = new Vector2(-190, -235);
        panel.Size = new Vector2(380, 470);
        _menuRoot.AddChild(panel);

        VBoxContainer box = new();
        box.AddThemeConstantOverride("separation", 7);
        panel.AddChild(box);

        Label title = new() { Text = "BRIGHTHOLLOW", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        box.AddChild(title);

        _locationLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        _locationLabel.AddThemeFontSizeOverride("font_size", 16);
        box.AddChild(_locationLabel);

        box.AddChild(CreateButton("Resume", ResumeGame));
        box.AddChild(CreateButton("Journal", OpenJournal));
        box.AddChild(CreateButton("Save Game", SaveGame));
        _loadButton = CreateButton("Load Game", LoadGame);
        box.AddChild(_loadButton);
        box.AddChild(CreateButton("Settings", ShowSettingsNotice));
        box.AddChild(CreateButton("Quit to Desktop", QuitGame));

        _statusLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, HorizontalAlignment = HorizontalAlignment.Center, CustomMinimumSize = new Vector2(330, 34) };
        box.AddChild(_statusLabel);
        box.AddChild(new Label { Text = "Esc closes the menu", HorizontalAlignment = HorizontalAlignment.Center, CustomMinimumSize = new Vector2(330, 22) });
    }

    private void BuildDialogueBox()
    {
        _dialogueRoot = FullScreenControl();
        AddChild(_dialogueRoot);
        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.BottomWide);
        panel.OffsetLeft = 80; panel.OffsetTop = -170; panel.OffsetRight = -80; panel.OffsetBottom = -30;
        _dialogueRoot.AddChild(panel);
        _dialogueLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, VerticalAlignment = VerticalAlignment.Center };
        _dialogueLabel.AddThemeFontSizeOverride("font_size", 22);
        panel.AddChild(_dialogueLabel);
    }

    private void BuildJournal()
    {
        _journalRoot = FullScreenControl();
        AddChild(_journalRoot);
        ColorRect shade = new() { Color = new Color(0, 0, 0, 0.65f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _journalRoot.AddChild(shade);
        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);
        panel.Position = new Vector2(-330, -210);
        panel.Size = new Vector2(660, 420);
        _journalRoot.AddChild(panel);
        VBoxContainer box = new();
        box.AddThemeConstantOverride("separation", 14);
        panel.AddChild(box);
        Label title = new() { Text = "JOURNAL", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        box.AddChild(title);
        _journalLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(600, 275) };
        _journalLabel.AddThemeFontSizeOverride("font_size", 20);
        box.AddChild(_journalLabel);
        box.AddChild(CreateButton("Back", CloseJournal));
    }

    private void BuildFade()
    {
        _fade = new ColorRect { Color = Colors.Black, MouseFilter = Control.MouseFilterEnum.Stop };
        _fade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _fade.Visible = false;
        AddChild(_fade);
    }

    private static Control FullScreenControl()
    {
        Control control = new() { MouseFilter = Control.MouseFilterEnum.Stop };
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        return control;
    }

    private static Button CreateButton(string text, Action action)
    {
        Button button = new() { Text = text, CustomMinimumSize = new Vector2(330, 40) };
        button.AddThemeFontSizeOverride("font_size", 18);
        button.Pressed += action;
        return button;
    }
}
