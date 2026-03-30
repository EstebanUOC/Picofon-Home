using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public class SessionService
{
    private readonly string url = ApiConfig.BaseUrl + "therapy-task-result/bulk";

    public async UniTask<ApiResult> CreateTherapySession(
        GeneralSessionDTO sessionInfo,
        TherapySessionDTO[] sessions,
        CancellationToken token = default
    )
    {
        string url = this.url;

        byte[] rawResponse;

        TherapySessionCreateRequest sessionRequest = new()
        {
            General = sessionInfo,
            Tasks = sessions,
        };

        byte[] jsonRequest = JsonHelper.ToBytes(in sessionRequest);

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
            return ApiResult.Fail("Network error occurred while creating therapy session.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView responseView = new(root);

        if (!responseView.Success)
        {
            PerformanceLog.Log(
                $"Failed to create therapy session \nURL: {url} \nRoot: {root} \nRequest: {JsonHelper.ToJson(sessionRequest)}"
            );
            return ApiResult.Fail(responseView.ErrorMessage);
        }

        return ApiResult.Ok();
    }
}
