using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyTemplate
{
    [JsonInclude]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    public string Description { get; set; } = string.Empty;

    [JsonInclude]
    public int TaskTypeId { get; set; } = 0;

    [JsonInclude]
    public int SoundId { get; set; } = 0;

    [JsonInclude]
    public int SkillId { get; set; } = 0;

    [JsonInclude]
    public int SyllablesNumber { get; set; } = 0;

    [JsonInclude]
    public string SyllableStructure { get; set; } = string.Empty;

    [JsonInclude]
    public string SyllablePosition { get; set; } = string.Empty;

    [JsonInclude]
    public string DifficultyLevel { get; set; } = string.Empty;

    [JsonInclude]
    public int Id { get; set; } = 0;

    [JsonInclude]
    public bool IsActive { get; set; } = false;
}
