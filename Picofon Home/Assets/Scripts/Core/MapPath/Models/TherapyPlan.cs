using Picofon.Utils;

namespace Picofon.Core.MapPath.Models
{
    public static class PlanTypes
    {
        public const string Level = "LEVEL";
        public const string LudicScene = "LUDIC_SCENE";
    }

    public class LudicSceneInfo
    {
        public int Id { get; init; }

        public string Code { get; init; }

        public string Name { get; init; }
    }

    public class TherapyPlan
    {
        public string ChildId { get; init; }

        public int TherapyPlanId { get; init; }

        public char? Vowel { get; init; }

        public TherapyStatus Status { get; init; }

        // null en planes LUDIC_SCENE (HOME_DISCOVERY v3)
        public TherapyTemplate TherapyTemplate { get; init; }

        public int LanguageId { get; init; }

        public int OrderNumber { get; init; }

        // Adaptive Policy v2: stage de la malla; null en planes manuales/legacy/discovery
        public int? PolicyStageId { get; init; }

        // Adaptive Policy v3 (HOME_DISCOVERY)
        public string PlanType { get; init; } = PlanTypes.Level;

        // HOME_DISCOVERY | CURRICULUM | MANUAL
        public string PlanMode { get; init; }

        public int? DiscoveryStepId { get; init; }

        public LudicSceneInfo LudicScene { get; init; }

        public bool IsLudicScene => PlanType == PlanTypes.LudicScene;
    }
}
