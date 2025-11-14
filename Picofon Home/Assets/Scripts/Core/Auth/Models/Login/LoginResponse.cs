using System.Text.Json.Serialization;

public class LoginResponse
{
    [JsonInclude]
    public bool Success { get; set; }

    [JsonInclude]
    public UserModel Data { get; set; }

    public static LoginResponse FromJson(string json)
    {
        return JsonHelper.FromJson<LoginResponse>(json);
    }
}
