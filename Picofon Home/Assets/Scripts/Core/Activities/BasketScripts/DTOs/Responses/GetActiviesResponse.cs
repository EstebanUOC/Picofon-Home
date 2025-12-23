using System.Text.Json.Serialization;
using UnityEngine;

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

        public static ApiResult<T> Ok(T data) => new(true, null, data);

        public static ApiResult<T> Fail(string message) => new(false, message, default);
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

    public readonly struct BasketActivity
    {
        public readonly Sprite LeftImage;
        public readonly Sprite RightImage;
        public readonly string LeftWord;
        public readonly string RightWord;
        public readonly string LeftSound;
        public readonly string RightSound;
        public readonly bool Answer;

        public BasketActivity(
            Sprite leftImage,
            Sprite rightImage,
            string leftWord,
            string rightWord,
            string leftSound,
            string rightSound,
            bool answer
        )
        {
            LeftImage = leftImage;
            RightImage = rightImage;
            LeftWord = leftWord;
            RightWord = rightWord;
            LeftSound = leftSound;
            RightSound = rightSound;
            Answer = answer;
        }
    }
}
