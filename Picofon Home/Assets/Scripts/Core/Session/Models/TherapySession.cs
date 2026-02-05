using System.Text.Json.Serialization;

public class TherapySession
{
    [JsonInclude]
    public int Id { get; private set; }

    [JsonInclude]
    public int TherapyPlanId { get; private set; }

    [JsonInclude]
    public string ChildId { get; private set; }
}
