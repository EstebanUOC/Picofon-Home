using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

public static class HttpClientUnity
{
    public static async UniTask<string> GetAsync(
        string url,
        int timeoutSeconds = 10,
        CancellationToken cancellationToken = default
    )
    {
        using var request = UnityWebRequest.Get(url);

        request.timeout = timeoutSeconds;

        await request.SendWebRequest().WithCancellation(cancellationToken);

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP GET Error: {request.error} | URL: {url}");

        return request.downloadHandler.text;
    }

    public static async UniTask<string> PostAsync(
        string url,
        string data,
        int timeoutSeconds = 10,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        request.timeout = timeoutSeconds;

        await request.SendWebRequest().WithCancellation(cancellationToken);

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP POST Error: {request.error} | URL: {url}");

        return request.downloadHandler.text;
    }
}
