using Cysharp.Threading.Tasks;

public static class ApiConfig
{
    public static string BaseUrl = PrimeUrl + "api/";

    // TODO: Switch URLs before building for production
    // public const string PrimeUrl = "https://ehc-picofon.techlab.uoc.edu/";
    // public const string FallbackUrl = "https://picofonlab.com/";

    public const string FallbackUrl = "https://ehc-picofon.techlab.uoc.edu/";
    public const string PrimeUrl = "https://picofonlab.com/";

    public static async UniTask<bool> Ping()
    {
        string url = $"{PrimeUrl}health";

        bool primaryUrlReachable = false;

        try
        {
            await HttpClientUnity.GetAsyncBytes(url: url, timeoutSeconds: 5);
            primaryUrlReachable = true;
        }
        catch (System.Exception) { }

        if (primaryUrlReachable)
        {
            BaseUrl = $"{PrimeUrl}api/";
            return true;
        }

        url = $"{FallbackUrl}health";

        try
        {
            await HttpClientUnity.GetAsyncBytes(url: url, timeoutSeconds: 5);
            BaseUrl = $"{FallbackUrl}api/";
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
