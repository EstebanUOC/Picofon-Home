using System.Collections.Generic;
using System.Text.Json.Serialization;

public class UserRegisterChildResponse
{
    [JsonInclude]
    public bool Success { get; set; } = false;

    [JsonInclude]
    public MessageContent Message { get; set; }

    public class MessageContent
    {
        [JsonInclude]
        public List<string> Content { get; set; }
    }
}
