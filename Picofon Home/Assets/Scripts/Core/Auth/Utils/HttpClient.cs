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

        cancellationToken.Register(() =>
        {
            if (!request.isDone)
                request.Abort();
        });

        await AwaitRequest(op, cancellationToken);

        if (request.result != UnityWebRequest.Result.Success)
            throw new Exception($"HTTP GET Error: {request.error} | URL: {url}");

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
