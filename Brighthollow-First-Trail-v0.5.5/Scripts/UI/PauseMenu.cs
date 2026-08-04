using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
    private Control _titleRoot = null!;
    private Control _setupRoot = null!;
    private Control _menuRoot = null!;
    private Control _dialogueRoot = null!;
    private Control _journalRoot = null!;
    private ColorRect _fade = null!;
    private Label _statusLabel = null!;
    private Label _dialogueLabel = null!;
    private Label _journalLabel = null!;
    private Label _locationLabel = null!;
    private Label _profileLabel = null!;
    private Label _titleMetadataLabel = null!;
    private Label _setupStatusLabel = null!;
    private Button _loadButton = null!;
    private Button _continueButton = null!;
    private LineEdit _playerNameInput = null!;
    private LineEdit _rivalNameInput = null!;
    private Label _playerNameCounter = null!;
    private Label _rivalNameCounter = null!;
    private OptionButton _appearanceSelect = null!;
    private ConfirmationDialog _overwriteDialog = null!;
    private Main _main = null!;
    private Label _instructions = null!;
    private double _playTimeSeconds;
    private bool _dialogueOpen;
    private bool _journalOpen;
    private bool _sessionStarted;
    private ulong _dialogueOpenedFrame;

    private static readonly string[] PlayerNameSuggestions =
    {
        "Avery", "Caleb", "Jordan", "Morgan", "Riley", "Skyler", "Taylor", "Quinn"
    };

    private static readonly string[] RivalNameSuggestions =
    {
        "Rowan", "Mara", "Ellis", "Sage", "Reese", "Emery", "Parker", "Casey"
    };

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        _main = GetNode<Main>("..");
        _instructions = GetNode<Label>("Instructions");

        BuildTitleScreen();
        BuildSetupScreen();
        BuildPauseMenu();
        BuildDialogueBox();
        BuildJournal();
        BuildFade();
        BuildOverwriteDialog();

        _setupRoot.Visible = false;
        _menuRoot.Visible = false;
        _dialogueRoot.Visible = false;
        _journalRoot.Visible = false;
        _instructions.Visible = false;
        RefreshTitleMetadata();
        UpdateLocation("Mossmere");
        GetTree().Paused = true;
    }

    public override void _Process(double delta)
    {
        if (_sessionStarted && !GetTree().Paused)
        {
            _playTimeSeconds += delta;
        }

        if (!_sessionStarted)
        {
            return;
        }

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
        if (_menuRoot.Visible || !_sessionStarted) return;
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

    private void RequestNewGame()
    {
        if (SaveManager.HasSave())
        {
            _overwriteDialog.PopupCentered();
            return;
        }

        OpenSetupScreen();
    }

    private void OpenSetupScreen()
    {
        _titleRoot.Visible = false;
        _setupRoot.Visible = true;
        _setupStatusLabel.Text = string.Empty;
        _playerNameInput.GrabFocus();
    }

    private void CancelSetup()
    {
        _setupRoot.Visible = false;
        _titleRoot.Visible = true;
        RefreshTitleMetadata();
    }

    private void ConfirmNewGame()
    {
        string playerName = _playerNameInput.Text.StripEdges();
        string rivalName = _rivalNameInput.Text.StripEdges();

        if (playerName.Length < 1)
        {
            _setupStatusLabel.Text = "Please enter a player name.";
            _playerNameInput.GrabFocus();
            return;
        }

        if (rivalName.Length < 1)
        {
            _setupStatusLabel.Text = "Please enter a rival name.";
            _rivalNameInput.GrabFocus();
            return;
        }

        PlayerProfileData profile = new()
        {
            PlayerName = playerName,
            RivalName = rivalName,
            AppearancePreset = _appearanceSelect.Selected
        };

        _main.BeginNewGame(profile);
        _playTimeSeconds = 0;
        BeginSession();
    }

    private void ContinueGame()
    {
        if (!SaveManager.TryLoad(out GameSaveData save))
        {
            RefreshTitleMetadata();
            return;
        }

        _main.ApplySaveData(save);
        _playTimeSeconds = save.PlayTimeSeconds;
        BeginSession();
    }

    private void BeginSession()
    {
        _sessionStarted = true;
        _titleRoot.Visible = false;
        _setupRoot.Visible = false;
        _instructions.Visible = true;
        _profileLabel.Text = $"Trainer: {_main.Profile.PlayerName}   Rival: {_main.Profile.RivalName}";
        UpdateLocation(_main.GetLocationDisplayName());
        GetTree().Paused = false;
    }

    private void RefreshTitleMetadata()
    {
        bool hasSave = SaveManager.TryReadMetadata(out GameSaveData metadata);
        _continueButton.Disabled = !hasSave;
        _titleMetadataLabel.Text = hasSave
            ? $"Continue as {metadata.Profile.PlayerName}\n{GetMapDisplayName(metadata.MapId)}  •  {FormatPlayTime(metadata.PlayTimeSeconds)}\nSaved: {metadata.SavedAt}"
            : "No save data found. Begin a new trail.";
    }

    private void ToggleMenu()
    {
        bool opening = !_menuRoot.Visible;
        _menuRoot.Visible = opening;
        _loadButton.Disabled = !SaveManager.HasSave();
        _profileLabel.Text = $"Trainer: {_main.Profile.PlayerName}   Rival: {_main.Profile.RivalName}";
        if (opening) _statusLabel.Text = string.Empty;
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

    private void BuildTitleScreen()
    {
        _titleRoot = FullScreenControl();
        AddChild(_titleRoot);
        ColorRect background = new() { Color = new Color("#14283b") };
        background.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _titleRoot.AddChild(background);

        PanelContainer panel = CenterPanel(_titleRoot, new Vector2(520, 440));
        VBoxContainer box = CreateVerticalBox(panel, 14);

        Label title = new() { Text = "BRIGHTHOLLOW", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 46);
        box.AddChild(title);
        Label subtitle = new() { Text = "FIRST TRAIL", HorizontalAlignment = HorizontalAlignment.Center };
        subtitle.AddThemeFontSizeOverride("font_size", 22);
        box.AddChild(subtitle);
        box.AddChild(new HSeparator());

        _continueButton = CreateButton("Continue", ContinueGame, 390, 48);
        box.AddChild(_continueButton);
        box.AddChild(CreateButton("New Game", RequestNewGame, 390, 48));
        box.AddChild(CreateButton("Quit", QuitGame, 390, 48));

        _titleMetadataLabel = new Label
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            CustomMinimumSize = new Vector2(420, 92)
        };
        box.AddChild(_titleMetadataLabel);
        box.AddChild(new Label { Text = BuildInfo.DisplayVersion, HorizontalAlignment = HorizontalAlignment.Center });
    }

    private void BuildSetupScreen()
    {
        _setupRoot = FullScreenControl();
        AddChild(_setupRoot);
        ColorRect background = new() { Color = new Color("#1d3445") };
        background.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _setupRoot.AddChild(background);

        PanelContainer panel = CenterPanel(_setupRoot, new Vector2(600, 500));
        VBoxContainer box = CreateVerticalBox(panel, 6);
        Label title = new() { Text = "CREATE YOUR TRAINER", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 30);
        box.AddChild(title);

        box.AddChild(new Label { Text = "Player Name" });
        _playerNameInput = new LineEdit { PlaceholderText = "Enter your own name", MaxLength = 16, Text = "Caleb" };
        _playerNameInput.TextChanged += _ => UpdateNameCounters();
        box.AddChild(_playerNameInput);
        HBoxContainer playerNameTools = new() { Alignment = BoxContainer.AlignmentMode.End };
        playerNameTools.AddThemeConstantOverride("separation", 10);
        playerNameTools.AddChild(CreateButton("Suggest Name", SuggestPlayerName, 150, 34));
        _playerNameCounter = new Label { Text = "5 / 16", VerticalAlignment = VerticalAlignment.Center };
        playerNameTools.AddChild(_playerNameCounter);
        box.AddChild(playerNameTools);

        box.AddChild(new Label { Text = "Rival Name" });
        _rivalNameInput = new LineEdit { PlaceholderText = "Enter a custom rival name", MaxLength = 16, Text = "Rowan" };
        _rivalNameInput.TextChanged += _ => UpdateNameCounters();
        box.AddChild(_rivalNameInput);
        HBoxContainer rivalNameTools = new() { Alignment = BoxContainer.AlignmentMode.End };
        rivalNameTools.AddThemeConstantOverride("separation", 10);
        rivalNameTools.AddChild(CreateButton("Suggest Name", SuggestRivalName, 150, 34));
        _rivalNameCounter = new Label { Text = "5 / 16", VerticalAlignment = VerticalAlignment.Center };
        rivalNameTools.AddChild(_rivalNameCounter);
        box.AddChild(rivalNameTools);

        box.AddChild(new Label { Text = "Appearance Preset" });
        _appearanceSelect = new OptionButton();
        _appearanceSelect.AddItem("Preset 1 — Blue Trail");
        _appearanceSelect.AddItem("Preset 2 — Rose Explorer");
        _appearanceSelect.AddItem("Preset 3 — Green Scout");
        _appearanceSelect.AddItem("Preset 4 — Violet Wanderer");
        box.AddChild(_appearanceSelect);

        Label note = new()
        {
            Text = "Appearance presets are temporary placeholders until original 32×32 sprite sheets are added.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        };
        box.AddChild(note);

        HBoxContainer buttons = new() { Alignment = BoxContainer.AlignmentMode.Center };
        buttons.AddThemeConstantOverride("separation", 12);
        buttons.AddChild(CreateButton("Back", CancelSetup, 180, 44));
        buttons.AddChild(CreateButton("Begin Adventure", ConfirmNewGame, 240, 44));
        box.AddChild(buttons);

        _setupStatusLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        box.AddChild(_setupStatusLabel);
        UpdateNameCounters();
    }


    private void SuggestPlayerName()
    {
        _playerNameInput.Text = PlayerNameSuggestions[GD.RandRange(0, PlayerNameSuggestions.Length - 1)];
        _playerNameInput.CaretColumn = _playerNameInput.Text.Length;
        UpdateNameCounters();
    }

    private void SuggestRivalName()
    {
        _rivalNameInput.Text = RivalNameSuggestions[GD.RandRange(0, RivalNameSuggestions.Length - 1)];
        _rivalNameInput.CaretColumn = _rivalNameInput.Text.Length;
        UpdateNameCounters();
    }

    private void UpdateNameCounters()
    {
        if (_playerNameCounter is not null)
        {
            _playerNameCounter.Text = $"{_playerNameInput.Text.Length} / 16";
        }

        if (_rivalNameCounter is not null)
        {
            _rivalNameCounter.Text = $"{_rivalNameInput.Text.Length} / 16";
        }
    }

    private void BuildPauseMenu()
    {
        _menuRoot = FullScreenControl();
        AddChild(_menuRoot);
        ColorRect shade = new() { Color = new Color(0, 0, 0, 0.58f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _menuRoot.AddChild(shade);

        PanelContainer panel = CenterPanel(_menuRoot, new Vector2(420, 510));
        VBoxContainer box = CreateVerticalBox(panel, 7);

        Label title = new() { Text = "BRIGHTHOLLOW", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        box.AddChild(title);

        _profileLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        box.AddChild(_profileLabel);
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

        _statusLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, HorizontalAlignment = HorizontalAlignment.Center, CustomMinimumSize = new Vector2(360, 32) };
        box.AddChild(_statusLabel);
        box.AddChild(new Label { Text = "Esc closes the menu", HorizontalAlignment = HorizontalAlignment.Center });
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
        PanelContainer panel = CenterPanel(_journalRoot, new Vector2(660, 420));
        VBoxContainer box = CreateVerticalBox(panel, 14);
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

    private void BuildOverwriteDialog()
    {
        _overwriteDialog = new ConfirmationDialog
        {
            Title = "Start New Game?",
            DialogText = "A save file already exists. Starting a new game will replace it the next time you save. Continue?",
            OkButtonText = "Continue"
        };
        _overwriteDialog.Confirmed += OpenSetupScreen;
        AddChild(_overwriteDialog);
    }

    private static Control FullScreenControl()
    {
        Control control = new() { MouseFilter = Control.MouseFilterEnum.Stop };
        control.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        return control;
    }

    private static PanelContainer CenterPanel(Control parent, Vector2 size)
    {
        PanelContainer panel = new();
        panel.SetAnchorsPreset(Control.LayoutPreset.Center);
        panel.Position = -size / 2;
        panel.Size = size;
        parent.AddChild(panel);
        return panel;
    }

    private static VBoxContainer CreateVerticalBox(Container parent, int separation)
    {
        VBoxContainer box = new();
        box.AddThemeConstantOverride("separation", separation);
        parent.AddChild(box);
        return box;
    }

    private static Button CreateButton(string text, Action action, float width = 360, float height = 40)
    {
        Button button = new() { Text = text, CustomMinimumSize = new Vector2(width, height) };
        button.AddThemeFontSizeOverride("font_size", 18);
        button.Pressed += action;
        return button;
    }

    private static string GetMapDisplayName(string mapId) => mapId switch
    {
        "player_house" => "Your House",
        "alder_lab" => "Professor Alder's Laboratory",
        _ => "Mossmere"
    };

    private static string FormatPlayTime(double seconds)
    {
        int totalMinutes = (int)(seconds / 60.0);
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;
        return $"{hours:00}:{minutes:00}";
    }
}
