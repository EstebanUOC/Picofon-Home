using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public class TherapyPlanService
{
    private const string BaseURL = ApiConfig.BaseUrl + "therapy/child";

    public async UniTask<ApiResult<T>> GetAllPlans<T>(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{BaseURL}/{childId}";

        byte[] rawResponse = await HttpClientUnity.GetAsyncBytes(
            url: url,
            cancellationToken: token
        );

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
