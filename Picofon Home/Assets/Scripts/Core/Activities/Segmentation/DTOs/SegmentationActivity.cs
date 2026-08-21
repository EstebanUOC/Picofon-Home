namespace Picofon.Activities.Segmentation
{
    using System.Text.Json.Serialization;
    using Picofon.Activities.Basket.DTOs.Responses;

    public class SegmentationActivity
    {
        [JsonInclude]
        public WordInfo[] Words { get; set; }

        [JsonInclude]
        public bool Answer { get; set; }

        public WordInfo Word => Words[0];

        public WordInfo Fingers => Words[1];
    }
}
