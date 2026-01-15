using System.Text.Json.Serialization;

public class TherapyPlan
{
    [JsonInclude]
    public int TherapyTemplateId { get; set; } = 0;

    [JsonInclude]
    public string ChildId { get; set; } = string.Empty;

    [JsonInclude]
    public int TherapyPlanId { get; set; } = 0;

    [JsonInclude]
    public TherapyTemplate TherapyTemplate { get; set; }
}
