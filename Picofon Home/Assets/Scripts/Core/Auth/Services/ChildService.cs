using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public readonly struct ChildService
{
    public async UniTask<ApiResult<ChildDataDTO>> GetChild(
        string childId,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}/children/{childId}";

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
            return ApiResult<ChildDataDTO>.Fail(
                "Network error occurred while fetching activities."
            );
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<ChildDataDTO> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<ChildDataDTO>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<ChildDataDTO>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult> UpdateChild(
        string childId,
        CreateChildDTO updateData,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}/children/{childId}";

        byte[] rawResponse;

        byte[] jsonRequest = JsonHelper.ToBytes(updateData);

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
            return ApiResult.Fail("Network error occurred while fetching activities.");
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
}
