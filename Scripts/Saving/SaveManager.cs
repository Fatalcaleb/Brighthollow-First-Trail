using Godot;

public static class SaveManager
{
    private const string SavePath = "user://save_slot_1.json";

    public static bool Save(Vector2 playerPosition, string locationName, double playTimeSeconds)
    {
        Godot.Collections.Dictionary data = new()
        {
            ["version"] = "0.2.0",
            ["location"] = locationName,
            ["player_x"] = playerPosition.X,
            ["player_y"] = playerPosition.Y,
            ["play_time_seconds"] = playTimeSeconds,
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

    public static bool TryLoad(out Vector2 playerPosition, out string locationName, out double playTimeSeconds)
    {
        playerPosition = Vector2.Zero;
        locationName = "Mossmere";
        playTimeSeconds = 0.0;

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

        float x = data["player_x"].AsSingle();
        float y = data["player_y"].AsSingle();
        playerPosition = new Vector2(x, y);

        if (data.ContainsKey("location"))
        {
            locationName = data["location"].AsString();
        }

        if (data.ContainsKey("play_time_seconds"))
        {
            playTimeSeconds = data["play_time_seconds"].AsDouble();
        }

        return true;
    }

    public static bool HasSave() => FileAccess.FileExists(SavePath);
}
