using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PauseMenu : CanvasLayer
{
    private Control _titleRoot = null!;
    private Control _setupRoot = null!;
    private Control _menuRoot = null!;
    private Control _dialogueRoot = null!;
    private Control _journalRoot = null!;
    private Control _partyRoot = null!;
    private Control _starterRoot = null!;
    private Control _debugRoot = null!;
    private ColorRect _fade = null!;
    private Label _statusLabel = null!;
    private Label _dialogueLabel = null!;
    private Label _journalLabel = null!;
    private Label _partyLabel = null!;
    private Label _starterDetailsLabel = null!;
    private Label _starterStatusLabel = null!;
    private Label _debugLabel = null!;
    private Label _debugStatusLabel = null!;
    private Button _collisionToggleButton = null!;
    private string _debugText = string.Empty;
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
    private bool _partyOpen;
    private bool _starterOpen;
    private bool _debugOpen;
    private string _selectedStarterId = string.Empty;
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
        BuildPartyScreen();
        BuildStarterSelection();
        BuildDebugScreen();
        BuildFade();
        BuildOverwriteDialog();

        _setupRoot.Visible = false;
        _menuRoot.Visible = false;
        _dialogueRoot.Visible = false;
        _journalRoot.Visible = false;
        _partyRoot.Visible = false;
        _starterRoot.Visible = false;
        _debugRoot.Visible = false;
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

        if (Input.IsKeyPressed(Key.F3) && !_debugOpen)
        {
            OpenDebugScreen();
            return;
        }

        if (Input.IsActionJustPressed("pause_menu"))
        {
            if (_dialogueOpen) CloseDialogue();
            else if (_debugOpen) CloseDebugScreen();
            else if (_starterOpen) CloseStarterSelection();
            else if (_partyOpen) CloseParty();
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
        if (_menuRoot.Visible || _starterOpen || !_sessionStarted) return;
        _dialogueLabel.Text = text + "\n\n[E / Space] Continue";
        _dialogueRoot.Visible = true;
        _dialogueOpen = true;
        _dialogueOpenedFrame = Engine.GetProcessFrames();
        GetTree().Paused = true;
    }

    public void OpenStarterSelection(IReadOnlyList<CreatureDefinition> starters)
    {
        if (starters.Count == 0 || _starterOpen) return;
        _selectedStarterId = starters[0].Id;
        UpdateStarterDetails(_selectedStarterId);
        _starterStatusLabel.Text = "Inspect all three companions, then confirm your choice.";
        _starterRoot.Visible = true;
        _starterOpen = true;
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

    private void OpenParty()
    {
        _menuRoot.Visible = false;
        _partyLabel.Text = BuildPartyText();
        _partyRoot.Visible = true;
        _partyOpen = true;
    }

    private void CloseParty()
    {
        _partyRoot.Visible = false;
        _partyOpen = false;
        _menuRoot.Visible = true;
    }

    private string BuildPartyText()
    {
        if (_main.Party.Count == 0) return "Your party is empty. Professor Alder is preparing three possible companions.";
        List<string> lines = new();
        for (int index = 0; index < _main.Party.Count; index++)
        {
            CreatureInstanceData instance = _main.Party[index];
            CreatureDefinition? definition = _main.FindCreature(instance.SpeciesId);
            string name = string.IsNullOrWhiteSpace(instance.Nickname) ? definition?.Name ?? instance.SpeciesId : instance.Nickname;
            string traits = definition is null ? "Unknown" : string.Join(", ", definition.PersonalityTraits);
            string moves = instance.MoveIds.Count == 0 ? "None" : string.Join(", ", instance.MoveIds);
            lines.Add($"{index + 1}. {name}  Lv.{instance.Level}\nHP {instance.CurrentHp} / {_main.GetMaxHp(instance)}\nType: {definition?.PrimaryType ?? "Unknown"}\nAbility: {definition?.Ability ?? "Unknown"}\nTraits: {traits}\nMoves: {moves}");
        }
        return string.Join("\n\n", lines);
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

        PanelContainer panel = CenterPanel(_menuRoot, new Vector2(420, 560));
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
        box.AddChild(CreateButton("Party", OpenParty));
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

    private void BuildPartyScreen()
    {
        _partyRoot = FullScreenControl();
        AddChild(_partyRoot);
        ColorRect shade = new() { Color = new Color(0, 0, 0, 0.70f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _partyRoot.AddChild(shade);
        PanelContainer panel = CenterPanel(_partyRoot, new Vector2(700, 500));
        VBoxContainer box = CreateVerticalBox(panel, 12);
        Label title = new() { Text = "PARTY", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        box.AddChild(title);
        ScrollContainer scroll = new() { CustomMinimumSize = new Vector2(640, 350) };
        _partyLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(610, 340) };
        _partyLabel.AddThemeFontSizeOverride("font_size", 18);
        scroll.AddChild(_partyLabel);
        box.AddChild(scroll);
        box.AddChild(CreateButton("Back", CloseParty));
    }

    private void BuildStarterSelection()
    {
        _starterRoot = FullScreenControl();
        AddChild(_starterRoot);
        ColorRect background = new() { Color = new Color("#173248") };
        background.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _starterRoot.AddChild(background);
        PanelContainer panel = CenterPanel(_starterRoot, new Vector2(780, 560));
        VBoxContainer box = CreateVerticalBox(panel, 12);
        Label title = new() { Text = "CHOOSE YOUR FIRST COMPANION", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        box.AddChild(title);
        box.AddChild(new Label { Text = "Professor Alder: Each one has a different temperament and strength. Take your time.", HorizontalAlignment = HorizontalAlignment.Center, AutowrapMode = TextServer.AutowrapMode.WordSmart });
        HBoxContainer choices = new() { Alignment = BoxContainer.AlignmentMode.Center };
        choices.AddThemeConstantOverride("separation", 12);
        choices.AddChild(CreateButton("Spriglet\nGrove", () => SelectStarter("spriglet"), 210, 72));
        choices.AddChild(CreateButton("Cindercub\nFlame", () => SelectStarter("cindercub"), 210, 72));
        choices.AddChild(CreateButton("Ripplefin\nTide", () => SelectStarter("ripplefin"), 210, 72));
        box.AddChild(choices);
        _starterDetailsLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(700, 220) };
        _starterDetailsLabel.AddThemeFontSizeOverride("font_size", 19);
        box.AddChild(_starterDetailsLabel);
        _starterStatusLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center, AutowrapMode = TextServer.AutowrapMode.WordSmart };
        box.AddChild(_starterStatusLabel);
        HBoxContainer buttons = new() { Alignment = BoxContainer.AlignmentMode.Center };
        buttons.AddThemeConstantOverride("separation", 12);
        buttons.AddChild(CreateButton("Not Yet", CloseStarterSelection, 180, 42));
        buttons.AddChild(CreateButton("Choose This Companion", ConfirmStarter, 260, 42));
        box.AddChild(buttons);
    }

    private void SelectStarter(string speciesId)
    {
        _selectedStarterId = speciesId;
        UpdateStarterDetails(speciesId);
        _starterStatusLabel.Text = "Selected for review. Confirm when you are sure.";
    }

    private void UpdateStarterDetails(string speciesId)
    {
        CreatureDefinition? creature = _main.FindCreature(speciesId);
        if (creature is null)
        {
            _starterDetailsLabel.Text = "Creature data could not be loaded.";
            return;
        }
        string traits = string.Join(", ", creature.PersonalityTraits);
        _starterDetailsLabel.Text = $"{creature.Name} — {creature.PrimaryType}\n\n{creature.Description}\n\nAbility: {creature.Ability}\nTraits: {traits}\nBase Stats: HP {creature.BaseHp}  ATK {creature.BaseAttack}  DEF {creature.BaseDefense}  SP.ATK {creature.BaseSpecialAttack}  SP.DEF {creature.BaseSpecialDefense}  SPD {creature.BaseSpeed}";
    }

    private void ConfirmStarter()
    {
        CreatureDefinition? selected = _main.FindCreature(_selectedStarterId);
        if (selected is null || !_main.ChooseStarter(_selectedStarterId))
        {
            _starterStatusLabel.Text = "The selection could not be completed.";
            return;
        }
        CreatureDefinition? rival = _main.FindCreature(_main.RivalStarterSpeciesId);
        _starterRoot.Visible = false;
        _starterOpen = false;
        GetTree().Paused = false;
        ShowDialogue($"Professor Alder: {selected.Name} has chosen to travel with you! {_main.Profile.RivalName} will study alongside {rival?.Name ?? "another companion"}. You can now view your partner from the Party menu.");
    }

    private void CloseStarterSelection()
    {
        _starterRoot.Visible = false;
        _starterOpen = false;
        GetTree().Paused = false;
    }

    private void BuildDebugScreen()
    {
        _debugRoot = FullScreenControl();
        AddChild(_debugRoot);
        ColorRect shade = new() { Color = new Color(0, 0, 0, 0.82f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _debugRoot.AddChild(shade);
        PanelContainer panel = CenterPanel(_debugRoot, new Vector2(720, 500));
        VBoxContainer box = CreateVerticalBox(panel, 12);
        Label title = new() { Text = "DEVELOPER DEBUG INFORMATION", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 26);
        box.AddChild(title);
        ScrollContainer scroll = new() { CustomMinimumSize = new Vector2(660, 360) };
        _debugLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, CustomMinimumSize = new Vector2(630, 340) };
        _debugLabel.AddThemeFontSizeOverride("font_size", 17);
        scroll.AddChild(_debugLabel);
        box.AddChild(scroll);
        HBoxContainer actions = new() { Alignment = BoxContainer.AlignmentMode.Center };
        actions.AddThemeConstantOverride("separation", 10);
        actions.AddChild(CreateButton("Copy All Debug Data", CopyDebugData, 220, 42));
        _collisionToggleButton = CreateButton("Collision: ON", ToggleCollision, 180, 42);
        actions.AddChild(_collisionToggleButton);
        actions.AddChild(CreateButton("Close", CloseDebugScreen, 150, 42));
        box.AddChild(actions);
        _debugStatusLabel = new Label { HorizontalAlignment = HorizontalAlignment.Center };
        _debugStatusLabel.AddThemeColorOverride("font_color", new Color("#9ed6a3"));
        box.AddChild(_debugStatusLabel);
    }

    private void OpenDebugScreen()
    {
        if (!_sessionStarted) return;
        _menuRoot.Visible = false;
        CreatureDefinition? playerStarter = _main.Party.Count > 0 ? _main.FindCreature(_main.Party[0].SpeciesId) : null;
        CreatureDefinition? rivalStarter = _main.FindCreature(_main.RivalStarterSpeciesId);
        string flags = _main.StoryFlags.Count == 0
            ? "None"
            : string.Join("\n", _main.StoryFlags.Where(pair => pair.Value).Select(pair => $"• {pair.Key}"));
        _debugText = $"Game: Brighthollow: First Trail\nVersion: {BuildInfo.DisplayVersion}\nDebug Build: Yes\nCollision: {(_main.CollisionEnabled ? "ON" : "OFF")}\n\nCurrent Map: {_main.CurrentMapId}\nPlayer Coordinates: X={_main.PlayerPosition.X:0.0}, Y={_main.PlayerPosition.Y:0.0}\nPlayer: {_main.Profile.PlayerName}\nRival: {_main.Profile.RivalName}\nPlayer Starter: {playerStarter?.Name ?? "None"} [{playerStarter?.Id ?? "—"}]\nRival Starter: {rivalStarter?.Name ?? "None"} [{rivalStarter?.Id ?? "—"}]\nParty Count: {_main.Party.Count}\n\nStory Flags\n{flags}";
        _debugLabel.Text = _debugText;
        _collisionToggleButton.Text = $"Collision: {(_main.CollisionEnabled ? "ON" : "OFF")}";
        _debugStatusLabel.Text = string.Empty;
        _debugRoot.Visible = true;
        _debugOpen = true;
        GetTree().Paused = true;
    }

    private void CopyDebugData()
    {
        DisplayServer.ClipboardSet(_debugText);
        _debugStatusLabel.Text = "Debug data copied to clipboard.";
    }

    private void ToggleCollision()
    {
        _main.ToggleCollision();
        _collisionToggleButton.Text = $"Collision: {(_main.CollisionEnabled ? "ON" : "OFF")}";
        _debugStatusLabel.Text = $"World collision turned {(_main.CollisionEnabled ? "on" : "off")}.";
        int collisionLineStart = _debugText.IndexOf("Collision: ", StringComparison.Ordinal);
        if (collisionLineStart >= 0)
        {
            int collisionLineEnd = _debugText.IndexOf('\n', collisionLineStart);
            string currentLine = collisionLineEnd >= 0
                ? _debugText.Substring(collisionLineStart, collisionLineEnd - collisionLineStart)
                : _debugText.Substring(collisionLineStart);
            _debugText = _debugText.Replace(currentLine, $"Collision: {(_main.CollisionEnabled ? "ON" : "OFF")}");
            _debugLabel.Text = _debugText;
        }
    }

    private void CloseDebugScreen()
    {
        _debugRoot.Visible = false;
        _debugOpen = false;
        GetTree().Paused = false;
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
