using System.Threading;
using BasketResponses;
using Cysharp.Threading.Tasks;

public class BasketService
{
    private const string URLBase =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/";

    public async UniTask<ActivitiesData> GetActivities(CancellationToken token = default)
    {
        string planId = "36";
        string childId = "98765432M";
        string url = $"{URLBase}{planId}/{childId}";

        string rawResponse = await HttpClientUnity.GetAsync(url: url, cancellationToken: token);

        GetActiviesResponse response = JsonHelper.FromJson<GetActiviesResponse>(rawResponse);

        return response.Data;
    }
}
