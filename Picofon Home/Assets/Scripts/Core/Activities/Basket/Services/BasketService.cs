using Picofon.Core.MapPath;
using Picofon.Core.Network;
using Picofon.Utils;

namespace Picofon.Activities.Basket.Services
{
    using System.Text.Json;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public readonly struct ActivitiesData<T>
    {
        public readonly T[] Activities { get; init; }
    }

    public class BasketService
    {
        private readonly string UrlBase = ApiConfig.BaseUrl + "questions";

        public async UniTask<ApiResult<T>> GetActivities<T>(
            ActivityRequestParams @params,
            CancellationToken token = default
        )
        {
            string url = $"{UrlBase}/{@params.PlanId}/{@params.ChildId}";

            byte[] rawResponse;

            try
            {
                rawResponse = await HttpClientUnity.GetAsyncBytes(
                    url: url,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception)
            {
                return ApiResult<T>.Fail("Network error occurred while fetching activities.");

                // if (!GamePrefs.DebugMode)
                // {
                //     PerformanceLog.LogError($"Network request failed with error: {e.Message}");
                //
                //     return ApiResult<T>.Fail("Network error occurred while fetching activities.");
                // }
                //
                // PerformanceLog.LogWarning(
                //     "Network request failed. Falling back to local data in Debug Mode."
                // );
                //
                // char activityChar = typeof(T).FullName.ToLower()[50];
                //
                // PerformanceLog.LogWarning($"Activity character determined as: {activityChar}");
                //
                // string streamingPath = System.IO.Path.Combine(
                //     Application.streamingAssetsPath,
                //     $"plan-{activityChar}.json"
                // );
                //
                // string uri = new System.Uri(streamingPath).AbsoluteUri;
                //
                // PerformanceLog.LogWarning($"Loading local data from: {uri}");
                //
                // rawResponse = await HttpClientUnity.GetAsyncBytes(
                //     url: uri,
                //     timeoutSeconds: 5,
                //     cancellationToken: token
                // );
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);

            JsonElement root = doc.RootElement;

            ApiResponseView<T> responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult<T>.Fail(responseView.ErrorMessage);
            }

            return ApiResult<T>.Ok(responseView.Data);
        }
    }
}
