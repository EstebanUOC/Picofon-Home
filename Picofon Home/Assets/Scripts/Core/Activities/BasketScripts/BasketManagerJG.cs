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

public enum AnswerEvaluation
{
    Correct,
    Incorrect,
}

public class BasketGameManagerJG : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private FeedbackController _feedbackController;

    [SerializeField]
    private BallController _ballController;

    [Space(15)]
    [SerializeField]
    private HoopManager _hoopManager;

    [SerializeField]
    private AnswerManagerJG _answerManager;

    [SerializeField]
    private BasketUIManager _uiManager;

    [Space(15)]
    [SerializeField]
    private AudioClip _instructionClip;

    [SerializeField]
    private AudioClip _positiveYesClip;

    [SerializeField]
    private AudioClip _positiveNoClip;

    [SerializeField]
    private AudioClip _negativeYesClip;

    [SerializeField]
    private AudioClip _negativeNoClip;

    private int _currentActivityIndex = 0;

    private JudgeActivity[] _activities;
    private JudgeActivity _currentActivity;

    private readonly Sprite[] _icons = new Sprite[2];
    private readonly string[] _texts = new string[2];
    private readonly string[] _syllabifiedWords = new string[2];

    public void Awake()
    {
        Application.targetFrameRate = 60;

        _answerManager.OnAnswerSelected += HandleAnswerSelected;

        _feedbackController.Init();

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

        _answerManager.DisableAnswers();

        AudioManager.Instance.PlayVoice(_instructionClip);

        await AudioManager.Instance.WaitVoiceToEnd();

        _answerManager.EnableAnswers();
    }

    private void HandleAnswerSelected(HoopType hoopType)
    {
        int hoopIndex = hoopType == HoopType.Positive ? 0 : 1;
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        _ballController.LaunchBall(hoopTransform);

        bool isPositive = hoopType == HoopType.Positive;
        bool isCorrect = _currentActivity.Answer == isPositive;

        AnswerEvaluation answerResult = isCorrect
            ? AnswerEvaluation.Correct
            : AnswerEvaluation.Incorrect;

        if (isCorrect)
        {
            if (isPositive)
                AudioManager.Instance.PlayVoice(_positiveYesClip);
            else
                AudioManager.Instance.PlayVoice(_positiveNoClip);
        }
        else
        {
            if (isPositive)
                AudioManager.Instance.PlayVoice(_negativeYesClip);
            else
                AudioManager.Instance.PlayVoice(_negativeNoClip);
        }

        InitCount(answerResult).Forget();
    }

    private async UniTaskVoid InitCount(AnswerEvaluation result)
    {
        FeedbackType feedbackType =
            result == AnswerEvaluation.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

        await UniTask.WaitForSeconds(2f);

        await _feedbackController.ShowFeedback(feedbackType);

        ChangeActivity();
    }

    private void ChangeActivity()
    {
        _currentActivity = _activities[_currentActivityIndex];
        _currentActivityIndex = (_currentActivityIndex + 1) % _activities.Length;

        _texts[0] = _currentActivity.Words[0].Word;
        _texts[1] = _currentActivity.Words[1].Word;

        _icons[0] = LoadSprite(_currentActivity.Words[0].Path);
        _icons[1] = LoadSprite(_currentActivity.Words[1].Path);

        _syllabifiedWords[0] = _currentActivity.Words[0].SyllabifiedWord;
        _syllabifiedWords[1] = _currentActivity.Words[1].SyllabifiedWord;

        bool leftAnswer = _currentActivity.Answer;
        bool rightAnswer = !leftAnswer;

        Span<bool> answers = stackalloc bool[2] { leftAnswer, rightAnswer };

        AnswerDTO answer = new(answers);
        _hoopManager.SetHoopStates(in answer);

        ViewContentDTO content = new(_icons, _texts);

        _uiManager.SetViewContent(in content);

        ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords);

        _feedbackController.SetItemsContent(in feedbackContent);

        ResetActivity();
    }

    private void ResetActivity()
    {
        _uiManager.Reset();
        _ballController.Reset();
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
