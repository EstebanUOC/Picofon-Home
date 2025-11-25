using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyPlan
{
    [JsonInclude]
    [JsonPropertyName("therapy_template_id")]
    public int TherapyTemplateId { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("assigned_by_id")]
    public string AssignedById { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("plan_number")]
    public string PlanNumber { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("target_sessions")]
    public int TargetSessions { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("notes")]
    public string Notes { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("start_date")]
    public string StartDate { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("child_id")]
    public string ChildId { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("therapy_plan_id")]
    public int Id { get; set; } = 0;

    [JsonInclude]
    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;

    [JsonInclude]
    [JsonPropertyName("therapy_template")]
    public TherapyTemplate TherapyTemplate { get; set; }
}
