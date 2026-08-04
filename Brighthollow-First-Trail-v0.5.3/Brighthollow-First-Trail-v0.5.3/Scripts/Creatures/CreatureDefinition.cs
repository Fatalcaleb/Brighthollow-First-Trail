using System.Collections.Generic;
using System.Text.Json.Serialization;

public sealed class CreatureDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "new_creature";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "New Creature";

    [JsonPropertyName("primary_type")]
    public string PrimaryType { get; set; } = "Neutral";

    [JsonPropertyName("secondary_type")]
    public string SecondaryType { get; set; } = "None";

    [JsonPropertyName("base_hp")]
    public int BaseHp { get; set; } = 45;

    [JsonPropertyName("base_attack")]
    public int BaseAttack { get; set; } = 45;

    [JsonPropertyName("base_defense")]
    public int BaseDefense { get; set; } = 45;

    [JsonPropertyName("base_special_attack")]
    public int BaseSpecialAttack { get; set; } = 45;

    [JsonPropertyName("base_special_defense")]
    public int BaseSpecialDefense { get; set; } = 45;

    [JsonPropertyName("base_speed")]
    public int BaseSpeed { get; set; } = 45;

    [JsonPropertyName("capture_difficulty")]
    public int CaptureDifficulty { get; set; } = 100;

    [JsonPropertyName("ability")]
    public string Ability { get; set; } = "Steady Heart";

    [JsonPropertyName("personality_traits")]
    public List<string> PersonalityTraits { get; set; } = new() { "Curious" };

    [JsonPropertyName("description")]
    public string Description { get; set; } = "A newly documented creature.";

    [JsonPropertyName("level_moves")]
    public List<LevelMoveDefinition> LevelMoves { get; set; } = new();
}

public sealed class LevelMoveDefinition
{
    [JsonPropertyName("level")]
    public int Level { get; set; }

    [JsonPropertyName("move_id")]
    public string MoveId { get; set; } = "tackle";
}
