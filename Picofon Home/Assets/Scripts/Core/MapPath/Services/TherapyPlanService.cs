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

public readonly struct CreateDefaultPlansRequest
{
    public string ChildId { get; init; }
    public string AssignedById { get; init; }
}

public readonly struct TherapyPlanService
{
    private readonly string baseURL;

    public TherapyPlanService(byte _)
    {
        baseURL = ApiConfig.BaseUrl + "therapy";
    }

    public async UniTask<ApiResult<T>> GetAllPlans<T>(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{baseURL}/child/{childId}";

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

            PerformanceLog.LogWarning(
                "Network request failed. Falling back to local data in Debug Mode."
            );

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

        PerformanceLog.Log($"Fetched therapy plans for child {childId}: {root}");
        ApiResponseView<T> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<T>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<T>.Ok(responseView.Data);
    }

    // public async UniTask<ApiResult<T>> CreateDefaultPlans<T>(
    //     string childId,
    //     string assignedById,
    //     CancellationToken token = default
    // )
    // {
    //     string url = $"{baseURL}/default";
    //
    //     byte[] rawResponse;
    //
    //     CreateDefaultPlansRequest request = new()
    //     {
    //         ChildId = childId,
    //         AssignedById = assignedById,
    //     };
    //
    //     byte[] jsonRequest = JsonHelper.ToBytes(in request);
    //
    //     try
    //     {
    //         rawResponse = await HttpClientUnity.PostAsyncBytes(
    //             url: url,
    //             data: jsonRequest,
    //             timeoutSeconds: 5,
    //             cancellationToken: token
    //         );
    //     }
    //     catch (System.Exception)
    //     {
    //         return ApiResult<T>.Fail(
    //             "Network error occurred while creating default therapy plans."
    //         );
    //     }
    //
    //     using JsonDocument doc = JsonDocument.Parse(rawResponse);
    //     JsonElement root = doc.RootElement;
    //
    //     ApiResponseView<T> responseView = new(root);
    //
    //     if (!responseView.Success)
    //     {
    //         PerformanceLog.Log(
    //             $"Failed to create default therapy plans for child {childId}.\n Error: {responseView.ErrorMessage} \n Request: {JsonHelper.ToJson(jsonRequest)}"
    //         );
    //         return ApiResult<T>.Fail(responseView.ErrorMessage);
    //     }
    //
    //     return ApiResult<T>.Ok(responseView.Data);
    // }

    public async UniTask<ApiResult> ChangePlanStatus(
        int id,
        TherapyStatus status,
        CancellationToken token = default
    )
    {
        string url = $"{baseURL}/{id}";

        byte[] rawResponse;

        TherapyPlanStatus req = new() { Status = status };

        byte[] jsonRequest = JsonHelper.ToBytes(in req);

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
            return ApiResult.Fail("Network error occurred while completing the therapy plan.");
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
