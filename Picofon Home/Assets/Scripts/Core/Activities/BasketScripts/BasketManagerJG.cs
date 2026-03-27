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

    [SerializeField]
    private Counter _counter;

    [SerializeField]
    private Fade _fade;

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

    private bool _taskCompleted = false;
    private int _currentActivityIndex = 0;

    private JudgeActivity[] _activities;
    private JudgeActivity _currentActivity;

    private readonly Dictionary<JudgeAudioID, AudioClip> _audioClips = new(5);

    private readonly AudioClip[] _audioItems = new AudioClip[2];

    public void Start()
    {
        _answerManager.OnAnswerSelected += HandleAnswerSelected;

        ActivityRequestParams @params = LevelPayload.Params;
        ActivitySkill skill = LevelPayload.Skill;

        _taskCompleted = LevelPayload.TaskCompleted;

        if (@params.ChildId is null)
        {
#if !UNITY_EDITOR
            Debug.LogError("Parameters are missing in LevelPayload.");
            return;
# else
            skill = ActivitySkill.Initial;
            @params = new ActivityRequestParams { PlanId = 41, ChildId = "19013454K" };
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

    public void OnDestroy()
    {
        _answerManager.OnAnswerSelected -= HandleAnswerSelected;

        AudioManager.Instance.UnloadAudios();
    }

    private async UniTaskVoid LoadActivities(ActivityRequestParams @params)
    {
        BasketService basketService = new();

        _fade.FirstLoad();

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
            Debug.LogWarning("No activities found for the given parameters.");
            return;
        }

        string[] audioPaths = new string[_activities.Length * 2];

        for (int i = 0; i < _activities.Length; i++)
        {
            JudgeActivity activity = _activities[i];

            for (int j = 0; j < activity.Words.Length; j++)
            {
                string word = activity.Words[j].Word;
                audioPaths[i * 2 + j] = word;
            }
        }

        await AudioManager.Instance.LoadAudios(audioPaths);

        if (!_taskCompleted)
        {
            TherapySessionDTO[] sessions = new TherapySessionDTO[_activities.Length];

            GeneralSessionDTO sessionInfo = new()
            {
                TherapyPlanId = @params.PlanId,
                ChildId = @params.ChildId,
            };

            _sessionManager.InitializeSession(
                sessionInfo: sessionInfo,
                sessionResults: sessions,
                completed: _taskCompleted
            );
        }

        _progressBar.Initialize(parts: _activities.Length, completed: _taskCompleted);

        ChangeActivity();

        _answerManager.DisableAnswers();

        _fade.Stop();

        AudioClip introClip = _audioClips[JudgeAudioID.Intro];

        _uiManager.SetIntroAudio(introClip);
        AudioManager.Instance.PlayVoice(clip: introClip);

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

        _progressBar.SetProgress(progress: _currentActivityIndex, correct: isCorrect);
        _counter.AddScore(correct: isCorrect);

        InitCount(answerResult).Forget();
    }

    private async UniTaskVoid InitCount(AnswerEvaluation result)
    {
        FeedbackType feedbackType =
            result == AnswerEvaluation.Correct ? FeedbackType.Positive : FeedbackType.Neutral;

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

        AudioManager.Instance.GetAudios(_currentActivityIndex, 2, _audioItems);

        _uiManager.SetAudioClips(_audioItems);

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
