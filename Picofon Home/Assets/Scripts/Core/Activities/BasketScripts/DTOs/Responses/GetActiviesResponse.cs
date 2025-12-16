using System.Text.Json.Serialization;

namespace BasketResponses
{
    public sealed class ApiResult<T>
    {
        public bool Success { get; }
        public string? Message { get; }
        public T? Data { get; }

        public ApiResult(bool success, string? message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResult<T> Ok(T data)
            => new(true, null, data);

        public static ApiResult<T> Fail(string message)
            => new(false, message, default);
    }

    public class ActivitiesData
    {
        [JsonInclude]
        public Activity[] Activities { get; set; }
    }

    public class Activity
    {
        [JsonInclude]
        public WordInfo[] Words { get; set; }

        [JsonInclude]
        public bool Answer { get; set; }
    }

    public class WordInfo
    {
        [JsonInclude]
        public string Word { get; set; }

        [JsonInclude]
        public string Path { get; set; }

        [JsonInclude]
        public string SyllabifiedWord { get; set; }

        [JsonInclude]
        [JsonPropertyName("word_sound")]
        public string Sound { get; set; }
    }
}
