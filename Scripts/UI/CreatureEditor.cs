using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CreatureEditor : CanvasLayer
{
    private static readonly string[] Types =
    {
        "None", "Neutral", "Flame", "Tide", "Grove", "Volt", "Frost", "Stone", "Earth",
        "Metal", "Air", "Mind", "Shadow", "Spirit", "Venom", "Insect", "Martial", "Mystic", "Light"
    };

    private Control _root = null!;
    private OptionButton _creaturePicker = null!;
    private LineEdit _idField = null!;
    private LineEdit _nameField = null!;
    private OptionButton _primaryType = null!;
    private OptionButton _secondaryType = null!;
    private SpinBox _hp = null!;
    private SpinBox _attack = null!;
    private SpinBox _defense = null!;
    private SpinBox _specialAttack = null!;
    private SpinBox _specialDefense = null!;
    private SpinBox _speed = null!;
    private SpinBox _captureDifficulty = null!;
    private LineEdit _ability = null!;
    private LineEdit _traits = null!;
    private TextEdit _description = null!;
    private Label _status = null!;
    private List<CreatureDefinition> _creatures = new();
    private int _currentIndex;
    private bool _loadingFields;

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        BuildInterface();
        _root.Visible = false;
    }

    public void Open()
    {
        _creatures = CreatureDatabase.LoadAll();
        if (_creatures.Count == 0)
        {
            _creatures.Add(new CreatureDefinition());
        }

        _currentIndex = 0;
        RebuildPicker();
        LoadCurrentIntoFields();
        _status.Text = CreatureDatabase.HasOverride()
            ? "Editing custom override data."
            : "Editing a copy of the bundled creature database.";
        _root.Visible = true;
        GetTree().Paused = true;
    }

    private void Close()
    {
        ApplyFieldsToCurrent();
        _root.Visible = false;
        GetNode<PauseMenu>("../Interface").ReturnFromCreatureEditor();
    }

    private void PickCreature(long index)
    {
        if (_loadingFields)
        {
            return;
        }

        ApplyFieldsToCurrent();
        _currentIndex = Math.Clamp((int)index, 0, _creatures.Count - 1);
        LoadCurrentIntoFields();
    }

    private void AddCreature()
    {
        ApplyFieldsToCurrent();
        int number = _creatures.Count + 1;
        _creatures.Add(new CreatureDefinition
        {
            Id = $"creature_{number}",
            Name = $"Creature {number}"
        });
        _currentIndex = _creatures.Count - 1;
        RebuildPicker();
        LoadCurrentIntoFields();
        _status.Text = "New creature added. Save the override to keep it.";
    }

    private void DuplicateCreature()
    {
        ApplyFieldsToCurrent();
        CreatureDefinition source = _creatures[_currentIndex];
        CreatureDefinition copy = new()
        {
            Id = source.Id + "_copy",
            Name = source.Name + " Copy",
            PrimaryType = source.PrimaryType,
            SecondaryType = source.SecondaryType,
            BaseHp = source.BaseHp,
            BaseAttack = source.BaseAttack,
            BaseDefense = source.BaseDefense,
            BaseSpecialAttack = source.BaseSpecialAttack,
            BaseSpecialDefense = source.BaseSpecialDefense,
            BaseSpeed = source.BaseSpeed,
            CaptureDifficulty = source.CaptureDifficulty,
            Ability = source.Ability,
            PersonalityTraits = new List<string>(source.PersonalityTraits),
            Description = source.Description,
            LevelMoves = source.LevelMoves.Select(move => new LevelMoveDefinition { Level = move.Level, MoveId = move.MoveId }).ToList()
        };
        _creatures.Add(copy);
        _currentIndex = _creatures.Count - 1;
        RebuildPicker();
        LoadCurrentIntoFields();
    }

    private void DeleteCreature()
    {
        if (_creatures.Count <= 1)
        {
            _status.Text = "The database must contain at least one creature.";
            return;
        }

        _creatures.RemoveAt(_currentIndex);
        _currentIndex = Math.Clamp(_currentIndex, 0, _creatures.Count - 1);
        RebuildPicker();
        LoadCurrentIntoFields();
        _status.Text = "Creature removed from the working copy.";
    }

    private void SaveOverride()
    {
        ApplyFieldsToCurrent();
        _status.Text = CreatureDatabase.SaveOverride(_creatures, out string message)
            ? message
            : $"Save failed: {message}";
        RebuildPicker();
    }

    private void ResetToBundled()
    {
        if (!CreatureDatabase.DeleteOverride())
        {
            _status.Text = "Could not remove the custom override file.";
            return;
        }

        _creatures = CreatureDatabase.LoadAll();
        _currentIndex = 0;
        RebuildPicker();
        LoadCurrentIntoFields();
        _status.Text = "Custom override removed. Bundled data restored.";
    }

    private void RebuildPicker()
    {
        _loadingFields = true;
        _creaturePicker.Clear();
        for (int index = 0; index < _creatures.Count; index++)
        {
            _creaturePicker.AddItem($"{index + 1:000} — {_creatures[index].Name}");
        }
        _creaturePicker.Select(_currentIndex);
        _loadingFields = false;
    }

    private void LoadCurrentIntoFields()
    {
        _loadingFields = true;
        CreatureDefinition creature = _creatures[_currentIndex];
        _idField.Text = creature.Id;
        _nameField.Text = creature.Name;
        SelectOption(_primaryType, creature.PrimaryType);
        SelectOption(_secondaryType, creature.SecondaryType);
        _hp.Value = creature.BaseHp;
        _attack.Value = creature.BaseAttack;
        _defense.Value = creature.BaseDefense;
        _specialAttack.Value = creature.BaseSpecialAttack;
        _specialDefense.Value = creature.BaseSpecialDefense;
        _speed.Value = creature.BaseSpeed;
        _captureDifficulty.Value = creature.CaptureDifficulty;
        _ability.Text = creature.Ability;
        _traits.Text = string.Join(", ", creature.PersonalityTraits);
        _description.Text = creature.Description;
        _loadingFields = false;
    }

    private void ApplyFieldsToCurrent()
    {
        if (_loadingFields || _creatures.Count == 0)
        {
            return;
        }

        CreatureDefinition creature = _creatures[_currentIndex];
        creature.Id = CreatureDatabase.NormalizeId(_idField.Text);
        creature.Name = _nameField.Text.Trim();
        creature.PrimaryType = _primaryType.GetItemText(_primaryType.Selected);
        creature.SecondaryType = _secondaryType.GetItemText(_secondaryType.Selected);
        creature.BaseHp = (int)_hp.Value;
        creature.BaseAttack = (int)_attack.Value;
        creature.BaseDefense = (int)_defense.Value;
        creature.BaseSpecialAttack = (int)_specialAttack.Value;
        creature.BaseSpecialDefense = (int)_specialDefense.Value;
        creature.BaseSpeed = (int)_speed.Value;
        creature.CaptureDifficulty = (int)_captureDifficulty.Value;
        creature.Ability = _ability.Text.Trim();
        creature.PersonalityTraits = _traits.Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        creature.Description = _description.Text.Trim();
    }

    private void BuildInterface()
    {
        _root = new Control { MouseFilter = Control.MouseFilterEnum.Stop };
        _root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        AddChild(_root);

        ColorRect shade = new() { Color = new Color(0.03f, 0.05f, 0.09f, 0.97f) };
        shade.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _root.AddChild(shade);

        MarginContainer margin = new();
        margin.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        margin.AddThemeConstantOverride("margin_left", 28);
        margin.AddThemeConstantOverride("margin_right", 28);
        margin.AddThemeConstantOverride("margin_top", 24);
        margin.AddThemeConstantOverride("margin_bottom", 24);
        _root.AddChild(margin);

        VBoxContainer page = new();
        page.AddThemeConstantOverride("separation", 10);
        margin.AddChild(page);

        Label title = new() { Text = "CREATURE DATABASE EDITOR", HorizontalAlignment = HorizontalAlignment.Center };
        title.AddThemeFontSizeOverride("font_size", 28);
        page.AddChild(title);

        HBoxContainer toolbar = new();
        toolbar.AddThemeConstantOverride("separation", 8);
        page.AddChild(toolbar);

        _creaturePicker = new OptionButton { SizeFlagsHorizontal = Control.SizeFlags.ExpandFill };
        _creaturePicker.ItemSelected += PickCreature;
        toolbar.AddChild(_creaturePicker);
        toolbar.AddChild(CreateButton("New", AddCreature, 90));
        toolbar.AddChild(CreateButton("Duplicate", DuplicateCreature, 110));
        toolbar.AddChild(CreateButton("Delete", DeleteCreature, 90));

        HBoxContainer columns = new() { SizeFlagsVertical = Control.SizeFlags.ExpandFill };
        columns.AddThemeConstantOverride("separation", 18);
        page.AddChild(columns);

        VBoxContainer left = CreateColumn();
        VBoxContainer right = CreateColumn();
        columns.AddChild(left);
        columns.AddChild(right);

        _idField = AddLineField(left, "Internal ID");
        _nameField = AddLineField(left, "Display Name");
        _primaryType = AddTypeField(left, "Primary Type");
        _secondaryType = AddTypeField(left, "Secondary Type");
        _ability = AddLineField(left, "Passive Ability");
        _traits = AddLineField(left, "Traits (comma separated)");

        GridContainer stats = new() { Columns = 2 };
        stats.AddThemeConstantOverride("h_separation", 10);
        stats.AddThemeConstantOverride("v_separation", 6);
        right.AddChild(stats);
        _hp = AddStatField(stats, "HP");
        _attack = AddStatField(stats, "Attack");
        _defense = AddStatField(stats, "Defense");
        _specialAttack = AddStatField(stats, "Special Attack");
        _specialDefense = AddStatField(stats, "Special Defense");
        _speed = AddStatField(stats, "Speed");
        _captureDifficulty = AddStatField(stats, "Capture Difficulty");

        right.AddChild(new Label { Text = "Encyclopedia Description" });
        _description = new TextEdit
        {
            CustomMinimumSize = new Vector2(0, 130),
            SizeFlagsVertical = Control.SizeFlags.ExpandFill,
            WrapMode = TextEdit.LineWrappingMode.Boundary
        };
        right.AddChild(_description);

        _status = new Label
        {
            Text = "",
            AutowrapMode = TextServer.AutowrapMode.WordSmart,
            HorizontalAlignment = HorizontalAlignment.Center,
            CustomMinimumSize = new Vector2(0, 42)
        };
        page.AddChild(_status);

        HBoxContainer footer = new();
        footer.Alignment = BoxContainer.AlignmentMode.Center;
        footer.AddThemeConstantOverride("separation", 12);
        page.AddChild(footer);
        footer.AddChild(CreateButton("Save Custom Override", SaveOverride, 220));
        footer.AddChild(CreateButton("Reset to Bundled Data", ResetToBundled, 210));
        footer.AddChild(CreateButton("Close Editor", Close, 150));
    }

    private static VBoxContainer CreateColumn()
    {
        VBoxContainer column = new()
        {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            SizeFlagsVertical = Control.SizeFlags.ExpandFill
        };
        column.AddThemeConstantOverride("separation", 6);
        return column;
    }

    private static LineEdit AddLineField(VBoxContainer parent, string label)
    {
        parent.AddChild(new Label { Text = label });
        LineEdit field = new();
        parent.AddChild(field);
        return field;
    }

    private static OptionButton AddTypeField(VBoxContainer parent, string label)
    {
        parent.AddChild(new Label { Text = label });
        OptionButton option = new();
        foreach (string type in Types)
        {
            option.AddItem(type);
        }
        parent.AddChild(option);
        return option;
    }

    private static SpinBox AddStatField(GridContainer grid, string label)
    {
        grid.AddChild(new Label { Text = label });
        SpinBox spin = new() { MinValue = 1, MaxValue = 255, Step = 1, Value = 45 };
        grid.AddChild(spin);
        return spin;
    }

    private static Button CreateButton(string text, Action action, float width)
    {
        Button button = new() { Text = text, CustomMinimumSize = new Vector2(width, 42) };
        button.Pressed += action;
        return button;
    }

    private static void SelectOption(OptionButton option, string text)
    {
        for (int index = 0; index < option.ItemCount; index++)
        {
            if (string.Equals(option.GetItemText(index), text, StringComparison.OrdinalIgnoreCase))
            {
                option.Select(index);
                return;
            }
        }
        option.Select(0);
    }
}
