namespace Picofon.Activities.Segmentation
{
    using System;
    using Cysharp.Threading.Tasks;
    using Picofon.Activities.Basket;
    using Picofon.Activities.Basket.DTOs.Responses;
    using Picofon.Activities.Feedback;
    using Picofon.Components;
    using Picofon.Core.MapPath;
    using Picofon.Core.Network;
    using Picofon.Utils;
    using PrimeTween;
    using TMPro;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class WordSegmentationManagerRelate : MonoBehaviour
    {
        # region References

        [SerializeField]
        private SpriteRenderer _wordImage;

        [SerializeField]
        private TMP_Text _wordText;

        [Space]
        [SerializeField]
        private GameMenu _gameMenu;

        [SerializeField]
        private Fade _fade;

        [SerializeField]
        private ProgressBar _progressBar;

        [SerializeField]
        private Counter _counter;

        [SerializeField]
        private FeedbackController _feedbackController;

        [Space]
        [SerializeField]
        private RectTransform _progressBarTransform;

        [SerializeField]
        private RectTransform _counterTransform;

        [SerializeField]
        private Transform _labelTransform;

        [Space]
        [SerializeField]
        private RectTransform _menuTransform;

        [SerializeField]
        private Transform _papirusTransform;

        [SerializeField]
        private Transform _handsTransform;

        [SerializeField]
        private SimplePhysicalButton _imageButton;

        // Hand Buttons
        [SerializeField]
        private SimplePhysicalButton _hand1Button;

        [SerializeField]
        private SimplePhysicalButton _hand2Button;

        [SerializeField]
        private SimplePhysicalButton _hand3Button;

        [SerializeField]
        private SimplePhysicalButton _hand4Button;

        [SerializeField]
        private SimplePhysicalButton _hand5Button;

        #endregion

        // Variables

        private DataManager _dataManager;

        private SegmentationActivity _currentActivity;

        private int _syllablesNumber;

        private bool _isMinimized;

        private float _defaultCounterX;
        private float _defaultProgressBarValue;

        private float _defaultMenuX;

        private bool _clueVisible = false;

        private AudioClip _currentWordClip;

        private AudioClip _instructionClip;

        private Vector2[] _handPositions = new Vector2[4];

        public void Awake()
        {
            _dataManager = new DataManager();

            _hand1Button.OnClick += () => HandleAnswer(1);
            _hand2Button.OnClick += () => HandleAnswer(2);
            _hand3Button.OnClick += () => HandleAnswer(3);
            _hand4Button.OnClick += () => HandleAnswer(4);

            _handPositions[0] = _hand1Button.transform.localPosition;
            _handPositions[1] = _hand2Button.transform.localPosition;
            _handPositions[2] = _hand3Button.transform.localPosition;
            _handPositions[3] = _hand4Button.transform.localPosition;

            _imageButton.OnClick += PositionUIForRound;

            _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;

            Tween.Scale(
                target: _labelTransform,
                endValue: Vector3.one * 0,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }

        public void OnDestroy()
        {
            AudioManager.Instance?.UnloadAudios();
        }

        public async void Start()
        {
            SceneOrientationHelper.LockToLandscape();

            ActivitySkill skill = LevelPayload.Skill;
            LanguageID language = LevelPayload.Language;
            ActivityRequestParams @params = LevelPayload.Params;

#if DEBUG
            if (@params.ChildId is null)
            {
                skill = ActivitySkill.Initial;
                language = LanguageID.Spanish;
                @params = new ActivityRequestParams { PlanId = 454, ChildId = "273343238" };
                PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
            }
#endif

            _feedbackController.Init(skill);

            await LoadActivities(@params, language);
        }

        private async UniTask LoadActivities(ActivityRequestParams @params, LanguageID language)
        {
            await UniTask.WaitForEndOfFrame(this);

            _fade.FirstLoad();

            PositionUI();

            ApiResult<SegmentationData> result = await _dataManager.LoadActivities(@params);

            if (!result.Success)
            {
                PerformanceLog.LogError(
                    $"[WordSegmentation] Error loading activities: {result.Message}"
                );

                _fade.StopAndZoom();
                return;
            }

            if (!_dataManager.HasActivities())
            {
                PerformanceLog.LogWarning("[WordSegmentation] No activities found.");

                _fade.StopAndZoom();
                return;
            }

            string[] audioPaths = BuildAudioPaths();

            ActivityLabels labels = new()
            {
                Mechanic = MechanicID.Segmentation,
                Language = language,
                Activity = "a",
            };

            await AudioManager.Instance.LoadAudios(audioPaths, labels);

            _progressBar.Initialize(_dataManager.GetActivityCount(), false);

            _fade.StopAndZoom();
            AnimateUI().Forget();

            SetupRound();

            AudioClip introClip = AudioManager.Instance.GetIntroClip();

            _imageButton.Interactable = false;

            AudioManager.Instance.PlayVoice(introClip);

            await AudioManager.Instance.WaitVoiceToEnd();

            _imageButton.Interactable = true;
        }

        private void PositionUI()
        {
            _defaultMenuX = _menuTransform.anchoredPosition.x;
            _defaultCounterX = _counterTransform.anchoredPosition.x;

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

        private void HideUI()
        {
            _ = Tween.UIAnchoredPositionX(
                target: _menuTransform,
                endValue: -200,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            _ = Tween.UIAnchoredPositionX(
                target: _counterTransform,
                endValue: 400,
                duration: 0.5f,
                ease: Ease.OutCubic
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

        private string[] BuildAudioPaths()
        {
            SegmentationActivity[] activities = _dataManager.GetActivities();

            if (activities == null || activities.Length == 0)
                return Array.Empty<string>();

            string[] paths = new string[activities.Length];

            for (int i = 0; i < activities.Length; i++)
            {
                paths[i] = activities[i].Word.Word;
            }

            return paths;
        }

        private void SetupRound()
        {
            _currentActivity = _dataManager.GetCurrentActivity();

            Sprite icon = LoadSprite(_currentActivity.Word.Path);

            _wordImage.sprite = icon;

            _wordText.text = _currentActivity.Word.Word;

            _syllablesNumber = _currentActivity.Word.SyllableCount;

            _isMinimized = false;

            ViewContentDTO feedbackContent = new(
                new[] { icon },
                new[] { _currentActivity.Word.SyllabifiedWord },
                Array.Empty<string>()
            );

            _feedbackController.SetItemsContent(in feedbackContent, length: 1);

            _currentWordClip = AudioManager.Instance.GetAudio(_dataManager.GetCurrentIndex());

            _instructionClip = AudioManager.Instance.GetSegmentationClips(
                SegmentationAudioID.Instruction,
                0
            );

            _menuTransform.anchoredPosition = new Vector2(-200, _menuTransform.anchoredPosition.y);

            _papirusTransform.localScale = Vector3.one * 1.4f;
            _papirusTransform.localPosition = Vector3.zero;

            _handsTransform.localPosition = new Vector3(14f, _handsTransform.localPosition.y, 0);

            if (_labelTransform.gameObject.activeSelf && _labelTransform.localScale != Vector3.zero)
            {
                _clueVisible = false;

                Tween.Scale(
                    target: _labelTransform,
                    endValue: Vector3.zero,
                    duration: 0.5f,
                    ease: Ease.OutCubic
                );
            }

            SetupHandPosition();
        }

        private void SetupHandPosition()
        {
            bool random = UnityEngine.Random.value > 0.5f;

            switch (_syllablesNumber)
            {
                case 1:
                    _hand1Button.gameObject.SetActive(true);
                    _hand5Button.gameObject.SetActive(false);

                    _hand1Button.transform.localPosition = _handPositions[0];
                    _hand2Button.transform.localPosition = _handPositions[1];
                    _hand3Button.transform.localPosition = _handPositions[2];
                    _hand4Button.transform.localPosition = _handPositions[3];
                    break;
                case 5:
                    _hand5Button.gameObject.SetActive(true);
                    _hand1Button.gameObject.SetActive(false);

                    _hand2Button.transform.localPosition = _handPositions[0];
                    _hand3Button.transform.localPosition = _handPositions[1];
                    _hand4Button.transform.localPosition = _handPositions[2];
                    break;

                default:
                    _hand1Button.gameObject.SetActive(random);
                    _hand5Button.gameObject.SetActive(!random);

                    if (random)
                    {
                        _hand1Button.transform.localPosition = _handPositions[0];
                        _hand2Button.transform.localPosition = _handPositions[1];
                        _hand3Button.transform.localPosition = _handPositions[2];
                        _hand4Button.transform.localPosition = _handPositions[3];
                    }
                    else
                    {
                        _hand2Button.transform.localPosition = _handPositions[0];
                        _hand3Button.transform.localPosition = _handPositions[1];
                        _hand4Button.transform.localPosition = _handPositions[2];
                    }

                    break;
            }
        }

        private void PositionUIForRound()
        {
            PlayWordAudio().Forget();

            if (_isMinimized)
            {
                return;
            }

            Tween.UIAnchoredPositionX(
                target: _menuTransform,
                endValue: _defaultMenuX,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.Scale(
                target: _papirusTransform,
                endValue: Vector3.one * 0.9f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.LocalPosition(
                target: _papirusTransform,
                endValue: new Vector3(-2.5f, 0.12f, 0),
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.LocalPositionX(
                target: _handsTransform,
                endValue: 5.6f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }

        private async UniTaskVoid ResetUIPosition()
        {
            _ = Tween.UIAnchoredPositionX(
                target: _menuTransform,
                endValue: -200,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            _ = Tween.Scale(
                target: _papirusTransform,
                endValue: Vector3.one * 1.4f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            _ = Tween.LocalPosition(
                target: _papirusTransform,
                endValue: Vector3.zero,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            _ = Tween.LocalPositionX(
                target: _handsTransform,
                endValue: 14f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            AudioClip introClip = AudioManager.Instance.GetIntroClip();

            _imageButton.Interactable = false;

            AudioManager.Instance.PlayVoice(introClip);

            await AudioManager.Instance.WaitVoiceToEnd();

            _imageButton.Interactable = true;

            _isMinimized = false;
        }

        private async UniTaskVoid PlayWordAudio()
        {
            AudioManager.Instance.PlayUI(_currentWordClip);

            if (_isMinimized)
            {
                return;
            }

            await AudioManager.Instance.WaitUIToEnd();

            AudioManager.Instance.PlayVoice(_instructionClip);

            _isMinimized = true;
        }

        private void HandleAnswer(int fingersCount)
        {
            bool isCorrect = fingersCount == _syllablesNumber;

            _progressBar.SetProgress(_dataManager.GetCurrentIndex() + 1, isCorrect);

            AudioClip feedbackClip = AudioManager.Instance.GetSegmentationClips(
                isCorrect ? SegmentationAudioID.Positive : SegmentationAudioID.Negative,
                _currentActivity.Word.SyllableCount
            );

            AudioManager.Instance.PlayVoice(feedbackClip);

            _counter.AddScore(isCorrect);

            ShowRoundResult(isCorrect).Forget();
        }

        private async UniTaskVoid ShowRoundResult(bool isCorrect)
        {
            FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

            await UniTask.WaitForSeconds(1.5f);

            await _feedbackController.ShowSegmentation(feedbackType);

            if (_dataManager.MoveNext())
            {
                SetupRound();
            }
            else
            {
                PerformanceLog.Log("[WordSegmentation] All activities completed.");

                HideUI();

                _fade.Load();

                LoadScene("MapPathScene").Forget();
            }
        }

        private void HandleMenuOptionSelected(GameMenuEvent menuEvent)
        {
            switch (menuEvent)
            {
                case GameMenuEvent.Exit:
                    LoadScene("MapPathScene").Forget();
                    break;

                case GameMenuEvent.Clue:
                    _clueVisible = !_clueVisible;

                    int target = _clueVisible ? 1 : 0;

                    Tween.Scale(
                        target: _labelTransform,
                        endValue: Vector3.one * target,
                        duration: 0.5f,
                        ease: Ease.OutCubic
                    );
                    break;

                case GameMenuEvent.Replay:
                    ResetUIPosition().Forget();

                    break;
            }
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

        private Sprite LoadSprite(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

            string file = System.IO.Path.GetFileNameWithoutExtension(path);

            return Resources.Load<Sprite>($"Images/ImgButtons/{file}");
        }
    }
}
