using System.Collections.Generic;
using System.Text.Json.Serialization;

public class LoginResponse
{
    [JsonInclude]
    public bool Success { get; set; } = false;

    [JsonInclude]
    public LoginData Data { get; set; }

    [JsonInclude]
    public MessageData Message { get; set; }
}

public class MessageData
{
    [JsonInclude]
    public List<string> Content { get; set; }
}
