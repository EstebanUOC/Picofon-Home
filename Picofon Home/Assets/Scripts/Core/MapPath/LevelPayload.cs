public static class LevelPayload
{
    // Deprecated: Use ActivityRequestParams directly
    public static int PlanIndex { get; set; }

    public static ActivityRequestParams Params { get; set; }

    public static ActivitySkill Skill { get; set; }

    public static LanguageID Language { get; set; }

    public static bool TaskCompleted { get; set; }

    public static bool IsFinalLevel { get; set; }

    public static bool IsAIEnabled { get; set; }

    public static char Vowel { get; set; }
}
