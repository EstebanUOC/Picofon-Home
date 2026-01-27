using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyTemplate
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("task_type_id")]
    public int TaskTypeId { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("sound_id")]
    public int SoundId { get; set; } = 0;

    [JsonInclude]
    public Skill Skill { get; set; }

    [JsonInclude]
    [JsonPropertyName("syllables_number")]
    public int SyllablesNumber { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("syllable_structure")]
    public string SyllableStructure { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("syllable_position")]
    public string SyllablePosition { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("difficulty_level")]
    public string DifficultyLevel { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("id")]
    public int Id { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("is_active")]
    public bool IsActive { get; set; } = false;

    // 🔥 Add nested task_type object to access task type name
    [JsonInclude]
    [JsonPropertyName("task_type")]
    public TaskType TaskType { get; set; }

    // 🔥 Convenience property to get task type name
    [JsonIgnore]
    public string TaskTypeName => TaskType?.Name ?? "Unknown";
}

[Serializable]
public class TaskType
{
    [JsonInclude]
    public int Id { get; set; }

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public class Skill
{
    [JsonInclude]
    public int Id { get; set; }
}
