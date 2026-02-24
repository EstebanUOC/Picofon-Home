using System;
using System.Collections.Generic;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = ApiResult<ActivitiesData<BasketResponses.JudgeActivity>>;

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
    [Space]
    [SerializeField]
    private CanvasUI _canvasUI;

    [SerializeField]
    private BallController _ballController;

    [SerializeField]
    private ProgressBar _progressBar;

    [Space]
    [SerializeField]
    private HoopManager _hoopManager;

    [SerializeField]
    private AnswerManagerJG _answerManager;

    [SerializeField]
    private BasketUIManager _uiManager;

    [SerializeField]
    private SessionManager _sessionManager;

    [Space]
    [SerializeField]
    private AudioCategory<JudgeAudioID>[] audioCategories;

    private readonly Sprite[] _icons = new Sprite[2];
    private readonly string[] _texts = new string[2];
    private readonly string[] _syllabifiedWords = new string[2];

    private int _currentActivityIndex = 0;

    private JudgeActivity[] _activities;
    private JudgeActivity _currentActivity;

    private readonly Dictionary<JudgeAudioID, AudioClip> _audioClips = new(5);

    public void Start()
    {
        Application.targetFrameRate = 60;

        _answerManager.OnAnswerSelected += HandleAnswerSelected;

        ActivityRequestParams @params = LevelPayload.Params;
        ActivitySkill skill = LevelPayload.Skill;

        if (@params.ChildId is null)
        {
#if !UNITY_EDITOR
            Debug.LogError("Parameters are missing in LevelPayload.");
            return;
# else
            skill = ActivitySkill.Medial;
            @params = new ActivityRequestParams { PlanId = 64, ChildId = "19013454K" };
            Debug.LogWarning("Using default parameters for testing in Unity Editor.");
# endif
        }

        _canvasUI.Init(skill);

        AudioEntry<JudgeAudioID>[] audioEntries = Array.Empty<AudioEntry<JudgeAudioID>>();

        foreach (AudioCategory<JudgeAudioID> category in audioCategories)
        {
            if (category.Id == skill)
            {
                audioEntries = category.Entries;
                break;
            }
        }

        foreach (AudioEntry<JudgeAudioID> entry in audioEntries)
        {
            _audioClips[entry.Id] = entry.Clip;
        }

        LoadActivities(@params).Forget();
    }

    private async UniTaskVoid LoadActivities(ActivityRequestParams @params)
    {
        BasketService basketService = new();

        ActivitiesResult result = await basketService.GetActivities<ActivitiesData<JudgeActivity>>(
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
        _progressBar.Initialize(_activities.Length);

        ChangeActivity();

        _answerManager.DisableAnswers();

        AudioManager.Instance.PlayVoice(_audioClips[JudgeAudioID.Intro]);

        await AudioManager.Instance.WaitVoiceToEnd();

        _sessionManager.StartTime();

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

        JudgeAudioID id = isCorrect
            ? (isPositive ? JudgeAudioID.PositiveAndCorrect : JudgeAudioID.NegativeAndCorrect)
            : (isPositive ? JudgeAudioID.PositiveAndIncorrect : JudgeAudioID.NegativeAndIncorrect);

        AudioManager.Instance.PlayVoice(_audioClips[id]);

        TaskInfo taskInfo = new()
        {
            IsCorrect = isCorrect,
            TaskIndex = _currentActivityIndex,
            SelectedButton = isPositive,
        };

        _sessionManager.RecordActivityResult(in taskInfo);

        _currentActivityIndex++;

        InitCount(answerResult).Forget();
    }

    private async UniTaskVoid InitCount(AnswerEvaluation result)
    {
        FeedbackType feedbackType =
            result == AnswerEvaluation.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

        _progressBar.SetProgress(_currentActivityIndex);

        await UniTask.WaitForSeconds(2f);

        await _canvasUI.ShowFeedback(feedbackType);

        ChangeActivity();

        _sessionManager.StartTime();
    }

    private void ChangeActivity()
    {
        if (_currentActivityIndex >= _activities.Length)
        {
            _sessionManager.EndSession();
            _canvasUI.ShowSummary();

            LevelDataStore instance = LevelDataStore.Instance;

            instance?.LevelCompleted();
            return;
        }

        _currentActivity = _activities[_currentActivityIndex];

        for (int i = 0; i < _texts.Length; i++)
        {
            _texts[i] = _currentActivity.Words[i].Word;
            _icons[i] = LoadSprite(_currentActivity.Words[i].Path);
            _syllabifiedWords[i] = _currentActivity.Words[i].SyllabifiedWord;
        }

        bool leftAnswer = _currentActivity.Answer;
        bool rightAnswer = !leftAnswer;

        Span<bool> answers = stackalloc bool[2] { leftAnswer, rightAnswer };

        AnswerDTO answer = new(answers);
        _hoopManager.SetHoopStates(in answer);

        ViewContentDTO content = new(_icons, _texts);

        _uiManager.SetViewContent(in content);

        ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords);

        _canvasUI.SetFeedbackContent(in feedbackContent);

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
