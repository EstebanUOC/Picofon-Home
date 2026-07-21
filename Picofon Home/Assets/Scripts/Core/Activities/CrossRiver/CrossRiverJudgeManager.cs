using System;
using BasketResponses;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using ActivitiesResult = ApiResult<ActivitiesData<BasketResponses.JudgeActivity>>;

public class CrossRiverJudgeManager : MonoBehaviour
{
    #region References

    [Space]
    [SerializeField]
    private FrameManager _frameManager;

    [SerializeField]
    private FloatManager _floatManager;

    [SerializeField]
    private Fade _fade;

    [SerializeField]
    private FeedbackController _feedbackController;

    [SerializeField]
    private ModalGame _modalGame;

    [SerializeField]
    private ProgressBar _progressBar;

    [SerializeField]
    private Counter _counter;

    [SerializeField]
    private SessionManager _sessionManager;

    [Space]
    [SerializeField]
    private RectTransform _progressBarTransform;

    [SerializeField]
    private RectTransform _counterTransform;

    [SerializeField]
    private RectTransform _menuTransform;

    #endregion

    // Readonly fields

    private readonly AudioClip[] _feedbackClips = new AudioClip[5];
    private readonly AudioClip[] _wordClips = new AudioClip[2];

    private readonly Sprite[] _icons = new Sprite[2];
    private readonly string[] _texts = new string[2];
    private readonly string[] _syllabifiedWords = new string[2];

    // Variables

    private DataManager _dataManager;

    private JudgeActivity _currentActivity;

    private bool _taskCompleted;

    private float _defaultMenuX;
    private float _defaultCounterX;
    private float _defaultProgressBarValue;

    public void Awake()
    {
        _dataManager = new DataManager();
        _floatManager.OnFloatClicked += HandleFloatClicked;
    }

    public void OnDestroy()
    {
        AudioManager.Instance?.UnloadAudios();
    }

    public async void Start()
    {
        ActivityRequestParams @params = LevelPayload.Params;
        ActivitySkill skill = LevelPayload.Skill;
        LanguageID language = LevelPayload.Language;
        _taskCompleted = LevelPayload.TaskCompleted;

#if DEBUG
        if (@params.ChildId is null)
        {
            skill = ActivitySkill.Initial;
            language = LanguageID.Catalan;
            @params = new ActivityRequestParams { PlanId = 112, ChildId = "12345678Z" };
            PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
        }
#endif

        SceneOrientationHelper.LockToLandscape();
        _feedbackController.Init(skill);

        await LoadActivities(@params, skill, language);
    }

    private async UniTask LoadActivities(
        ActivityRequestParams @params,
        ActivitySkill skill,
        LanguageID language
    )
    {
        _fade.FirstLoad();

        PositionMenu();

        ActivitiesResult result = await _dataManager.LoadActivities(@params);

        if (!result.Success)
        {
            PerformanceLog.LogError($"Error loading activities: {result.Message}");

            _fade.StopAndZoom();

            await _modalGame.ShowWarning();
            return;
        }

        if (!_dataManager.HasActivities())
        {
            _fade.StopAndZoom();
            await _modalGame.ShowWarning();

            _fade.Load();
            await UniTask.WaitForSeconds(1f);

            TherapyPlanService therapyPlanService = new(0);

            await therapyPlanService.ChangePlanStatus(
                id: @params.PlanId,
                status: TherapyStatus.Cancelled
            );

            _ = _fade.Stop(onComplete: () => SceneManager.LoadScene("MapPathScene"));
            return;
        }

        string[] audioPaths = BuildAudioPaths();

        ActivityLabels labels = new()
        {
            Language = language,
            Skill = skill,
            Activity = "judge",
        };

        await AudioManager.Instance.LoadAudios(audioPaths, labels);
        AudioManager.Instance.GetIntroAudios(_feedbackClips);

        if (!_taskCompleted)
        {
            int count = _dataManager.GetActivityCount();

            TherapySessionDTO[] sessions = new TherapySessionDTO[count];

            GeneralSessionDTO sessionInfo = new()
            {
                TherapyPlanId = @params.PlanId,
                ChildId = @params.ChildId,
                Console = "MO",
                ConductedById = @params.ConductedById,
            };

            _sessionManager.InitializeSession(sessionInfo, sessions, _taskCompleted);
        }

        SetupRound();
        _progressBar.Initialize(_dataManager.GetActivityCount(), _taskCompleted);

        _fade.StopAndZoom();
        AnimateUI().Forget();

        _currentActivity = _dataManager.GetCurrentActivity();

        int introIndex = (int)ResponseAudioID.Intro;
        AudioClip introClip = _feedbackClips[introIndex];

        AudioManager.Instance.PlayVoice(introClip);

        await AudioManager.Instance.WaitVoiceToEnd();

        _sessionManager.StartTime();
        _floatManager.EnableInteraction();
    }

    private void PositionMenu()
    {
        _defaultMenuX = _menuTransform.anchoredPosition.x;
        _defaultCounterX = _counterTransform.anchoredPosition.x;

        _menuTransform.anchoredPosition = new Vector2(-200, _menuTransform.anchoredPosition.y);
        _counterTransform.anchoredPosition = new Vector2(400, _counterTransform.anchoredPosition.y);

        if (_progressBarTransform.rotation.z == 0)
        {
            _defaultProgressBarValue = _progressBarTransform.anchoredPosition.y;

            _progressBarTransform.anchoredPosition = new Vector2(
                _progressBarTransform.anchoredPosition.x,
                700
            );
        }
        else
        {
            _defaultProgressBarValue = _progressBarTransform.anchoredPosition.x;

            _progressBarTransform.anchoredPosition = new Vector2(
                -100,
                _progressBarTransform.anchoredPosition.y
            );
        }
    }

    private async UniTaskVoid AnimateUI()
    {
        await UniTask.WaitForSeconds(1f);

        _ = Sequence
            .Create()
            .Group(
                Tween.UIAnchoredPositionX(
                    target: _menuTransform,
                    endValue: _defaultMenuX,
                    duration: 0.5f,
                    ease: Ease.OutCubic
                )
            )
            .Group(
                Tween.UIAnchoredPositionX(
                    target: _counterTransform,
                    endValue: _defaultCounterX,
                    duration: 0.5f,
                    ease: Ease.OutCubic
                )
            );

        if (_progressBarTransform.rotation.z == 0)
        {
            _ = Tween.UIAnchoredPositionY(
                target: _progressBarTransform,
                endValue: _defaultProgressBarValue,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }
        else
        {
            _ = Tween.UIAnchoredPositionX(
                target: _progressBarTransform,
                endValue: _defaultProgressBarValue,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }
    }

    private string[] BuildAudioPaths()
    {
        JudgeActivity[] activities = _dataManager.GetActivities();

        if (activities == null || activities.Length == 0)
            return Array.Empty<string>();

        int wordCount = 2;
        string[] paths = new string[activities.Length * wordCount];

        for (int i = 0; i < activities.Length; i++)
        {
            WordInfo[] words = activities[i].Words;

            for (int j = 0; j < wordCount && j < words.Length; j++)
            {
                paths[i * wordCount + j] = words[j].Word;
            }
        }

        return paths;
    }

    private void SetupRound()
    {
        _currentActivity = _dataManager.GetCurrentActivity();

        WordInfo word0 = _currentActivity.Words[0];
        WordInfo word1 = _currentActivity.Words[1];

        _icons[0] = LoadSprite(word0.Path);
        _icons[1] = LoadSprite(word1.Path);

        _syllabifiedWords[0] = word0.SyllabifiedWord;
        _syllabifiedWords[1] = word1.SyllabifiedWord;

        // Configure frames

        _frameManager.ShowLeftFrame(word0.Word, _icons[0]);
        _frameManager.ShowRightFrame(word1.Word, _icons[1]);

        // Configure feedback

        ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords, true);

        _feedbackController.SetItemsContent(in feedbackContent);

        AudioManager.Instance.GetAudios(_dataManager.GetCurrentIndex(), 2, _wordClips);
    }

    private void HandleFloatClicked(int floatIndex)
    {
        _floatManager.DisableInteraction();

        bool isPositive = (floatIndex & 1) == 0;
        bool isCorrect = _currentActivity.Answer == isPositive;

        WordInfo word = _currentActivity.Words[0];

        ResponseAudioID id = isCorrect ? ResponseAudioID.Correct : ResponseAudioID.Incorrect;

        AudioManager.Instance.PlayVoice(_feedbackClips[(int)id]);

        TaskInfo taskInfo = new()
        {
            IsCorrect = isCorrect,
            TaskIndex = _dataManager.GetCurrentIndex(),
            MainAttributeWs = word?.Id,
            SelectedButton = isPositive,
        };

        _sessionManager.RecordActivityResult(taskInfo);

        _progressBar.SetProgress(_dataManager.GetCurrentIndex() + 1, isCorrect);
        _counter.AddScore(isCorrect);

        ShowRoundResult(isCorrect).Forget();
    }

    private async UniTaskVoid ShowRoundResult(bool isCorrect)
    {
        FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

        await UniTask.WaitForSeconds(1.5f);

        await _feedbackController.Show(feedbackType);

        if (_dataManager.MoveNext())
        {
            SetupRound();
            _sessionManager.StartTime();
            _floatManager.EnableInteraction();
        }
        else
        {
            EndActivity().Forget();
        }
    }

    private async UniTaskVoid EndActivity()
    {
        await _modalGame.ShowSummary();

        _fade.Load();

        LevelDataStore.Instance?.LevelCompleted();

        await _sessionManager.EndSession();

        if (LevelPayload.IsFinalLevel)
        {
            await CalculateLearningRate();
        }
        else
        {
            await UniTask.WaitForSeconds(1f);
        }

        await LoadScene("MapPathScene");
    }

    private async UniTask LoadScene(string sceneName)
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        await UniTask.WaitUntil(() => loadOperation.progress >= 0.9f);

        _ = _fade.Stop(
            target: loadOperation,
            onComplete: target => target.allowSceneActivation = true
        );
    }

    private async UniTask CalculateLearningRate()
    {
        if (!LevelPayload.IsAIEnabled)
        {
            await _modalGame.ShowFinal("Has completat tots els nivells, felicitats!");
            await LoadScene("AuthScene");
            return;
        }

        LearningRateService service = new(0);
        string childId = LevelPayload.Params.ChildId;
        int planId = LevelPayload.Params.PlanId;

        ApiResult<LearningRateData> result = await service.GetLearningRate(childId, planId);

        if (!result.Success)
        {
            await _modalGame.ShowFinal("No s'han pogut carregar els nivells següents.");
            await LoadScene("AuthScene");
            return;
        }

        TherapyPlanBulkData data = new()
        {
            ChildId = childId,
            AssignedById = LevelPayload.Params.ConductedById,
            Vowel = LevelPayload.Vowel,
            Levels = result.Data.Levels,
        };

        ApiResult createResult = await service.CreateTherapyPlanBulk(data);

        if (!createResult.Success)
        {
            await _modalGame.ShowFinal("No s'han pogut carregar els nivells següents.");
            await LoadScene("AuthScene");
            return;
        }

        await _modalGame.ShowFinal("Niveles cargados correctamente, ¡a jugar!");
        await LoadScene("MapPathScene");
    }

    private Sprite LoadSprite(string path)
    {
        if (string.IsNullOrEmpty(path))
            return null;

        string file = System.IO.Path.GetFileNameWithoutExtension(path);

        return Resources.Load<Sprite>($"Images/ImgButtons/{file}");
    }
}
