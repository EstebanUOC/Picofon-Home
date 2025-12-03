using System.Text.Json.Serialization;

public class LoginResponse
{
    [JsonInclude]
    public bool Success { get; set; }

    [JsonInclude]
    public UserModel Data { get; set; }
}
