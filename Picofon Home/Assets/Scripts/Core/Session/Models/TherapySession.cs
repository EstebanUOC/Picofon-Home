namespace Picofon.Core.Session.Models
{
    using System;

    public static class WordRole
    {
        public const string Main = "MAIN";

        public const string Comparison = "COMPARISON";

        public const string CorrectOption = "CORRECT_OPTION";

        public const string Distractor = "DISTRACTOR";
    }

    public readonly struct PresentedWordDTO
    {
        public readonly int WordSegmentId { get; init; }

        public readonly string WordRole { get; init; }

        public readonly int? DisplayOrder { get; init; }

        public readonly bool IsSelected { get; init; }
    }

    public readonly struct GeneralSessionDTO
    {
        public readonly int TherapyPlanId { get; init; }

        public readonly string ChildId { get; init; }

        public readonly string Console { get; init; }

        public readonly string ConductedById { get; init; }
    }

    public readonly struct TherapySessionDTO
    {
        public readonly PresentedWordDTO[] PresentedWords { get; init; }

        public readonly int? SelectedButton { get; init; }

        public readonly bool IsCorrect { get; init; }

        public readonly float DurationSeconds { get; init; }

        public readonly DateTime StartedAt { get; init; }

        public readonly DateTime CompletedAt { get; init; }
    }

    public readonly struct TherapySessionCreateRequest
    {
        public GeneralSessionDTO General { get; init; }

        public TherapySessionDTO[] Tasks { get; init; }
    }
}
