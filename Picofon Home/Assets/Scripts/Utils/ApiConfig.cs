using Cysharp.Threading.Tasks;
using UnityEngine;

public static class ApiConfig
{
    public static string BaseUrl = string.Empty;

    public const string PrimeUrl = "https://ehc-picofon.techlab.uoc.edu/api/";
    public const string FallbackUrl = "https://picofon-api.fly.dev/api/";

    public static async UniTask Ping()
    {
        string url = $"{PrimeUrl}auth";

        try
        {
            await HttpClientUnity.GetAsyncBytes(url: url, timeoutSeconds: 5);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Primary API URL is unreachable. Switching to fallback URL.");
            Debug.LogWarning(e.Message);
            BaseUrl = FallbackUrl;
            return;
        }

        BaseUrl = PrimeUrl;
    }
}
