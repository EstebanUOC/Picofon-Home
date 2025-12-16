using System.Text.Json.Serialization;

namespace BasketResponses
{
    public class GetActiviesResponse
    {
        [JsonInclude]
        public bool Success { get; set; } = false;

        [JsonInclude]
        public MessageData Message { get; set; }

        [JsonInclude]
        public ActivitiesData Data { get; set; }
    }

    public class ActivitiesData
    {
        [JsonInclude]
        public Activity Activity1 { get; set; }

        [JsonInclude]
        public Activity Activity2 { get; set; }

        [JsonInclude]
        public Activity Activity3 { get; set; }

        [JsonInclude]
        public Activity Activity4 { get; set; }

        [JsonInclude]
        public Activity Activity5 { get; set; }

        [JsonInclude]
        public AvailabilityInfo AvailabilityInfo { get; set; }
    }

    public class Activity
    {
        [JsonInclude]
        public string Question { get; set; }

        [JsonInclude]
        public WordInfo Word1 { get; set; }

        [JsonInclude]
        public WordInfo Word2 { get; set; }

        [JsonInclude]
        public bool Answer { get; set; }

        [JsonInclude]
        public string AnswerType { get; set; }

        [JsonInclude]
        public string FeedbackPositive { get; set; }

        [JsonInclude]
        public string FeedbackNeutral { get; set; }

        [JsonInclude]
        public string FeedbackNoAnswer { get; set; }
    }

    public class WordInfo
    {
        [JsonInclude]
        public string Word { get; set; }

        [JsonInclude]
        [JsonPropertyName("PATH")]
        public string Path { get; set; }

        [JsonInclude]
        public int Id { get; set; }

        [JsonInclude]
        public string SyllabifiedWord { get; set; }
    }


    public class AvailabilityInfo
    {
        [JsonInclude]
        public int WordsAvailable { get; set; }

        [JsonInclude]
        public int QuestionsPossible { get; set; }

        [JsonInclude]
        public int TherapyPlanRequested { get; set; }

        [JsonInclude]
        public int WordsPerQuestion { get; set; }

        [JsonInclude]
        public bool SufficientWords { get; set; }

        [JsonInclude]
        public int ActivityNumber { get; set; }

        [JsonInclude]
        public int ActivitiesCreated { get; set; }

        [JsonInclude]
        public int ActivitiesRequested { get; set; }
    }
}
