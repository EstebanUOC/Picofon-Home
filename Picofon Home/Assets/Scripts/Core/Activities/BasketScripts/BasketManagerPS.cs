using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = ApiResult<BasketResponses.ActivitiesData<BasketResponses.SelectActivity>>;

public class BasketManagerPS : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private BallController _ballController;

    [SerializeField]
    private FeedbackController _feedbackController;

    [Space(15)]
    [SerializeField]
    private AnswerManagerPS _answerManager;

    [SerializeField]
    private HoopManager _hoopManager;

    [SerializeField]
    private BasketUIManager _uiManager;

    [SerializeField]
    private SessionManager _sessionManager;

    [SerializeField]
    private AudioClip _instructionClip;

    [SerializeField]
    private AudioClip _positiveClip;

    [SerializeField]
    private AudioClip _negativeClip;

    private int _currentActivityIndex = 0;

    private SelectActivity[] _activities;
    private SelectActivity _currentActivity;

    private readonly Sprite[] _icons = new Sprite[4];
    private readonly string[] _texts = new string[4];
    private readonly string[] _syllabifiedWords = new string[4];

    public void Awake()
    {
        Application.targetFrameRate = 60;

        _answerManager.OnHoopSelected += HandleHoopSelected;

        ActivityRequestParams @params = LevelPayload.Params;
        ActivitySkill skill = LevelPayload.Skill;

        if (@params.ChildId is null)
        {
#if !UNITY_EDITOR
            Debug.LogError("Parameters are missing in LevelPayload.");
            return;
# else
            skill = ActivitySkill.Initial;
            @params = new ActivityRequestParams { PlanId = 42, ChildId = "19013454K" };
            Debug.LogWarning("Using default parameters for testing in Unity Editor.");
# endif
        }

        _feedbackController.Init(skill);

        LoadActivities(@params).Forget();
    }

    private async UniTaskVoid LoadActivities(ActivityRequestParams @params)
    {
        BasketService basketService = new();

        ActivitiesResult result = await basketService.GetActivities<ActivitiesData<SelectActivity>>(
            @params
        );

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

        TherapySessionDTO[] sessions = new TherapySessionDTO[_activities.Length];
        GeneralSessionDTO sessionInfo = new()
        {
            TherapyPlanId = @params.PlanId,
            ChildId = @params.ChildId,
        };

        _sessionManager.InitializeSession(sessionInfo, sessions);

        ChangeActivity();
        _uiManager.EnableClueButton(false);

        AudioManager.Instance.PlayVoice(_instructionClip);

        await AudioManager.Instance.WaitVoiceToEnd();

        _uiManager.Prueba();

        await UniTask.WaitForSeconds(0.25f);

        _answerManager.Prueba();

        _sessionManager.StartTime();
        _uiManager.EnableClueButton(true);
    }

    private void HandleHoopSelected(int hoopIndex)
    {
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        WordInfoPS currentWord = _currentActivity.Words[hoopIndex];

        _ballController.LaunchBall(hoopTransform);

        bool isCorrect = currentWord.Answer;

        AudioClip clip = isCorrect ? _positiveClip : _negativeClip;

        AudioManager.Instance.PlayVoice(clip);

        int correctWordId = 0;

        if (isCorrect)
        {
            correctWordId = currentWord.Id;
        }
        else
        {
            foreach (var word in _currentActivity.Words)
            {
                if (word.Answer)
                {
                    correctWordId = word.Id;
                    break;
                }
            }
        }

        TaskInfo taskInfo = new()
        {
            IsCorrect = isCorrect,
            TaskIndex = _currentActivityIndex,
            SelectedAttributeWs = currentWord.Id,
            CorrectAttributeWs = correctWordId,
        };

        _sessionManager.RecordActivityResult(in taskInfo);

        _currentActivityIndex++;

        InitCount(isCorrect).Forget();
    }

    private async UniTaskVoid InitCount(bool isCorrect)
    {
        FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

        await UniTask.WaitForSeconds(2f);

        await _feedbackController.ShowFeedback(feedbackType);

        ChangeActivity();

        _sessionManager.StartTime();
    }

    private void ChangeActivity()
    {
        if (_currentActivityIndex >= _activities.Length)
        {
            _sessionManager.EndSession();
            return;
        }

        _currentActivity = _activities[_currentActivityIndex];

        for (int i = 0; i < _texts.Length; i++)
        {
            _texts[i] = _currentActivity.Words[i].Word;
            _icons[i] = LoadSprite(_currentActivity.Words[i].Path);
            _syllabifiedWords[i] = _currentActivity.Words[i].SyllabifiedWord;
        }

        Span<bool> answers = stackalloc bool[4];
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i] = _currentActivity.Words[i].Answer;
        }

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
