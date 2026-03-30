using System;

public readonly struct GeneralSessionDTO
{
    public readonly int TherapyPlanId { get; init; }

    public readonly string ChildId { get; init; }
}

public readonly struct TherapySessionDTO
{
    public readonly bool IsCorrect { get; init; }

    public readonly float DurationSeconds { get; init; }

    public readonly DateTime StartedAt { get; init; }

    public readonly DateTime CompletedAt { get; init; }

    public readonly int? MainAttributeWs { get; init; }

    public readonly int? CorrectAttributeWs { get; init; }

    public readonly int? SelectedAttributeWs { get; init; }

    public readonly bool SelectedButton { get; init; }
}

public readonly struct TherapySessionCreateRequest
{
    public GeneralSessionDTO General { get; init; }

    public TherapySessionDTO[] Tasks { get; init; }
}
