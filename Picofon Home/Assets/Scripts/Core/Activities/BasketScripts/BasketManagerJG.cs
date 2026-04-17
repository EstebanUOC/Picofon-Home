using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using ActivitiesResult = ApiResult<ActivitiesData<BasketResponses.JudgeActivity>>;

public enum HoopType
{
    Si,
    No,
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
    private ModalGame _modalGame;

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

    private readonly Sprite[] _icons = new Sprite[2];
    private readonly string[] _texts = new string[2];
    private readonly string[] _syllabifiedWords = new string[2];

    private bool _taskCompleted = false;
    private int _currentActivityIndex = 0;

    private JudgeActivity[] _activities;
    private JudgeActivity _currentActivity;

    private readonly AudioClip[] _audioClips = new AudioClip[5];

    private readonly AudioClip[] _audioItems = new AudioClip[2];

    public void Start()
    {
        _answerManager.OnAnswerSelected += HandleAnswerSelected;

        ActivityRequestParams @params = LevelPayload.Params;
        ActivitySkill skill = LevelPayload.Skill;
        LanguageID language = LevelPayload.Language;

        _taskCompleted = LevelPayload.TaskCompleted;

#if DEBUG
        if (@params.ChildId is null)
        {
            skill = ActivitySkill.Initial;
            language = LanguageID.Spanish;
            @params = new ActivityRequestParams { PlanId = 112, ChildId = "12345678Z" };
            PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
        }
# endif

        _canvasUI.Init(skill);

        LoadActivities(@params: @params, skill: skill, language: language).Forget();
    }

    public void OnDestroy()
    {
        _answerManager.OnAnswerSelected -= HandleAnswerSelected;

        AudioManager.Instance.UnloadAudios();
    }

    private async UniTaskVoid LoadActivities(
        ActivityRequestParams @params,
        ActivitySkill skill,
        LanguageID language
    )
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
            _fade.StopAndZoom();

            await _modalGame.ShowWarning();

            _fade.Load();
            await UniTask.WaitForSeconds(1f);

            _ = _fade.Stop(onComplete: () => SceneManager.LoadScene("MapPathScene"));

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

        ActivityLabels labels = new()
        {
            Language = language,
            Skill = skill,
            Activity = "judge",
        };

        await AudioManager.Instance.LoadAudios(audioPaths, labels);

        AudioManager.Instance.GetIntroAudios(_audioClips);

        if (!_taskCompleted)
        {
            TherapySessionDTO[] sessions = new TherapySessionDTO[_activities.Length];

            GeneralSessionDTO sessionInfo = new()
            {
                TherapyPlanId = @params.PlanId,
                ChildId = @params.ChildId,
                Console = "MO",
                ConductedById = @params.ConductedById,
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

        _fade.StopAndZoom();

        int introIndex = (int)ResponseAudioID.Intro;
        AudioClip introClip = _audioClips[introIndex];

        _uiManager.SetIntroAudio(introClip);
        AudioManager.Instance.PlayVoice(clip: introClip);

        await AudioManager.Instance.WaitVoiceToEnd();

        _sessionManager.StartTime();

        _answerManager.EnableAnswers();
    }

    private void HandleAnswerSelected(HoopType hoopType)
    {
        int hoopIndex = hoopType == HoopType.Si ? 0 : 1;
        Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

        _ballController.LaunchBall(hoopTransform);

        bool isPositive = hoopType == HoopType.Si;
        bool isCorrect = _currentActivity.Answer == isPositive;

        AnswerEvaluation answerResult = isCorrect
            ? AnswerEvaluation.Correct
            : AnswerEvaluation.Incorrect;

        ResponseAudioID id = isCorrect ? ResponseAudioID.Correct : ResponseAudioID.Incorrect;

        int variant = hoopIndex;

        int audioIndex = (int)id + variant;

        AudioManager.Instance.PlayVoice(_audioClips[audioIndex]);

        TaskInfo taskInfo = new()
        {
            IsCorrect = isCorrect,
            TaskIndex = _currentActivityIndex,
            SelectedButton = isPositive,
            CorrectAttributeWs = _currentActivity.Words[0].Id,
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
            EndActivity().Forget();
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

    private async UniTaskVoid EndActivity()
    {
        await _modalGame.ShowSummary();

        _fade.Load();
        LevelDataStore.Instance?.LevelCompleted();

        await _sessionManager.EndSession();
        await UniTask.WaitForSeconds(1f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("MapPathScene");
        loadOperation.allowSceneActivation = false;

        await UniTask.WaitUntil(() => loadOperation.progress >= 0.9f);

        _ = _fade.Stop(
            target: loadOperation,
            onComplete: target => target.allowSceneActivation = true
        );
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
