using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public static class CreatureDatabase
{
    private const string BundledPath = "res://Data/Creatures/creatures.json";
    private const string OverridePath = "user://creatures.custom.json";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    public static List<CreatureDefinition> LoadAll()
    {
        string path = FileAccess.FileExists(OverridePath) ? OverridePath : BundledPath;
        using FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        if (file is null)
        {
            GD.PushError($"Could not open creature database at {path}: {FileAccess.GetOpenError()}");
            return new List<CreatureDefinition>();
        }

        try
        {
            List<CreatureDefinition>? creatures = JsonSerializer.Deserialize<List<CreatureDefinition>>(file.GetAsText(), JsonOptions);
            creatures ??= new List<CreatureDefinition>();
            Validate(creatures);
            return creatures;
        }
        catch (Exception exception)
        {
            GD.PushError($"Creature database JSON could not be read: {exception.Message}");
            return new List<CreatureDefinition>();
        }
    }

    public static bool SaveOverride(IReadOnlyCollection<CreatureDefinition> creatures, out string message)
    {
        try
        {
            List<CreatureDefinition> list = creatures.ToList();
            Validate(list);
            string json = JsonSerializer.Serialize(list, JsonOptions);
            using FileAccess file = FileAccess.Open(OverridePath, FileAccess.ModeFlags.Write);
            if (file is null)
            {
                message = $"Could not open override file: {FileAccess.GetOpenError()}";
                return false;
            }

            file.StoreString(json);
            message = $"Saved {list.Count} creatures to {ProjectSettings.GlobalizePath(OverridePath)}";
            return true;
        }
        catch (Exception exception)
        {
            message = exception.Message;
            GD.PushError($"Creature override save failed: {exception}");
            return false;
        }
    }

    public static bool DeleteOverride()
    {
        if (!FileAccess.FileExists(OverridePath))
        {
            return true;
        }

        return DirAccess.RemoveAbsolute(ProjectSettings.GlobalizePath(OverridePath)) == Error.Ok;
    }

    public static bool HasOverride() => FileAccess.FileExists(OverridePath);

    private static void Validate(List<CreatureDefinition> creatures)
    {
        HashSet<string> ids = new(StringComparer.OrdinalIgnoreCase);
        foreach (CreatureDefinition creature in creatures)
        {
            creature.Id = NormalizeId(creature.Id);
            if (string.IsNullOrWhiteSpace(creature.Id))
            {
                throw new InvalidOperationException("Every creature needs an ID.");
            }

            if (!ids.Add(creature.Id))
            {
                throw new InvalidOperationException($"Duplicate creature ID: {creature.Id}");
            }

            if (string.IsNullOrWhiteSpace(creature.Name))
            {
                throw new InvalidOperationException($"Creature '{creature.Id}' needs a display name.");
            }

            creature.BaseHp = Math.Clamp(creature.BaseHp, 1, 255);
            creature.BaseAttack = Math.Clamp(creature.BaseAttack, 1, 255);
            creature.BaseDefense = Math.Clamp(creature.BaseDefense, 1, 255);
            creature.BaseSpecialAttack = Math.Clamp(creature.BaseSpecialAttack, 1, 255);
            creature.BaseSpecialDefense = Math.Clamp(creature.BaseSpecialDefense, 1, 255);
            creature.BaseSpeed = Math.Clamp(creature.BaseSpeed, 1, 255);
            creature.CaptureDifficulty = Math.Clamp(creature.CaptureDifficulty, 1, 255);
        }
    }

    public static string NormalizeId(string value)
    {
        string trimmed = value.Trim().ToLowerInvariant().Replace(' ', '_').Replace('-', '_');
        return new string(trimmed.Where(character => char.IsLetterOrDigit(character) || character == '_').ToArray());
    }
}
