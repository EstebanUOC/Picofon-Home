using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class UserService
{
    private const string BaseUrl =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/children/owner/";

    public async Task<List<ChildListItemDTO>> GetUserChildren(
        string userId,
        CancellationTokenSource cts
    )
    {
        string url = $"{BaseUrl}/{userId}?is_active=true";

        string textRaw = await HttpClientUnity.GetAsync(url, cancellationToken: cts.Token);
        Debug.Log($"GetUserChildren response: {textRaw}");
        UserChildrenResponse response = JsonHelper.FromJson<UserChildrenResponse>(textRaw);

        return response.Data;
    }
}
