using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public readonly struct OralnitasData
{
    public readonly int CorrectAnswers { get; init; }
}

public readonly struct OralnitasService
{
    private readonly string _url;

    public OralnitasService(byte _)
    {
        _url = ApiConfig.BaseUrl + "therapy-task-result/child";
    }

    public async UniTask<ApiResult<OralnitasData>> GetOralnitas(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{_url}/{childId}/correct-answers";

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
            return ApiResult<OralnitasData>.Fail("Network error while fetching Oralnitas data.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<OralnitasData> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<OralnitasData>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<OralnitasData>.Ok(responseView.Data);
    }
}
