namespace Picofon.Activities.Segmentation
{
    using Cysharp.Threading.Tasks;
    using Picofon.Activities.Basket.Services;
    using Picofon.Components;
    using Picofon.Core.MapPath;
    using Picofon.Core.Network;
    using Picofon.Utils;
    using TMPro;
    using UnityEngine;

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

        #endregion

        // Variables

        private DataManager _dataManager;

        private SegmentationActivity _currentActivity;

        private int _currentFingers;

        private static readonly System.Random _rng = new();

        public void Awake()
        {
            _dataManager = new DataManager();

            _yesButton.OnClick += () => HandleAnswer(true);
            _noButton.OnClick += () => HandleAnswer(false);
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
            ApiResult<ActivitiesData<SegmentationActivity>> result =
                await _dataManager.LoadActivities(@params);

            if (!result.Success)
            {
                Debug.LogError($"[WordSegmentation] Error loading activities: {result.Message}");
                return;
            }

            if (!_dataManager.HasActivities())
            {
                Debug.LogWarning("[WordSegmentation] No activities found.");
                return;
            }

            Debug.Log(
                $"[WordSegmentation] Loaded {_dataManager.GetActivityCount()} activities successfully."
            );

            SetupRound();
        }

        private void SetupRound()
        {
            _currentActivity = _dataManager.GetCurrentActivity();

            _wordImage.sprite = LoadSprite(_currentActivity.Word.Path);

            _wordText.text = _currentActivity.Word.Word;

            _currentFingers = _rng.Next(0, 6);

            handManager.Fingers = _currentFingers;
        }

        private void HandleAnswer(bool isYes)
        {
            bool isCorrect = isYes == _currentActivity.Answer;

            Debug.Log(
                $"[WordSegmentation] Word=\"{_currentActivity.Word.Word}\", Answer={_currentActivity.Answer}, Selected={(isYes ? "Yes" : "No")}: {(isCorrect ? "CORRECTO" : "INCORRECTO")}"
            );

            if (_dataManager.MoveNext())
            {
                SetupRound();
            }
            else
            {
                Debug.Log("[WordSegmentation] All activities completed.");
            }
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
