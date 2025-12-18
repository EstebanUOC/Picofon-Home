using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum HoopType
{
    Positive,
    Negative,
}

public enum AnswerResult
{
    Correct,
    Incorrect,
}

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    public BallTest Ball;

    [Space(15)]
    public FeedbackController FeedbackController;

    public AnswerController AnswerController;
    public HoopsControllers HoopsControllers;

    public Button ClueButton;

    public Sprite LeftSprite { get; set; }
    public Sprite RightSprite { get; set; }

    public event Action<BasketResponses.Activity> OnActivityChange;
    public event Action<bool> OnClueActived;

    private int _currentActivityIndex = 0;

    private BasketResponses.Activity[] _activities;
    private BasketResponses.Activity _currentActivity;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        AnswerController.OnAnswerSelected += LaunchBall;
        LoadActivities().Forget();

        // Codigo que sera borrado
        ClueButton.onClick.AddListener(() =>
        {
            OnClueActived?.Invoke(true);
        });
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
        Ball.TargetPosition = HoopsControllers.GetHoopTarget(hoopType);
        Ball.Launch();

        bool answer = hoopType == HoopType.Positive;

        AnswerResult answerResult =
            _currentActivity.Answer == answer ? AnswerResult.Correct : AnswerResult.Incorrect;

        InitCount(answerResult).Forget();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];

        OnActivityChange?.Invoke(_currentActivity);

        _currentActivityIndex++;

        if (_currentActivityIndex == _activities.Length)
            _currentActivityIndex = 0;
    }

    private async UniTaskVoid InitCount(AnswerResult answerResult)
    {
        await UniTask.WaitForSeconds(2f);

        FeedbackType feedbackType =
            answerResult == AnswerResult.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

        await FeedbackController.ShowFeedback(feedbackType, LeftSprite, RightSprite);

        ChangeActivity();
    }
}
