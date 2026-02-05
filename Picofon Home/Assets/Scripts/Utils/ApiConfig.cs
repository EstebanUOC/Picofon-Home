using Cysharp.Threading.Tasks;
using UnityEngine;

public static class ApiConfig
{
    public static string BaseUrl = string.Empty;

    public const string PrimeUrl = "https://ehc-picofon.techlab.uoc.edu/";
    public const string FallbackUrl = "https://picofonlab.com/";

    public static async UniTask Ping()
    {
        string url = $"{PrimeUrl}health";

        try
        {
            await HttpClientUnity.GetAsyncBytes(url: url, timeoutSeconds: 5);
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Primary API URL is unreachable. Switching to fallback URL.");
            BaseUrl = $"{FallbackUrl}api/";
            return;
        }

        BaseUrl = $"{PrimeUrl}api/";
    }
}
