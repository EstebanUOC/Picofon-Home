namespace Picofon.Activities.Segmentation
{
    using System.Text.Json.Serialization;

    public class SegmentationGeneralData
    {
        [JsonInclude]
        public int SkillId { get; set; }

        [JsonInclude]
        public int TaskTypeId { get; set; }
    }

    public class SegmentationData
    {
        [JsonInclude]
        public SegmentationGeneralData GeneralData { get; set; }

        [JsonInclude]
        public SegmentationActivity[] Activities { get; set; }
    }
}
