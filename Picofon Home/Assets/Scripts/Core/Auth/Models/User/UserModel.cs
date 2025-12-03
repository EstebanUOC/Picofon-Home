using System;
using System.Text.Json.Serialization;

[Serializable]
public class UserModel
{
    [JsonInclude]
    public string Id { get; set; } = string.Empty;

    [JsonInclude]
    public string Email { get; set; } = string.Empty;

    [JsonInclude]
    public string Role { get; set; } = string.Empty;
}
