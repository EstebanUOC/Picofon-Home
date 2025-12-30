using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = BasketResponses.ApiResult<BasketResponses.ActivitiesData<BasketResponses.JudgeActivity>>;

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

public class BasketGameManagerJG : MonoBehaviour
{
    [Space(15)]
    public FeedbackController FeedbackController;
    public AnswerControllerJG AnswerController;

    [SerializeField]
    private HoopManager _hoopManager;

    [Space(15)]
    [SerializeField]
    private BasketUIManager _uiManager;

    [SerializeField]
    private BallController _ballController;

    private int _currentActivityIndex = 0;

    private JudgeActivity[] _activities;
    private JudgeActivity _currentActivity;

    public void Awake()
    {
        Application.targetFrameRate = 60;

        AnswerController.OnAnswerSelected += HandleAnswerSelected;

        FeedbackController.Init();

        LoadActivities().Forget();
    }

    private async UniTaskVoid LoadActivities()
    {
        BasketService basketService = new();

        ActivitiesResult result = await basketService.GetActivities<
            ActivitiesData<JudgeActivity>
        >();

        // NOTE: Wait a frame to ensure all initializations are done, Do not delete, 100% necessary
        await UniTask.Yield();

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
        int hoopIndex = hoopType == HoopType.Positive ? 0 : 1;
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        _ballController.LaunchBall(hoopTransform);

        bool answer = hoopType == HoopType.Positive;

        AnswerResult answerResult =
            _currentActivity.Answer == answer ? AnswerResult.Correct : AnswerResult.Incorrect;

        InitCount(answerResult).Forget();
    }

    private async UniTaskVoid InitCount(AnswerResult result)
    {
        FeedbackType feedbackType =
            result == AnswerResult.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

        await UniTask.WaitForSeconds(2f);

        await FeedbackController.ShowFeedback(feedbackType);

        ChangeActivity();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];
        _currentActivityIndex = (_currentActivityIndex + 1) % _activities.Length;

        Sprite leftSprite = LoadSprite(_currentActivity.Words[0].Path);
        Sprite rightSprite = LoadSprite(_currentActivity.Words[1].Path);

        string leftWord = _currentActivity.Words[0].Word;
        string rightWord = _currentActivity.Words[1].Word;

        Span<Sprite> icons = new Sprite[2] { leftSprite, rightSprite };
        Span<string> texts = new string[2] { leftWord, rightWord };

        Span<bool> answers = new bool[_activities.Length];

        for (int i = 0; i < _activities.Length; i++)
        {
            answers[i] = _activities[i].Answer;
        }

        AnswerDTO answer = new(answers);
        ViewContentDTO content = new(icons, texts);

        _uiManager.SetViewContent(in content);
        _hoopManager.SetHoopStates(in answer);
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
