using System.Text.Json.Serialization;

public class LoginRequest
{
    [JsonInclude]
    public string FirebaseIdToken { get; set; } = string.Empty;

    public string ToJson()
    {
        return JsonHelper.ToJson(this);
    }
}
