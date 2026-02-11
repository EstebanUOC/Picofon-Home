using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;
using UnityEngine;

public readonly struct TherapyData
{
    public readonly TherapyPlan[] Plans { get; init; }
}

public sealed class TherapyPlanService
{
    private readonly string BaseURL = ApiConfig.BaseUrl + "therapy/child";

    public async UniTask<ApiResult<T>> GetAllPlans<T>(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{BaseURL}/{childId}";

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
            if (!GamePrefs.DebugMode)
            {
                return ApiResult<T>.Fail("Network error occurred while fetching therapy plans.");
            }

            Debug.LogWarning("Network request failed. Falling back to local data in Debug Mode.");

            string streamingPath = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                "plans.json"
            );
            string uri = new System.Uri(streamingPath).AbsoluteUri;

            rawResponse = await HttpClientUnity.GetAsyncBytes(
                url: uri,
                timeoutSeconds: 5,
                cancellationToken: token
            );
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
