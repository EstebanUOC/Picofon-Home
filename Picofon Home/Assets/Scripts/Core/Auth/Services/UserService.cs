using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public class UserService
{
    private readonly string ChildrenURL = ApiConfig.BaseUrl + "children";

    private readonly string UserURL = ApiConfig.BaseUrl + "auth/login";

    public async UniTask<UserModel> LoginWithFirebaseToken(
        string firebaseToken,
        CancellationTokenSource cancellationTokenSource = default
    )
    {
        string URL = UserURL;

        LoginRequest loginRequest = new() { FirebaseIdToken = firebaseToken };
        string loginRequestJson = JsonHelper.ToJson(loginRequest);

        string rawResponse = await HttpClientUnity.PostAsync(
            url: URL,
            data: loginRequestJson,
            cancellationToken: cancellationTokenSource?.Token ?? CancellationToken.None
        );

        LoginResponse response = JsonHelper.FromJson<LoginResponse>(rawResponse);

        if (!response.Success)
        {
            throw new System.Exception(
                "Login failed: " + string.Join(", ", response.Message.Content)
            );
        }

        return response.Data.User;
    }

    public async UniTask<List<ChildListItemDTO>> GetUserChildren(
        string userId,
        CancellationTokenSource cancellationTokenSource = default
    )
    {
        string url = $"{ChildrenURL}/owner/{userId}?is_active=true";

        string rawResponse = await HttpClientUnity.GetAsync(
            url: url,
            cancellationToken: cancellationTokenSource?.Token ?? CancellationToken.None
        );

        UserChildrenResponse response = JsonHelper.FromJson<UserChildrenResponse>(rawResponse);

        if (!response.Success)
        {
            throw new System.Exception("Get children failed");
        }

        return response.Data;
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
