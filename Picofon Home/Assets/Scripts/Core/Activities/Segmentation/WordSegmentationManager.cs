namespace Picofon.Activities.Segmentation
{
    using Cysharp.Threading.Tasks;
    using Picofon.Activities.Basket.Services;
    using Picofon.Components;
    using Picofon.Core.MapPath;
    using Picofon.Core.Network;
    using Picofon.Utils;
    using UnityEngine;

    public class WordSegmentationManager : MonoBehaviour
    {
        private DataManager _dataManager;

        public void Awake()
        {
            _dataManager = new DataManager();
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

            PrintActivities();
        }

        private void PrintActivities()
        {
            SegmentationActivity[] activities = _dataManager.GetActivities();

            for (int i = 0; i < activities.Length; i++)
            {
                SegmentationActivity activity = activities[i];

                Debug.Log(
                    $"[WordSegmentation] Activity {i}: Answer={activity.Answer}, Word={activity.Word.Word} ({activity.Word.SyllabifiedWord}), Fingers={activity.Fingers.Word}"
                );
            }
        }
    }
}
