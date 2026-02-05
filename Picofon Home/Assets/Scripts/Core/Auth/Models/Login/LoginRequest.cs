using System.Text.Json.Serialization;

public struct LoginRequest
{
    [JsonInclude]
    public string FirebaseIdToken { get; set; }
}
