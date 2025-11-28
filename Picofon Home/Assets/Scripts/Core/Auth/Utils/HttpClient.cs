using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
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

        await AwaitRequest(op, request, cancellationToken);

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

        await AwaitRequest(op, request, cancellationToken);

        registration.Dispose();

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP POST Error: {request.error} | URL: {url}");

        return request.downloadHandler.text;
    }

    private static Task AwaitRequest(
        UnityWebRequestAsyncOperation op,
        UnityWebRequest request,
        CancellationToken ct
    )
    {
        if (ct.IsCancellationRequested)
        {
            try
            {
                request?.Abort();
            }
            catch { }
            return Task.FromCanceled(ct);
        }

        var tcs = new TaskCompletionSource<bool>();

        CancellationTokenRegistration registration = default;

        void callbackComplete(AsyncOperation _)
        {
            registration.Dispose();

            if (ct.IsCancellationRequested)
                tcs.TrySetCanceled(ct);
            else
                tcs.TrySetResult(true);

            op.completed -= callbackComplete;
        }

        op.completed += callbackComplete;

        registration = ct.Register(() =>
        {
            try
            {
                request?.Abort();
            }
            catch { }

            tcs.TrySetCanceled(ct);

            op.completed -= callbackComplete;
        });

        return tcs.Task;
    }
}
