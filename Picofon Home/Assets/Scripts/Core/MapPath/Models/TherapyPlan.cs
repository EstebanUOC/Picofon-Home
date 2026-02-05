using System.Text.Json.Serialization;

public class TherapyPlan
{
    [JsonInclude]
    public int TherapyTemplateId { get; private set; }

    [JsonInclude]
    public string ChildId { get; private set; }

    [JsonInclude]
    public int TherapyPlanId { get; private set; }

    [JsonInclude]
    public TherapyTemplate TherapyTemplate { get; private set; }
}
