using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;
using UnityEngine;

public readonly struct LoginData
{
    public readonly UserModel User { get; init; }
}

public readonly struct UpdateUserRoleRequest
{
    public readonly UserRole Role { get; init; }
}

public class UserService
{
    private readonly string ChildrenURL = ApiConfig.BaseUrl + "children/";

    public async UniTask<ApiResult<LoginData>> LoginWithFirebaseToken(
        string firebaseToken,
        CancellationToken token = default
    )
    {
        string url = $"{ApiConfig.BaseUrl}auth/login";

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
        catch (System.Exception)
        {
            return ApiResult<LoginData>.Fail("Network error occurred while logging in.");
        }

        using JsonDocument doc = JsonDocument.Parse(rawResponse);
        JsonElement root = doc.RootElement;

        ApiResponseView<LoginData> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<LoginData>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<LoginData>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult> UpdateUserRole(
        string userId,
        UserRole newRole,
        CancellationToken token = default
    )
    {
        if (newRole == UserRole.Admin)
        {
            return ApiResult.Fail("Cannot assign Admin role through this method.");
        }

        string url = $"{ApiConfig.BaseUrl}/users/{userId}/role";

        byte[] rawResponse;

        UpdateUserRoleRequest requestData = new() { Role = newRole };

        byte[] jsonRequest = JsonHelper.ToBytes(requestData);

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
            return ApiResult.Fail("Network error occurred while updating user role.");
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

    public async UniTask<ApiResult<ChildListItemDTO[]>> GetUserChildren(
        string userId,
        CancellationToken token = default
    )
    {
        string url = $"{ChildrenURL}/user/{userId}";

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
            return ApiResult.Fail("Network error occurred while registering child.");
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
