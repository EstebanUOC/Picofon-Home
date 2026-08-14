using Picofon.Utils;

namespace Picofon.Core.MapPath.Models
{
    public class TherapyPlan
    {
        public string ChildId { get; init; }

        public int TherapyPlanId { get; init; }

        public char Vowel { get; init; }

        public TherapyStatus Status { get; init; }

        public TherapyTemplate TherapyTemplate { get; init; }

        public int LanguageId { get; init; }

        public int OrderNumber { get; init; }
    }
}
