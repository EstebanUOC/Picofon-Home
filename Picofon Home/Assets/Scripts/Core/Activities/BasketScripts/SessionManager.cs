using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

    public void RecordActivityResult(int activityIndex, bool isCorrect)
    {
        DateTime now = DateTime.UtcNow;
        TimeSpan activityDuration = now - _activityStartTime;

        _sessionResults[activityIndex] = new TherapySessionDTO
        {
            IsCorrect = isCorrect,
            StartedAt = _activityStartTime,
            DurationSeconds = (float)activityDuration.TotalSeconds,
            CompletedAt = now,
        };
    }

    public void EndSession()
    {
        SessionService sessionService = new();
        sessionService.CreateTherapySession(_sessionInfo, _sessionResults).Forget();
    }
}
