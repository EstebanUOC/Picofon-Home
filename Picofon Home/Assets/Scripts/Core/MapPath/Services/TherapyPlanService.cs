using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;
using UnityEngine;

public readonly struct TherapyData
{
    public readonly TherapyPlan[] Plans { get; init; }
}

public readonly struct TherapyPlanStatus
{
    public TherapyStatus Status { get; init; }
}

public sealed class TherapyPlanService
{
    private readonly string BaseURL = ApiConfig.BaseUrl + "therapy";

    public async UniTask<ApiResult<T>> GetAllPlans<T>(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{BaseURL}/child/{childId}";

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

    public async UniTask<ApiResult> CompletePlan(int id, CancellationToken token = default)
    {
        string url = $"{BaseURL}/{id}";

        byte[] rawResponse;

        TherapyPlanStatus status = new() { Status = TherapyStatus.Completed };

        byte[] jsonRequest = JsonHelper.ToBytes(in status);

        try
        {
            rawResponse = await HttpClientUnity.PatchAsyncBytes(
                url: url,
                data: jsonRequest,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception)
        {
            if (!GamePrefs.DebugMode)
            {
                return ApiResult.Fail("Network error occurred while fetching therapy plans.");
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

        ApiResponseView responseView = new(root);

        if (!responseView.Success)
        {
            PerformanceLog.Log(
                $"Failed to complete therapy plan with id {id}. Error: {responseView.ErrorMessage} \n Request: {JsonHelper.ToJson(jsonRequest)}"
            );
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }
}
