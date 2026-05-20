using System;
using System.Text.Json.Serialization;
using UnityEngine;

namespace BasketResponses
{
    public enum ResponseAudioID : byte
    {
        Intro = 0,
        Correct = 1,
        Incorrect = 2,
    }

    [Serializable]
    public struct AudioEntry<T>
    {
        public T Id;
        public AudioClip Clip;
    }

    [Serializable]
    public struct AudioCategory<T>
    {
        public ActivitySkill Id;
        public AudioEntry<T>[] Entries;
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
        public readonly string[] SyllabifiedWords;
        public readonly string[] Words;

        public ViewContentDTO(Sprite[] icons, string[] syllabifiedWords, string[] words)
        {
            Icons = icons;
            SyllabifiedWords = syllabifiedWords;
            Words = words;
        }

        public ViewContentDTO(Sprite[] icons, string[] words, bool isSyllabified)
        {
            Icons = icons;

            if (isSyllabified)
            {
                SyllabifiedWords = words;
                Words = Array.Empty<string>();
                return;
            }

            SyllabifiedWords = Array.Empty<string>();
            Words = words;
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
