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
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using ActivitiesResult = ApiResult<ActivitiesData<SelectActivity>>;

    public class BasketManagerPS : MonoBehaviour
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
        private AnswerManagerPS _answerManager;

        [SerializeField]
        private HoopManager _hoopManager;

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

        private readonly Sprite[] _icons = new Sprite[4];
        private readonly string[] _texts = new string[4];
        private readonly string[] _syllabifiedWords = new string[4];

        private readonly AudioClip[] _audioClips = new AudioClip[3];

        private readonly AudioClip[] _audioItems = new AudioClip[4];

        // Variables

        private bool _taskCompleted = false;
        private int _currentActivityIndex = 0;

        private SelectActivity[] _activities;
        private SelectActivity _currentActivity;

        private float _defaultMenuX;
        private float _defaultCounterX;
        private float _defaultProgressBarValue;

        public void Start()
        {
            _answerManager.OnHoopSelected += HandleHoopSelected;

            ActivityRequestParams @params = LevelPayload.Params;
            ActivitySkill skill = LevelPayload.Skill;
            LanguageID language = LevelPayload.Language;

            _taskCompleted = LevelPayload.TaskCompleted;

#if DEBUG
            if (@params.ChildId is null)
            {
                skill = ActivitySkill.Initial;
                language = LanguageID.Catalan;
                @params = new ActivityRequestParams { PlanId = 113, ChildId = "12345678Z" };
                PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
            }
# endif

            _feedbackController.Init(skill);

            AudioEntry<ResponseAudioID>[] audioEntries = Array.Empty<AudioEntry<ResponseAudioID>>();

            LoadActivities(@params: @params, skill: skill, language: language).Forget();
        }

        public void OnDestroy()
        {
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
            PositionMenu();

            ActivitiesResult result = await basketService.GetActivities<
                ActivitiesData<SelectActivity>
            >(@params);

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

                TherapyPlanService therapyPlanService = new(0);

                await therapyPlanService.ChangePlanStatus(
                    id: @params.PlanId,
                    status: TherapyStatus.Cancelled
                );

                _ = _fade.Stop(onComplete: () => SceneManager.LoadScene("MapPathScene"));

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

            ActivityLabels labels = new()
            {
                Mechanic = MechanicID.Basket,
                Language = language,
                Skill = skill,
                Activity = "select",
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

            _fade.StopAndZoom();
            AnimateUI().Forget();

            int introIndex = (int)ResponseAudioID.Intro;
            AudioClip introClip = _audioClips[introIndex];

            _uiManager.SetIntroAudio(introClip);

            AudioManager.Instance.PlayVoice(introClip);

            await AudioManager.Instance.WaitVoiceToEnd();

            _uiManager.Prueba();

            await UniTask.WaitForSeconds(0.25f);

            _answerManager.Prueba();

            _sessionManager.StartTime();
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

        private void HandleHoopSelected(int hoopIndex)
        {
            Transform hoopTransform = _hoopManager.GetHoopTransform(hoopIndex);

            WordInfoPS currentWord = _currentActivity.Words[hoopIndex];

            _ballController.LaunchBall(hoopTransform);

            bool isCorrect = currentWord.Answer;

            ResponseAudioID id = isCorrect ? ResponseAudioID.Correct : ResponseAudioID.Incorrect;
            int audioIndex = (int)id;

            AudioManager.Instance.PlayVoice(_audioClips[audioIndex]);

            int correctWordId = 0;
            int correctIndex = 0;

            if (isCorrect)
            {
                correctWordId = currentWord.Id;
                correctIndex = hoopIndex;
            }
            else
            {
                int i = 0;

                foreach (var word in _currentActivity.Words)
                {
                    if (word.Answer)
                    {
                        correctWordId = word.Id;
                        correctIndex = i;
                        break;
                    }

                    i++;
                }
            }

            TaskInfo taskInfo = new()
            {
                IsCorrect = isCorrect,
                TaskIndex = _currentActivityIndex,
                MainAttributeWs = correctWordId,
                CorrectAttributeWs = correctWordId,
                SelectedAttributeWs = currentWord.Id,
            };

            _sessionManager.RecordActivityResult(in taskInfo);

            _currentActivityIndex++;

            _progressBar.SetProgress(progress: _currentActivityIndex, correct: isCorrect);
            _counter.AddScore(correct: isCorrect);

            InitCount(isCorrect, correctIndex).Forget();
        }

        private async UniTaskVoid InitCount(bool isCorrect, int correctIndex)
        {
            FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

            await UniTask.WaitForSeconds(2f);

            await _feedbackController.ShowSL(feedbackType, correctIndex);

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

            Span<bool> answers = stackalloc bool[4];
            for (int i = 0; i < answers.Length; i++)
            {
                answers[i] = _currentActivity.Words[i].Answer;
            }

            AnswerDTO answer = new(answers);
            _hoopManager.SetHoopStates(in answer);

            ViewContentDTO content = new(_icons, _texts, false);

            _uiManager.SetViewContent(in content);

            AudioManager.Instance.GetAudios(_currentActivityIndex, 4, _audioItems);

            _uiManager.SetAudioClips(_audioItems);

            ViewContentDTO feedbackContent = new(_icons, _syllabifiedWords, _texts);

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
                await CalculateLearningRate();
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

            ApiResult<LearningRateData> result = await service.GetLearningRate(
                childId: childId,
                therapyPlan: planId
            );

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

            ApiResult resultCreate = await service.CreateTherapyPlanBulk(data);

            if (!resultCreate.Success)
            {
                await _modalGame.ShowFinal("No s'han pogut carregar els nivells següents.");

                await LoadScene("AuthScene");

                return;
            }

            await _modalGame.ShowFinal("Niveles cargados correctamente, ¡a jugar!");

            await LoadScene("MapPathScene");
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
