using System;
using System.Collections.Generic;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ActivitiesResult = ApiResult<ActivitiesData<BasketResponses.SelectActivity>>;

public class BasketManagerPS : MonoBehaviour
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
    private AnswerManagerPS _answerManager;

    [SerializeField]
    private HoopManager _hoopManager;

    [SerializeField]
    private BasketUIManager _uiManager;

    [SerializeField]
    private SessionManager _sessionManager;

    [Space]
    [SerializeField]
    private AudioCategory<OthersAudioID>[] audioCategories;

    private bool _taskCompleted = false;
    private int _currentActivityIndex = 0;

    private SelectActivity[] _activities;
    private SelectActivity _currentActivity;

    private readonly Sprite[] _icons = new Sprite[4];
    private readonly string[] _texts = new string[4];
    private readonly string[] _syllabifiedWords = new string[4];

    private readonly Dictionary<OthersAudioID, AudioClip> _audioClips = new(3);

    private readonly AudioClip[] _audioItems = new AudioClip[4];

    public void Start()
    {
        Application.targetFrameRate = 60;

        _answerManager.OnHoopSelected += HandleHoopSelected;

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
            @params = new ActivityRequestParams { PlanId = 42, ChildId = "19013454K" };
            Debug.LogWarning("Using default parameters for testing in Unity Editor.");
# endif
        }

        _canvasUI.Init(skill);

        AudioEntry<OthersAudioID>[] audioEntries = Array.Empty<AudioEntry<OthersAudioID>>();

        foreach (AudioCategory<OthersAudioID> category in audioCategories)
        {
            if (category.Id == skill)
            {
                audioEntries = category.Entries;
                break;
            }
        }

        foreach (AudioEntry<OthersAudioID> entry in audioEntries)
        {
            _audioClips[entry.Id] = entry.Clip;
        }

        LoadActivities(@params).Forget();
    }

    public void OnDestroy()
    {
        AudioManager.Instance.UnloadAudios();
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
            _canvasUI.ShowWarning();
            return;
        }

        string[] audioPaths = new string[_activities.Length * 4];

        for (int i = 0; i < _activities.Length; i++)
        {
            SelectActivity activity = _activities[i];
            int wordCount = activity.Words.Length;

            for (int j = 0; j < wordCount; j++)
            {
                string word = activity.Words[j].Word;
                audioPaths[i * wordCount + j] = word;
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

        AudioClip introClip = _audioClips[OthersAudioID.Intro];

        _uiManager.SetIntroAudio(introClip);
        AudioManager.Instance.PlayVoice(introClip);

        await AudioManager.Instance.WaitVoiceToEnd();

        _uiManager.Prueba();

        await UniTask.WaitForSeconds(0.25f);

        _answerManager.Prueba();

        _sessionManager.StartTime();
    }

    private void HandleHoopSelected(int hoopIndex)
    {
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        WordInfoPS currentWord = _currentActivity.Words[hoopIndex];

        _ballController.LaunchBall(hoopTransform);

        bool isCorrect = currentWord.Answer;

        OthersAudioID id = isCorrect ? OthersAudioID.Positive : OthersAudioID.Negative;

        AudioManager.Instance.PlayVoice(_audioClips[id]);

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
            _canvasUI.ShowSummary();
            _sessionManager.EndSession();

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

        Span<bool> answers = stackalloc bool[4];
        for (int i = 0; i < answers.Length; i++)
        {
            answers[i] = _currentActivity.Words[i].Answer;
        }

        AnswerDTO answer = new(answers);
        _hoopManager.SetHoopStates(in answer);

        ViewContentDTO content = new(_icons, _texts);

        _uiManager.SetViewContent(in content);

        AudioManager.Instance.GetAudios(_currentActivityIndex, 4, _audioItems);

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
