using System.Text.Json.Serialization;

public class LoginRequest
{
    [JsonInclude]
    public string FirebaseIdToken { get; set; } = string.Empty;
}
