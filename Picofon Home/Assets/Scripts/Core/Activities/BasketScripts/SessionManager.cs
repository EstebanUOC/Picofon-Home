using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public readonly struct TaskInfo
{
    public readonly bool IsCorrect { get; init; }

    public readonly int TaskIndex { get; init; }

    public readonly int MainAttributeWs { get; init; }

    public readonly int CorrectAttributeWs { get; init; }

    public readonly int SelectedAttributeWs { get; init; }

    public readonly bool SelectedButton { get; init; }
}

public class SessionManager : MonoBehaviour
{
    private GeneralSessionDTO _sessionInfo;
    private TherapySessionDTO[] _sessionResults;

    private DateTime _activityStartTime;

    public void InitializeSession(GeneralSessionDTO sessionInfo, TherapySessionDTO[] sessionResults)
    {
        _sessionInfo = sessionInfo;
        _sessionResults = sessionResults;
    }

    public void StartTime()
    {
        _activityStartTime = DateTime.UtcNow;
    }

    public void RecordActivityResult(in TaskInfo taskInfo)
    {
        DateTime now = DateTime.UtcNow;
        TimeSpan activityDuration = now - _activityStartTime;

        _sessionResults[taskInfo.TaskIndex] = new TherapySessionDTO
        {
            IsCorrect = taskInfo.IsCorrect,
            StartedAt = _activityStartTime,
            DurationSeconds = (float)activityDuration.TotalSeconds,
            CompletedAt = now,
            MainAttributeWs = taskInfo.MainAttributeWs,
            CorrectAttributeWs = taskInfo.CorrectAttributeWs,
            SelectedAttributeWs = taskInfo.SelectedAttributeWs,
            SelectedButton = taskInfo.SelectedButton,
        };
    }

    public void EndSession()
    {
        if (Application.isEditor)
            return;

        SessionService sessionService = new();
        sessionService.CreateTherapySession(_sessionInfo, _sessionResults).Forget();
    }
}
