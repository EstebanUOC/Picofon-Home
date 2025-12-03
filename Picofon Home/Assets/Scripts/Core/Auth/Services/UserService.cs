using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class UserService
{
    private const string ChildrenURL =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/children";

    private const string UserURL =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/auth/login";

    public async UniTask<UserModel> LoginWithFirebaseToken(
        string firebaseToken,
        CancellationTokenSource cancellationTokenSource = default
    )
    {
        const string URL = UserURL;

        LoginRequest loginRequest = new() { FirebaseIdToken = firebaseToken };
        string loginRequestJson = JsonHelper.ToJson(loginRequest);

        // string rawResponse = await HttpClientUnity.PostAsync(
        //     url: URL,
        //     data: loginRequestJson,
        //     cancellationToken: cancellationTokenSource.Token
        // );
        //
        // LoginResponse response = JsonHelper.FromJson<LoginResponse>(rawResponse);

        using var request = new UnityWebRequest(URL, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(loginRequestJson);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        await request.SendWebRequest();

        string rawResponse = request.downloadHandler.text;

        LoginResponse response = JsonHelper.FromJson<LoginResponse>(rawResponse);

        return response.Data;
    }

    public async Task<List<ChildListItemDTO>> GetUserChildren(
        string userId,
        CancellationTokenSource cts
    )
    {
        string url = $"{ChildrenURL}/owner/{userId}?is_active=true";

        string textRaw = await HttpClientUnity.GetAsync(url: url, cancellationToken: cts.Token);
        UserChildrenResponse response = JsonHelper.FromJson<UserChildrenResponse>(textRaw);

        return response.Data;
    }

    public async Task<UserRegisterChildResponse> RegisterChild(
        ChildCreateDTO childCreateDTO,
        CancellationTokenSource cts
    )
    {
        string url = $"{ChildrenURL}/";

        string jsonRequest = JsonHelper.ToJson(childCreateDTO);
        string textRaw = await HttpClientUnity.PostAsync(
            url,
            data: jsonRequest,
            cancellationToken: cts.Token
        );

        var response = JsonHelper.FromJson<UserRegisterChildResponse>(textRaw);

        return response;
    }
}
