using System;
using System.Collections.Generic;

namespace Picofon.Games.Create
{
    [Serializable]
    public class WordData
    {
        public string word;
        public string PATH;
        public int id;
        public string syllabified_word;
    }

    [Serializable]
    public class MessageData
    {
        public List<string> content;
        public bool displayable;
    }

    [Serializable]
    public class AvailabilityInfo
    {
        public int words_available;
        public int questions_possible;
        public int therapy_plan_requested;
        public int words_per_question;
        public bool sufficient_words;
        public int activity_number;
        public int activities_created;
        public int activities_requested;
    }

    [Serializable]
    public class ActivityCreate
    {
        public string question;
        public WordData main_word;
        public WordData hint_word;
        public string hint;
        public bool? answer;
        public string feedback_positive;
        public string feedback_neutral;
        public string feedback_no_answer;
        public List<int> used_word_ids;
    }

    [Serializable]
    public class DataCreate
    {
        public ActivityCreate activity1;
        public ActivityCreate activity2;
        public ActivityCreate activity3;
        public ActivityCreate activity4;
        public ActivityCreate activity5;
        public AvailabilityInfo availability_info;
    }

    [Serializable]
    public class ApiResponseCreate
    {
        public bool success;
        public MessageData message;
        public DataCreate data;
    }
}
