namespace Picofon.Activities.Segmentation
{
    using Cysharp.Threading.Tasks;
    using Picofon.Activities.Basket;
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

        [Space]
        [SerializeField]
        private RectTransform _progressBarTransform;

        [SerializeField]
        private RectTransform _counterTransform;

        [SerializeField]
        private RectTransform _menuTransform;

        [SerializeField]
        private Transform _labelTransform;

        #endregion

        // Variables

        private DataManager _dataManager;

        private SegmentationActivity _currentActivity;

        private int _syllablesNumber;

        private int _currentFingers;

        private bool _expectedAnswer;

        private float _defaultMenuX;
        private float _defaultCounterX;
        private float _defaultProgressBarValue;

        private bool _clueVisible = false;

        private static readonly System.Random _rng = new();

        public void Awake()
        {
            _dataManager = new DataManager();

            _yesButton.OnClick += () => HandleAnswer(true);
            _noButton.OnClick += () => HandleAnswer(false);

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

            ActivityRequestParams @params = LevelPayload.Params;

#if DEBUG
            if (@params.ChildId is null)
            {
                @params = new ActivityRequestParams { PlanId = 441, ChildId = "99345678A" };
                PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
            }
#endif

            await LoadActivities(@params);
        }

        private async UniTask LoadActivities(ActivityRequestParams @params)
        {
            await UniTask.WaitForEndOfFrame(this);

            _fade.FirstLoad();

            PositionMenu();

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

            _syllablesNumber = _dataManager.GetGeneralData()?.SyllablesNumber ?? 0;

            Debug.Log(
                $"[WordSegmentation] Loaded {_dataManager.GetActivityCount()} activities successfully."
            );

            _progressBar.Initialize(_dataManager.GetActivityCount(), false);

            _fade.StopAndZoom();
            AnimateUI().Forget();

            SetupRound();
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

            _ = Tween.UIAnchoredPositionX(
                target: _menuTransform,
                endValue: _defaultMenuX,
                duration: 0.5f,
                ease: Ease.OutCubic
            );

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

            _wordImage.sprite = LoadSprite(_currentActivity.Word.Path);

            _wordText.text = _currentActivity.Word.Word;

            _currentFingers = _rng.Next(0, 6);

            _expectedAnswer = _currentFingers == _syllablesNumber;

            handManager.Fingers = _currentFingers;
        }

        private void HandleAnswer(bool isYes)
        {
            bool isCorrect = isYes == _expectedAnswer;

            Debug.Log(
                $"[WordSegmentation] Word=\"{_currentActivity.Word.Word}\", Syllables={_syllablesNumber}, Fingers={_currentFingers}, Expected={(_expectedAnswer ? "Yes" : "No")}, Selected={(isYes ? "Yes" : "No")} → {(isCorrect ? "CORRECT" : "INCORRECT")}"
            );

            _progressBar.SetProgress(_dataManager.GetCurrentIndex() + 1, isCorrect);
            _counter.AddScore(isCorrect);

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
