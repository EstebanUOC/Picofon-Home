public enum TherapyStatus : byte
{
    Active = 1,
    Completed = 2,
    Paused = 3,
    Cancelled = 4,
}

public class TherapyPlan
{
    public string ChildId { get; init; }

    public int TherapyPlanId { get; init; }

    public TherapyStatus Status { get; init; }

    public TherapyTemplate TherapyTemplate { get; init; }
}
