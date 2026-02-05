using System.Text.Json.Serialization;

public class ChildListItemDTO
{
    [JsonInclude]
    public string Id { get; private set; } = string.Empty;

    [JsonInclude]
    public string FirstName { get; private set; } = string.Empty;

    [JsonInclude]
    public string LastName { get; private set; } = string.Empty;
}
