using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class UserService
{
    private const string BaseUrl =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/children";

    public async Task<List<ChildListItemDTO>> GetUserChildren(
        string userId,
        CancellationTokenSource cts
    )
    {
        string url = $"{BaseUrl}/owner/{userId}?is_active=true";

        string textRaw = await HttpClientUnity.GetAsync(url, cancellationToken: cts.Token);
        UserChildrenResponse response = JsonHelper.FromJson<UserChildrenResponse>(textRaw);

        return response.Data;
    }

    public async Task<UserRegisterChildResponse> RegisterChild(
        ChildCreateDTO childCreateDTO,
        CancellationTokenSource cts
    )
    {
        string url = $"{BaseUrl}/";

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
