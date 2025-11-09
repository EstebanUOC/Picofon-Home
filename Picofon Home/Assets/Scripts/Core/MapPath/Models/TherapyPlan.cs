using System;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyPlan
{
    [JsonInclude]
    public int TherapyTemplateId;

    [JsonInclude]
    public string AssignedById;

    [JsonInclude]
    public string PlanNumber;

    [JsonInclude]
    public string Name;

    [JsonInclude]
    public int TargetSessions;

    [JsonInclude]
    public string Notes;

    [JsonInclude]
    public string Status;

    [JsonInclude]
    public string StartDate;

    [JsonInclude]
    public string ChildId;

    [JsonInclude]
    public int Id;

    [JsonInclude]
    public string CreatedAt;

    [JsonInclude]
    public string UpdatedAt;

    [JsonInclude]
    public TherapyTemplate TherapyTemplate;
}
