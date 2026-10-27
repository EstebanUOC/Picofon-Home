using Picofon.Core.MapPath.Services;
using Picofon.Core.Session.Models;
using Picofon.Core.Session.Services;
using Picofon.Utils;

namespace Picofon.Activities.Basket
{
    using System;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public readonly struct TaskInfo
    {
        public readonly bool IsCorrect { get; init; }

        public readonly int TaskIndex { get; init; }

        public readonly PresentedWordDTO[] PresentedWords { get; init; }

        public readonly int? SelectedButton { get; init; }
    }

    public class SessionManager : MonoBehaviour
    {
        private GeneralSessionDTO _sessionInfo;
        private TherapySessionDTO[] _sessionResults;

        private DateTime _activityStartTime;
        private bool _taskCompleted = false;

        public void InitializeSession(
            GeneralSessionDTO sessionInfo,
            TherapySessionDTO[] sessionResults,
            bool completed
        )
        {
            _sessionInfo = sessionInfo;
            _sessionResults = sessionResults;
            _taskCompleted = completed;
        }

        public void StartTime()
        {
            if (_taskCompleted)
                return;

            _activityStartTime = DateTime.UtcNow;
        }

        public void RecordActivityResult(in TaskInfo taskInfo)
        {
            if (_taskCompleted)
                return;

            DateTime now = DateTime.UtcNow;
            TimeSpan activityDuration = now - _activityStartTime;

            _sessionResults[taskInfo.TaskIndex] = new TherapySessionDTO
            {
                IsCorrect = taskInfo.IsCorrect,
                StartedAt = _activityStartTime,
                DurationSeconds = (float)activityDuration.TotalSeconds,
                CompletedAt = now,
                PresentedWords = taskInfo.PresentedWords ?? Array.Empty<PresentedWordDTO>(),
                SelectedButton = taskInfo.SelectedButton,
            };
        }

        public async UniTask EndSession()
        {
            // TODO: descoment this
            // if (Application.isEditor)
            //     return;

            if (_taskCompleted)
                return;

            TherapyPlanService therapyPlanService = new(0);
            SessionService sessionService = new();

            PerformanceLog.Log("Lleguo aqui");

            await sessionService.CreateTherapySession(_sessionInfo, _sessionResults);

            await therapyPlanService.ChangePlanStatus(
                id: _sessionInfo.TherapyPlanId,
                status: TherapyStatus.Completed
            );

            await sessionService.CreateTherapySession(_sessionInfo, _sessionResults);

            LearningRateService learningRateService = new(0);

            await learningRateService.CalculateLearningRate(
                childId: _sessionInfo.ChildId,
                therapyPlan: _sessionInfo.TherapyPlanId
            );
        }
    }
}
