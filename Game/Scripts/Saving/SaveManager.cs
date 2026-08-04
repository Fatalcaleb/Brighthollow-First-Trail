using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

public sealed class PlayerProfileData
{
    public string PlayerName { get; set; } = "Trainer";
    public string RivalName { get; set; } = "Rowan";
    public int AppearancePreset { get; set; }
}

public sealed class GameSaveData
{
    public Vector2 PlayerPosition { get; set; }
    public string MapId { get; set; } = "mossmere";
    public double PlayTimeSeconds { get; set; }
    public Dictionary<string, bool> StoryFlags { get; set; } = new();
    public PlayerProfileData Profile { get; set; } = new();
    public string SavedAt { get; set; } = string.Empty;
}

public static class SaveManager
{
    private const string SavePath = "user://save_slot_1.json";
    private static bool _migrationChecked;

    public static bool Save(GameSaveData save)
    {
        EnsureLegacySaveMigration();

        Godot.Collections.Dictionary flags = new();
        foreach ((string key, bool value) in save.StoryFlags)
        {
            flags[key] = value;
        }

        Godot.Collections.Dictionary profile = new()
        {
            ["player_name"] = save.Profile.PlayerName,
            ["rival_name"] = save.Profile.RivalName,
            ["appearance_preset"] = save.Profile.AppearancePreset
        };

        Godot.Collections.Dictionary data = new()
        {
            ["version"] = BuildInfo.Version,
            ["map_id"] = save.MapId,
            ["player_x"] = save.PlayerPosition.X,
            ["player_y"] = save.PlayerPosition.Y,
            ["play_time_seconds"] = save.PlayTimeSeconds,
            ["story_flags"] = flags,
            ["profile"] = profile,
            ["saved_at"] = Time.GetDatetimeStringFromSystem()
        };

        using FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file is null)
        {
            GD.PushError($"Unable to open save file: {FileAccess.GetOpenError()}");
            return false;
        }

        file.StoreString(Json.Stringify(data, "  "));
        return true;
    }

    public static bool TryLoad(out GameSaveData save)
    {
        EnsureLegacySaveMigration();
        save = new GameSaveData();
        if (!FileAccess.FileExists(SavePath))
        {
            return false;
        }

        using FileAccess file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file is null)
        {
            GD.PushError($"Unable to read save file: {FileAccess.GetOpenError()}");
            return false;
        }

        Variant parsed = Json.ParseString(file.GetAsText());
        if (parsed.VariantType != Variant.Type.Dictionary)
        {
            GD.PushError("Save file is not valid JSON data.");
            return false;
        }

        Godot.Collections.Dictionary data = parsed.AsGodotDictionary();
        if (!data.ContainsKey("player_x") || !data.ContainsKey("player_y"))
        {
            return false;
        }

        save.PlayerPosition = new Vector2(data["player_x"].AsSingle(), data["player_y"].AsSingle());
        save.MapId = data.ContainsKey("map_id") ? data["map_id"].AsString() : "mossmere";
        save.PlayTimeSeconds = data.ContainsKey("play_time_seconds") ? data["play_time_seconds"].AsDouble() : 0.0;
        save.SavedAt = data.ContainsKey("saved_at") ? data["saved_at"].AsString() : string.Empty;

        if (data.ContainsKey("profile") && data["profile"].VariantType == Variant.Type.Dictionary)
        {
            Godot.Collections.Dictionary profile = data["profile"].AsGodotDictionary();
            save.Profile.PlayerName = profile.ContainsKey("player_name") ? profile["player_name"].AsString() : "Trainer";
            save.Profile.RivalName = profile.ContainsKey("rival_name") ? profile["rival_name"].AsString() : "Rowan";
            save.Profile.AppearancePreset = profile.ContainsKey("appearance_preset") ? profile["appearance_preset"].AsInt32() : 0;
        }

        if (data.ContainsKey("story_flags") && data["story_flags"].VariantType == Variant.Type.Dictionary)
        {
            Godot.Collections.Dictionary flags = data["story_flags"].AsGodotDictionary();
            foreach (Variant key in flags.Keys)
            {
                save.StoryFlags[key.AsString()] = flags[key].AsBool();
            }
        }

        return true;
    }

    public static bool TryReadMetadata(out GameSaveData metadata) => TryLoad(out metadata);

    public static bool HasSave()
    {
        EnsureLegacySaveMigration();
        return FileAccess.FileExists(SavePath);
    }

    /// <summary>
    /// Moves one compatible save from an older version-specific Godot user folder into
    /// the fixed Brighthollow user directory. The original file is never deleted.
    /// </summary>
    private static void EnsureLegacySaveMigration()
    {
        if (_migrationChecked)
        {
            return;
        }

        _migrationChecked = true;
        string currentSavePath = ProjectSettings.GlobalizePath(SavePath);
        if (System.IO.File.Exists(currentSavePath))
        {
            return;
        }

        try
        {
            string? appData = System.Environment.GetEnvironmentVariable("APPDATA");
            if (string.IsNullOrWhiteSpace(appData))
            {
                return;
            }

            string legacyRoot = System.IO.Path.Combine(appData, "Godot", "app_userdata");
            if (!System.IO.Directory.Exists(legacyRoot))
            {
                return;
            }

            string currentDirectory = System.IO.Path.GetDirectoryName(currentSavePath) ?? string.Empty;
            var candidates = System.IO.Directory
                .EnumerateDirectories(legacyRoot)
                .Where(directory => !string.Equals(directory, currentDirectory, StringComparison.OrdinalIgnoreCase))
                .Where(directory =>
                {
                    string name = System.IO.Path.GetFileName(directory);
                    return name.Contains("Brighthollow", StringComparison.OrdinalIgnoreCase)
                        && name.Contains("First Trail", StringComparison.OrdinalIgnoreCase);
                })
                .Select(directory => System.IO.Path.Combine(directory, "save_slot_1.json"))
                .Where(System.IO.File.Exists)
                .Where(IsCompatibleLegacySave)
                .OrderByDescending(System.IO.File.GetLastWriteTimeUtc)
                .ToList();

            if (candidates.Count == 0)
            {
                return;
            }

            System.IO.Directory.CreateDirectory(currentDirectory);
            System.IO.File.Copy(candidates[0], currentSavePath, overwrite: false);
            GD.Print($"Migrated a compatible legacy save from: {candidates[0]}");
            GD.Print($"The original save was preserved. New save location: {currentSavePath}");
        }
        catch (Exception exception)
        {
            GD.PushWarning($"Legacy save migration was skipped: {exception.Message}");
        }
    }

    private static bool IsCompatibleLegacySave(string path)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(System.IO.File.ReadAllText(path));
            JsonElement root = document.RootElement;
            return root.ValueKind == JsonValueKind.Object
                && root.TryGetProperty("player_x", out _)
                && root.TryGetProperty("player_y", out _);
        }
        catch
        {
            return false;
        }
    }
}
