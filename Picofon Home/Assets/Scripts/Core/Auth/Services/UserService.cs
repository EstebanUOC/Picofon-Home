using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UserService
{
    private const string ChildrenURL =
        "https://ehc-picofon2.techlab.uoc.edu/api/children";

    private const string UserURL =
        "https://ehc-picofon2.techlab.uoc.edu/api/auth/login";

    public async UniTask<UserModel> LoginWithFirebaseToken(
        string firebaseToken,
        CancellationTokenSource cancellationTokenSource = default
    )
    {
        const string URL = UserURL;

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

    public async UniTask<UserRegisterChildResponse> RegisterChild(
        ChildCreateDTO childCreateDTO,
        CancellationTokenSource cancellationTokenSource = default
    )
    {
        string url = $"{ChildrenURL}/";

        string jsonRequest = JsonHelper.ToJson(childCreateDTO);
        string rawResponse = await HttpClientUnity.PostAsync(
            url: url,
            data: jsonRequest,
            cancellationToken: cancellationTokenSource?.Token ?? CancellationToken.None
        );

        var response = JsonHelper.FromJson<UserRegisterChildResponse>(rawResponse);

        // TODO: Handle errors properly
        // if (!response.Success)
        // {
        //     Debug.LogError("Register child failed: " + string.Join(", ", response.Message.Content));
        // }

        return response;
    }
}
