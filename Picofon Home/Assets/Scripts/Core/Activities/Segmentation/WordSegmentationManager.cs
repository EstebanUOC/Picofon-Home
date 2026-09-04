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

    public class WordSegmentationManager : MonoBehaviour
    {
        # region References

        [SerializeField]
        private HandManager handManager;

        [SerializeField]
        private SimplePhysicalButton _yesButton;

        [SerializeField]
        private SimplePhysicalButton _noButton;

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
        private Transform _buttonsTransform;

        [SerializeField]
        private Transform _handsTransform;

        [SerializeField]
        private SimplePhysicalButton _imageButton;

        #endregion

        // Variables

        private DataManager _dataManager;

        private SegmentationActivity _currentActivity;

        private int _syllablesNumber;

        private int _currentFingers;

        private bool _expectedAnswer;

        private float _defaultCounterX;
        private float _defaultProgressBarValue;

        private float _defaultMenuX;

        private bool _clueVisible = false;

        private static readonly System.Random _rng = new();

        public void Awake()
        {
            _dataManager = new DataManager();

            _yesButton.OnClick += () => HandleAnswer(true);
            _noButton.OnClick += () => HandleAnswer(false);

            _imageButton.OnClick += Test;

            _gameMenu.OnMenuOptionSelected += HandleMenuOptionSelected;

            Tween.Scale(
                target: _labelTransform,
                endValue: Vector3.one * 0,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }

        public async void Start()
        {
            SceneOrientationHelper.LockToLandscape();

            _feedbackController.Init(LevelPayload.Skill);

            ActivityRequestParams @params = LevelPayload.Params;

#if DEBUG
            if (@params.ChildId is null)
            {
                @params = new ActivityRequestParams { PlanId = 454, ChildId = "273343238" };
                PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
            }
#endif

            await LoadActivities(@params);
        }

        private async UniTask LoadActivities(ActivityRequestParams @params)
        {
            await UniTask.WaitForEndOfFrame(this);

            _fade.FirstLoad();

            PositionUI();

            ApiResult<SegmentationData> result = await _dataManager.LoadActivities(@params);

            if (!result.Success)
            {
                Debug.LogError($"[WordSegmentation] Error loading activities: {result.Message}");

                _fade.StopAndZoom();
                return;
            }

            if (!_dataManager.HasActivities())
            {
                Debug.LogWarning("[WordSegmentation] No activities found.");

                _fade.StopAndZoom();
                return;
            }

            Debug.Log(
                $"[WordSegmentation] Loaded {_dataManager.GetActivityCount()} activities successfully."
            );

            _progressBar.Initialize(_dataManager.GetActivityCount(), false);

            _fade.StopAndZoom();
            AnimateUI().Forget();

            SetupRound();
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

            _ = Tween.UIAnchoredPositionX(
                target: _counterTransform,
                endValue: _defaultCounterX,
                duration: 0.5f,
                ease: Ease.OutCubic
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

        private void SetupRound()
        {
            _currentActivity = _dataManager.GetCurrentActivity();

            Sprite icon = LoadSprite(_currentActivity.Word.Path);

            _wordImage.sprite = icon;

            _wordText.text = _currentActivity.Word.Word;

            ViewContentDTO feedbackContent = new(
                new[] { icon },
                new[] { _currentActivity.Word.SyllabifiedWord },
                Array.Empty<string>()
            );

            _syllablesNumber = _currentActivity.Word.SyllablesCount;

            _feedbackController.SetItemsContent(in feedbackContent, length: 1);

            _currentFingers = _rng.Next(0, 6);

            _expectedAnswer = _currentFingers == _syllablesNumber;

            handManager.Fingers = _currentFingers;

            // Test

            _menuTransform.anchoredPosition = new Vector2(-200, _menuTransform.anchoredPosition.y);

            _papirusTransform.localScale = Vector3.one * 1.4f;
            _papirusTransform.localPosition = Vector3.zero;

            _buttonsTransform.localPosition = new Vector3(
                _buttonsTransform.localPosition.x,
                -6.5f,
                0
            );

            _handsTransform.localPosition = new Vector3(14f, _handsTransform.localPosition.y, 0);
        }

        private void Test()
        {
            Tween.UIAnchoredPositionX(
                target: _menuTransform,
                endValue: _defaultMenuX,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.Scale(
                target: _papirusTransform,
                endValue: Vector3.one,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.LocalPosition(
                target: _papirusTransform,
                endValue: new Vector3(-2.5f, 1.3f, 0),
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.LocalPositionY(
                target: _buttonsTransform,
                endValue: -3.55f,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

            Tween.LocalPositionX(
                target: _handsTransform,
                endValue: 6,
                duration: 0.5f,
                ease: Ease.OutCubic
            );
        }

        private void HandleAnswer(bool isYes)
        {
            _yesButton.Interactable = false;
            _noButton.Interactable = false;

            bool isCorrect = isYes == _expectedAnswer;

            Debug.Log(
                $"[WordSegmentation] Word=\"{_currentActivity.Word.Word}\", Syllables={_syllablesNumber}, Fingers={_currentFingers}, Expected={(_expectedAnswer ? "Yes" : "No")}, Selected={(isYes ? "Yes" : "No")} → {(isCorrect ? "CORRECT" : "INCORRECT")}"
            );

            _progressBar.SetProgress(_dataManager.GetCurrentIndex() + 1, isCorrect);
            _counter.AddScore(isCorrect);

            ShowRoundResult(isCorrect).Forget();
        }

        private async UniTaskVoid ShowRoundResult(bool isCorrect)
        {
            FeedbackType feedbackType = isCorrect ? FeedbackType.Positive : FeedbackType.Neutral;

            await UniTask.WaitForSeconds(1.5f);

            await _feedbackController.ShowSegmentation(feedbackType);

            _yesButton.Interactable = true;
            _noButton.Interactable = true;

            if (_dataManager.MoveNext())
            {
                SetupRound();
            }
            else
            {
                Debug.Log("[WordSegmentation] All activities completed.");

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
