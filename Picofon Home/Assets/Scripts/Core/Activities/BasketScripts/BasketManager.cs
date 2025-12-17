using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum HoopType
{
    Positive,
    Negative,
}

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    [Space(15)]
    public BallTest Ball;
    public FeedbackController FeedbackController;

    [Space(15)]
    public Hoop HoopPositive;
    public Hoop HoopNegative;

    public Action<BasketResponses.Activity> OnActivityChange;

    private BasketResponses.Activity[] _activities;
    private int _currentActivityIndex = 0;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        LoadActivities().Forget();
    }

    public async UniTaskVoid LoadActivities()
    {
        BasketService basketService = new();

        ApiResult<ActivitiesData> result = await basketService.GetActivities<ActivitiesData>();

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        _activities = result.Data.Activities;

        if (_activities is null || _activities.Length == 0)
        {
            Debug.LogWarning("No hay actividades cargadas.");
            return;
        }

        ChangeActivity();
    }

    public void LaunchBall(HoopType hoopType)
    {
        Ball.TargetPosition =
            hoopType == HoopType.Positive
                ? HoopPositive.TargetPosition
                : HoopNegative.TargetPosition;

        Ball.Launch();
        InitCount().Forget();
    }

    private void ChangeActivity()
    {
        OnActivityChange?.Invoke(_activities[_currentActivityIndex]);
        _currentActivityIndex++;
    }

    private async UniTaskVoid InitCount()
    {
        await UniTask.WaitForSeconds(2f);

        await FeedbackController.ShowFeedback(FeedbackType.Positive);

        ChangeActivity();
    }
}
