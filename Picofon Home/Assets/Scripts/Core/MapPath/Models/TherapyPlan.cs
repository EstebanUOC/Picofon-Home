using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyPlan
{
    [JsonInclude]
    public int TherapyTemplateId { get; set; } = 0;

    [JsonInclude]
    public string AssignedById { get; set; } = string.Empty;

    [JsonInclude]
    public string PlanNumber { get; set; } = string.Empty;

    [JsonInclude]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    public int TargetSessions { get; set; } = 0;

    [JsonInclude]
    public string Notes { get; set; } = string.Empty;

    [JsonInclude]
    public string Status { get; set; } = string.Empty;

    [JsonInclude]
    public string StartDate { get; set; } = string.Empty;

    [JsonInclude]
    public string ChildId { get; set; } = string.Empty;

    [JsonInclude]
    public int Id { get; set; } = 0;

    [JsonInclude]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonInclude]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonInclude]
    public TherapyTemplate TherapyTemplate { get; set; }
}
