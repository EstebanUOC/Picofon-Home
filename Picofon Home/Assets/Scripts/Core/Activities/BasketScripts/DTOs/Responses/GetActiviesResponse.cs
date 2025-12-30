using System;
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

    public class ActivitiesData<T>
    {
        [JsonInclude]
        public T[] Activities { get; set; }
    }

    public class JudgeActivity
    {
        [JsonInclude]
        public WordInfo[] Words { get; set; }

        [JsonInclude]
        public bool Answer { get; set; }
    }

    public class SelectActivity
    {
        [JsonInclude]
        public WordInfo[] Words { get; set; }
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

    public readonly ref struct ViewContentDTO
    {
        public readonly Span<Sprite> Icons;
        public readonly Span<string> Texts;

        public ViewContentDTO(Span<Sprite> icons, Span<string> texts)
        {
            Icons = icons;
            Texts = texts;
        }
    }

    public readonly ref struct AnswerDTO
    {
        public readonly Span<bool> Answers;

        public AnswerDTO(Span<bool> answers)
        {
            Answers = answers;
        }
    }

    public readonly struct BasketActivity
    {
        public readonly Sprite LeftImage;
        public readonly Sprite RightImage;
        public readonly string LeftWord;
        public readonly string RightWord;
        public readonly string LeftSyllabifiedWord;
        public readonly string RightSyllabifiedWord;
        public readonly string LeftSound;
        public readonly string RightSound;
        public readonly bool Answer;

        public BasketActivity(
            Sprite leftImage,
            Sprite rightImage,
            string leftWord,
            string rightWord,
            string leftSyllabifiedWord,
            string rightSyllabifiedWord,
            string leftSound,
            string rightSound,
            bool answer
        )
        {
            LeftImage = leftImage;
            RightImage = rightImage;
            LeftWord = leftWord;
            RightWord = rightWord;
            LeftSyllabifiedWord = leftSyllabifiedWord;
            RightSyllabifiedWord = rightSyllabifiedWord;
            LeftSound = leftSound;
            RightSound = rightSound;
            Answer = answer;
        }
    }
}
