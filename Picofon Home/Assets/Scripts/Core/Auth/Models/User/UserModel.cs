using System;
using System.Text.Json.Serialization;

[Serializable]
public class UserModel
{
    [JsonInclude]
    public string Id;

    [JsonInclude]
    public string Email;

    [JsonInclude]
    public string Role;

    public string ToJson()
    {
        return JsonHelper.ToJson(this);
    }

    public static UserModel FromJson(string json)
    {
        return JsonHelper.FromJson<UserModel>(json);
    }
}
