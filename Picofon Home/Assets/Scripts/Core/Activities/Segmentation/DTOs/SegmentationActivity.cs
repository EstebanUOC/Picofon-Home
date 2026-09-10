namespace Picofon.Activities.Segmentation
{
    using System.Text.Json.Serialization;
    using Picofon.Activities.Basket.DTOs.Responses;

    public class SegmentationActivity
    {
        [JsonInclude]
        public WordInfoSegmentation[] Words { get; set; }

        public WordInfoSegmentation Word => Words[0];
    }
}
