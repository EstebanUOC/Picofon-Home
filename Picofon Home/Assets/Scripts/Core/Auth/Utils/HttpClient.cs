using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Networking;

public static class HttpClientUnity
{
    public static async Task<string> GetAsync(
        string url,
        int timeoutSeconds = 10,
        CancellationToken cancellationToken = default
    )
    {
        using var request = UnityWebRequest.Get(url);

        request.timeout = timeoutSeconds;

        var op = request.SendWebRequest();

        var registration = cancellationToken.Register(() =>
        {
            if (request != null && !request.isDone)
                request.Abort();
        });

        await AwaitRequest(op, cancellationToken);

        registration.Dispose();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP GET Error: {request.error} | URL: {url}");

        return request.downloadHandler.text;
    }

    public static async Task<string> PostAsync(
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

        var op = request.SendWebRequest();

        var registration = cancellationToken.Register(() =>
        {
            if (request != null && !request.isDone)
                request.Abort();
        });

        await AwaitRequest(op, cancellationToken);

        registration.Dispose();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP POST Error: {request.error} | URL: {url}");

        return request.downloadHandler.text;
    }

    private static Task AwaitRequest(UnityWebRequestAsyncOperation op, CancellationToken ct)
    {
        var tcs = new TaskCompletionSource<bool>();

        op.completed += _ =>
        {
            if (ct.IsCancellationRequested)
                tcs.TrySetCanceled(ct);
            else
                tcs.TrySetResult(true);
        };

        return tcs.Task;
    }
}
