using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace BasketResponses
{
    public enum JudgeAudioID
    {
        Intro,
        PositiveAndCorrect,
        PositiveAndIncorrect,
        NegativeAndCorrect,
        NegativeAndIncorrect,
    }

    public enum OthersAudioID
    {
        Intro,
        Positive,
        Negative,
    }

    [Serializable]
    public struct AudioEntry<T>
    {
        public T Id;
        public AudioClip Clip;
    }

    public class JudgeActivity
    {
        public WordInfo[] Words { get; set; }

        public bool Answer { get; set; }
    }

    public class SelectActivity
    {
        [JsonInclude]
        public WordInfoPS[] Words { get; set; }
    }

    public class RelateActivity
    {
        [JsonInclude]
        public WordInfo MainWord { get; set; }

        [JsonInclude]
        public WordInfoPS[] Words { get; set; }
    }

    public class WordInfo
    {
        public int Id { get; set; }

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

    public class WordInfoPS : WordInfo
    {
        [JsonInclude]
        public bool Answer { get; set; }
    }

    public readonly ref struct ViewContentDTO
    {
        public readonly Sprite[] Icons;
        public readonly string[] Texts;

        public ViewContentDTO(Sprite[] icons, string[] texts)
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
}
