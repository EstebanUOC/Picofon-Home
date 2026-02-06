using System.Text.Json.Serialization;

public struct TherapySessionDTO
{
    [JsonInclude]
    public int Id { get; private set; }

    [JsonInclude]
    public int TherapyPlanId { get; private set; }

    [JsonInclude]
    public string ChildId { get; private set; }
}
