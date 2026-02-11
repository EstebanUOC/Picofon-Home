using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;
using UnityEngine;

public class UserService
{
    private readonly string ChildrenURL = ApiConfig.BaseUrl + "children";

    private readonly string UserURL = ApiConfig.BaseUrl + "auth/login";

    public async UniTask<ApiResult<UserModel>> LoginWithFirebaseToken(
        string firebaseToken,
        CancellationToken token = default
    )
    {
        string url = UserURL;

        byte[] rawResponse;

        LoginRequest loginRequest = new() { FirebaseIdToken = firebaseToken };
        byte[] jsonRequest = JsonHelper.ToBytes(in loginRequest);

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
            Debug.LogError("Network error during login: " + e.Message);
            return ApiResult<UserModel>.Fail("Network error occurred while logging in.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;
        Debug.Log("Login response: " + root.ToString());

        ApiResponseView<UserModel> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<UserModel>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<UserModel>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult<ChildListItemDTO[]>> GetUserChildren(
        string userId,
        CancellationToken token = default
    )
    {
        string url = $"{ChildrenURL}/owner/{userId}?is_active=true";

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
            return ApiResult<ChildListItemDTO[]>.Fail(
                "Network error occurred while fetching activities."
            );
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<ChildListItemDTO[]> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<ChildListItemDTO[]>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<ChildListItemDTO[]>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult> RegisterChild(
        ChildCreateDTO childCreateDTO,
        CancellationToken token = default
    )
    {
        string url = ChildrenURL;

        byte[] rawResponse;

        byte[] jsonRequest = JsonHelper.ToBytes(childCreateDTO);

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
