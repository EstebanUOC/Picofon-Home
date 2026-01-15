using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public enum ActivityType : byte
{
    Judge = 1,
    Select = 2,
    Relate = 3,
}

public struct ActivityRequestParams
{
    public int PlanId;
    public string ChildId;
}

public class BasketService
{
    private const string UrlBase = ApiConfig.BaseUrl + "questions";

    public async UniTask<ApiResult<T>> GetActivities<T>(
        ActivityRequestParams @params,
        CancellationToken token = default
    )
    {
        string url = $"{UrlBase}/{@params.PlanId}/{@params.ChildId}";

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
