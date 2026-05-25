using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public readonly struct CalculateLearningRateRequest
{
    public readonly string ChildId { get; init; }
    public readonly int TherapyPlan { get; init; }
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
            TherapyPlan = therapyPlan,
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
        catch (System.Exception)
        {
            return ApiResult.Fail("Network error occurred while calculating learning rate.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }

    public async UniTask<ApiResult> GetLearningRate(
        string childId,
        int therapyPlan,
        CancellationToken token = default
    )
    {
        string url =
            $"{baseURL}/get-next-level-params?child_id={childId}&therapy_plan_id={therapyPlan}";

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
            return ApiResult.Fail("Network error occurred while fetching activities.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        PerformanceLog.Log($"Get learning rate response \nURL: {url} \nRoot: {root}");

        ApiResponseView responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }
}
