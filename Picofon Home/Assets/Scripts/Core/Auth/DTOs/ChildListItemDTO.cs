using System.Text.Json.Serialization;

public class ChildListItemDTO
{
    [JsonInclude]
    public string Id { get; set; } = string.Empty;

    [JsonInclude]
    public string FirstName { get; set; } = string.Empty;

    [JsonInclude]
    public string LastName { get; set; } = string.Empty;
}
