using System;
using System.Collections.Generic;

namespace Picofon.Games.Judge
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
    public class ActivityJudge
    {
        public string question;
        public WordData word1;
        public WordData word2;
        public bool answer;
        public string answer_type;
        public string feedback_positive;
        public string feedback_neutral;
        public string feedback_no_answer;
    }

    [Serializable]
    public class DataJudge
    {
        public ActivityJudge activity1;
        public ActivityJudge activity2;
        public ActivityJudge activity3;
        public ActivityJudge activity4;
        public ActivityJudge activity5;
        public AvailabilityInfo availability_info; // 🔥 ADD THIS
    }

    [Serializable]
    public class ApiResponseJudge
    {
        public bool success; // 🔥 ADD THIS
        public MessageData message; // 🔥 ADD THIS
        public DataJudge data;
    }
}
