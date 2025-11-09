using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyTemplate
{
    [JsonInclude]
    public string Name;

    [JsonInclude]
    public string Description;

    [JsonInclude]
    public int TaskTypeId;

    [JsonInclude]
    public int SoundId;

    [JsonInclude]
    public int SkillId;

    [JsonInclude]
    public int SyllablesNumber;

    [JsonInclude]
    public string SyllableStructure;

    [JsonInclude]
    public string SyllablePosition;

    [JsonInclude]
    public string DifficultyLevel;

    [JsonInclude]
    public int Id;

    [JsonInclude]
    public bool IsActive;
}
