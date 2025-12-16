using System.Text.Json.Serialization;

namespace Picofon.Core.Network
{
  public class ApiResponse<T>
  {
    [JsonInclude]
    public bool Success { get; set; } = false;

    [JsonInclude]
    public MessageData Message { get; set; }

    [JsonInclude]
    public T Data { get; set; }
  }

  public class MessageData
  {
    [JsonInclude]
    public string Code { get; set; }

    [JsonInclude]
    public string Text { get; set; }
  }
}

