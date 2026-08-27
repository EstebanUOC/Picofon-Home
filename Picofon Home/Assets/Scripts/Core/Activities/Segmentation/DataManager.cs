namespace Picofon.Activities.Segmentation
{
    using Cysharp.Threading.Tasks;
    using Picofon.Activities.Basket.Services;
    using Picofon.Core.MapPath;
    using Picofon.Core.Network;

    public class DataManager
    {
        private SegmentationActivity[] _activities;
        private SegmentationGeneralData _generalData;
        private int _currentIndex;

        public SegmentationActivity GetCurrentActivity() => _activities?[_currentIndex];

        public SegmentationActivity[] GetActivities() => _activities;

        public SegmentationGeneralData GetGeneralData() => _generalData;

        public int GetActivityCount() => _activities?.Length ?? 0;

        public int GetCurrentIndex() => _currentIndex;

        public bool HasActivities() => _activities is { Length: > 0 };

        public bool MoveNext()
        {
            if (_activities == null || _currentIndex >= _activities.Length - 1)
                return false;

            _currentIndex++;
            return true;
        }

        public async UniTask<ApiResult<SegmentationData>> LoadActivities(
            ActivityRequestParams @params
        )
        {
            BasketService service = new();
            ApiResult<SegmentationData> result = await service.GetActivities<SegmentationData>(
                @params
            );

            if (result.Success && result.Data is { Activities: { Length: > 0 } acts })
            {
                _activities = acts;
                _generalData = result.Data.GeneralData;
                _currentIndex = 0;
            }

            return result;
        }

        public void Reset()
        {
            _activities = null;
            _generalData = null;
            _currentIndex = 0;
        }
    }
}
