using System;
using System.Collections.Generic;

namespace Picofon.Games.Relate
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
    public class ActivityRelate
    {
        public string question;
        public WordData main_word;
        public WordData correct_option;
        public WordData wrong_option1;
        public WordData wrong_option2;
        public WordData wrong_option3;
        public bool answer;
        public string feedback_positive;
        public string feedback_neutral;
        public string feedback_no_answer;
    }

    [Serializable]
    public class DataRelate
    {
        public ActivityRelate activity1;
        public ActivityRelate activity2;

        public AvailabilityInfo availability_info;
    }

    [Serializable]
    public class ApiResponseRelate
    {
        public bool success;
        public MessageData message;
        public DataRelate data;
    }
}
