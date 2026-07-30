using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public sealed class TherapyPlanBulkData
{
    public string ChildId { get; init; }

    public string AssignedById { get; init; }

    public char Vowel { get; init; }

    public JsonElement Levels { get; init; }
}

public sealed class LearningRateData
{
    public JsonElement Levels { get; init; }
}

public readonly struct CalculateLearningRateRequest
{
    public readonly string ChildId { get; init; }
    public readonly int TherapyPlanId { get; init; }
}

public readonly struct LearningRateService
{
    private readonly string baseURL;

    public LearningRateService(byte _)
    {
        baseURL = ApiConfig.BaseUrl + "segment-levels";
    }

    public async UniTask<ApiResult> CalculateLearningRate(
        string childId,
        int therapyPlan,
        CancellationToken token = default
    )
    {
        string url = $"{baseURL}/post_learning_rate";

        byte[] rawResponse;

        CalculateLearningRateRequest request = new()
        {
            ChildId = childId,
            TherapyPlanId = therapyPlan,
        };

        byte[] jsonRequest = JsonHelper.ToBytes(in request);

        try
        {
            rawResponse = await HttpClientUnity.PostAsyncBytes(
                url: url,
                data: jsonRequest,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception e)
        {
            PerformanceLog.Log(
                $"Error calculating learning rate: {e.Message}, URL: {url}, Payload string: {JsonHelper.ToJson(request)}"
            );

            return ApiResult.Fail("Network error occurred while calculating learning rate.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView responseView = new(root);

        PerformanceLog.Log(
            $"CalculateLearningRate response: {root}, URL: {url}, Payload string: {JsonHelper.ToJson(request)}"
        );

        if (!responseView.Success)
        {
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }

    public async UniTask<ApiResult<LearningRateData>> GetLearningRate(
        string childId,
        int therapyPlan,
        CancellationToken token = default
    )
    {
        string url =
            $"{baseURL}/generate-next-level-params?child_id={childId}&therapy_plan_id={therapyPlan}";

        byte[] rawResponse;

        try
        {
            rawResponse = await HttpClientUnity.GetAsyncBytes(
                url: url,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception e)
        {
            PerformanceLog.Log($"Error fetching learning rate: {e.Message}, URL: {url}");
            return ApiResult<LearningRateData>.Fail(
                "Network error occurred while fetching activities."
            );
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<LearningRateData> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<LearningRateData>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<LearningRateData>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult> CreateTherapyPlanBulk(
        TherapyPlanBulkData payload,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}/therapy/bulk";

        byte[] jsonRequest = JsonHelper.ToBytes(in payload);

        byte[] rawResponse;

        try
        {
            rawResponse = await HttpClientUnity.PostAsyncBytes(
                url: url,
                data: jsonRequest,
                timeoutSeconds: 5,
                cancellationToken: token
            );
        }
        catch (System.Exception e)
        {
            PerformanceLog.Log(
                $"Error creating therapy plan bulk: {e.Message}, URL: {url}, Payload stirng: {JsonHelper.ToJson(payload)}"
            );
            return ApiResult.Fail("Network error occurred while creating therapy plan bulk.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<LearningRateData> responseView = new(root);

        if (!responseView.Success)
        {
            PerformanceLog.Log($"Error creating therapy plan bulk: {responseView.ErrorMessage}");
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }
}
