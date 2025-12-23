using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

    [Space(15)]
    public FeedbackController FeedbackController;
    public AnswerController AnswerController;
    public HoopsControllers HoopsControllers;
    public ClueController ClueController;

    public event ActionIn<BasketResponses.BasketActivity> OnActivityChange;

    public event Action<bool> OnClueActived
    {
        add { ClueController.OnClueActived += value; }
        remove { ClueController.OnClueActived -= value; }
    }

    public event Action<HoopType> OnAnswerSelected
    {
        add { AnswerController.OnAnswerSelected += value; }
        remove { AnswerController.OnAnswerSelected -= value; }
    }

    private int _currentActivityIndex = 0;

    private BasketResponses.Activity[] _activities;
    private BasketResponses.Activity _currentActivity;

    public void Awake()
    {
        Application.targetFrameRate = 60;
        AnswerController.OnAnswerSelected += HandleAnswerSelected;

        Instance = this;
        FeedbackController.Init();

        LoadActivities().Forget();
    }

    private async UniTaskVoid LoadActivities()
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

    private void HandleAnswerSelected(HoopType hoopType)
    {
        InitCount(hoopType).Forget();
    }

    private async UniTaskVoid InitCount(HoopType hoopType)
    {
        bool answer = hoopType == HoopType.Positive;

        AnswerResult answerResult =
            _currentActivity.Answer == answer ? AnswerResult.Correct : AnswerResult.Incorrect;

        await UniTask.WaitForSeconds(2f);

        FeedbackType feedbackType =
            answerResult == AnswerResult.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

        await FeedbackController.ShowFeedback(feedbackType);

        ChangeActivity();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];

        Sprite leftSprite = LoadSprite(_currentActivity.Words[0].Path);
        Sprite rightSprite = LoadSprite(_currentActivity.Words[1].Path);

        string leftWord = _currentActivity.Words[0].Word;
        string rightWord = _currentActivity.Words[1].Word;

        BasketResponses.BasketActivity activity = new(
            leftSprite,
            rightSprite,
            leftWord,
            rightWord,
            _currentActivity.Words[0].Sound,
            _currentActivity.Words[1].Sound,
            _currentActivity.Answer
        );

        OnActivityChange?.Invoke(activity);

        _currentActivityIndex++;

        if (_currentActivityIndex == _activities.Length)
            _currentActivityIndex = 0;
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"No se encontró sprite: {file}");

        return s;
    }
}
