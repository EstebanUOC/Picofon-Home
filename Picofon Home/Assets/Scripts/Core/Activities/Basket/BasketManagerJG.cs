using Picofon.Activities.Basket.DTOs.Responses;
using Picofon.Activities.Basket.Services;
using Picofon.Activities.Feedback;
using Picofon.Components;
using Picofon.Core.MapPath;
using Picofon.Core.MapPath.Services;
using Picofon.Core.Network;
using Picofon.Core.Session.Models;
using Picofon.Utils;

namespace Picofon.Activities.Basket
{
    using System;
    using Cysharp.Threading.Tasks;
    using Picofon.Core.Auth;
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using ActivitiesResult = ApiResult<ActivitiesData<JudgeActivity>>;

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
        #region References

        [Space]
        [SerializeField]
        private FeedbackController _feedbackController;

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

        [Space]
        [SerializeField]
        private RectTransform _progressBarTransform;

        [SerializeField]
        private RectTransform _counterTransform;

        [SerializeField]
        private RectTransform _menuTransform;

        #endregion

        // Readonly fields

        private readonly AudioClip[] _audioClips = new AudioClip[5];

        private readonly AudioClip[] _audioItems = new AudioClip[2];

        private readonly Sprite[] _icons = new Sprite[2];
        private readonly string[] _texts = new string[2];
        private readonly string[] _syllabifiedWords = new string[2];

        // Variables

        private bool _taskCompleted = false;
        private int _currentActivityIndex = 0;

        private JudgeActivity[] _activities;
        private JudgeActivity _currentActivity;

        private float _defaultMenuX;
        private float _defaultCounterX;
        private float _defaultProgressBarValue;

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
                skill = ActivitySkill.Final;
                language = LanguageID.Catalan;
                @params = new ActivityRequestParams { PlanId = 531, ChildId = "00000099F" };

                GamePrefs.DebugMode = true;

                PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
            }
# endif

            _feedbackController.Init(skill);

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

            PositionMenu();
            _fade.FirstLoad();

            ActivitiesResult result = await basketService.GetActivities<
                ActivitiesData<JudgeActivity>
            >(@params);

            if (!result.Success)
            {
                PerformanceLog.LogError($"Error loading activities: {result.Message}");

                _fade.StopAndZoom();

                await _modalGame.ShowWarning();

                return;
            }

            _activities = result.Data.Activities;

            if (_activities is null || _activities.Length == 0)
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
                Mechanic = MechanicID.Basket,
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
            AnimateUI().Forget();

            int introIndex = (int)ResponseAudioID.Intro;
            AudioClip introClip = _audioClips[introIndex];

            _uiManager.SetIntroAudio(introClip);
            AudioManager.Instance.PlayVoice(clip: introClip);

            await AudioManager.Instance.WaitVoiceToEnd();

            _sessionManager.StartTime();

            _answerManager.EnableAnswers();
        }

        private void PositionMenu()
        {
            _defaultMenuX = _menuTransform.anchoredPosition.x;
            _defaultCounterX = _counterTransform.anchoredPosition.x;

            _menuTransform.anchoredPosition = new Vector2(-200, _menuTransform.anchoredPosition.y);
            _counterTransform.anchoredPosition = new Vector2(
                400,
                _counterTransform.anchoredPosition.y
            );

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

        private void HideUI()
        {
            Sequence
                .Create()
                .Group(
                    Tween.UIAnchoredPositionX(
                        target: _menuTransform,
                        endValue: -200,
                        duration: 0.5f,
                        ease: Ease.OutCubic
                    )
                )
                .Group(
                    Tween.UIAnchoredPositionX(
                        target: _counterTransform,
                        endValue: 400,
                        duration: 0.5f,
                        ease: Ease.OutCubic
                    )
                );

            if (_progressBarTransform.rotation.z == 0)
            {
                _ = Tween.UIAnchoredPositionY(
                    target: _progressBarTransform,
                    endValue: 700,
                    duration: 0.5f,
                    ease: Ease.OutCubic
                );
            }
            else
            {
                _ = Tween.UIAnchoredPositionX(
                    target: _progressBarTransform,
                    endValue: -100,
                    duration: 0.5f,
                    ease: Ease.OutCubic
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

        private void HandleAnswerSelected(HoopType hoopType)
        {
            int hoopIndex = (int)hoopType;
            Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

            _ballController.LaunchBall(hoopTransform);

            bool isPositive = hoopType == HoopType.Si;
            bool isCorrect = _currentActivity.Answer == isPositive;

            AnswerEvaluation answerResult = AnswerEvaluation.Correct;

            int variant = 0;

            if (!isCorrect)
            {
                variant = 2;
                answerResult = AnswerEvaluation.Incorrect;
            }

            int audioIndex = hoopIndex + 1 + variant;

            AudioManager.Instance.PlayVoice(_audioClips[audioIndex]);

            WordInfo[] judgeWords = _currentActivity.Words;

            int judgeWordCount = Math.Min(judgeWords.Length, _texts.Length);

            PresentedWordDTO[] presentedWords = new PresentedWordDTO[judgeWordCount];

            for (int i = 0; i < judgeWordCount; i++)
            {
                presentedWords[i] = new PresentedWordDTO
                {
                    WordSegmentId = judgeWords[i].Id,
                    WordRole = i == 0 ? WordRole.Main : WordRole.Comparison,
                    DisplayOrder = i + 1,
                };
            }

            TaskInfo taskInfo = new()
            {
                IsCorrect = isCorrect,
                TaskIndex = _currentActivityIndex,
                PresentedWords = presentedWords,
                SelectedButton = isPositive ? 1 : 0,
            };

            _sessionManager.RecordActivityResult(in taskInfo);

            _currentActivityIndex++;

            _progressBar.SetProgress(progress: _currentActivityIndex, correct: isCorrect);
            _counter.AddScore(correct: isCorrect);

            InitCount(answerResult).Forget();
        }

        private async UniTaskVoid InitCount(AnswerEvaluation result)
        {
            FeedbackType feedbackType = FeedbackType.Positive;

            if (result == AnswerEvaluation.Incorrect)
            {
                feedbackType = FeedbackType.Neutral;
            }

            await UniTask.WaitForSeconds(2f);

            await _feedbackController.Show(feedbackType);

            ChangeActivity();

            _sessionManager.StartTime();
        }

        private void ChangeActivity()
        {
            if (_currentActivityIndex >= _activities.Length)
            {
                EndActivity().Forget();
                HideUI();
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

            ViewContentDTO content = new(_icons, _texts, false);

            _uiManager.SetViewContent(in content);

            AudioManager.Instance.GetAudios(_currentActivityIndex, 2, _audioItems);

            _uiManager.SetAudioClips(_audioItems);

            ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords, true);

            _feedbackController.SetItemsContent(in feedbackContent);

            ResetActivity();
        }

        private async UniTaskVoid EndActivity()
        {
            await _modalGame.ShowSummary();

            _fade.Load();

            LevelDataStore.Instance?.LevelCompleted();

            await _sessionManager.EndSession();

            if (LevelPayload.IsFinalLevel)
            {
                MapPathPayload.NextAction = NextAction.CalculateLR;
            }
            else
            {
                await UniTask.WaitForSeconds(1f);
            }

            await LoadScene("MapPathScene");
        }

        private void ResetActivity()
        {
            _uiManager.Reset();
            _ballController.Reset();
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

        private Sprite LoadSprite(string p)
        {
            string file = System.IO.Path.GetFileNameWithoutExtension(p);
            Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

            if (!s)
                Debug.LogWarning($"No se encontró sprite: {file}");

            return s;
        }
    }
}
