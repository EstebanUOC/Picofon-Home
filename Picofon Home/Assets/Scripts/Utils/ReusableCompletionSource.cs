using System;
using System.Threading;
using Cysharp.Threading.Tasks;

public sealed class ReusableCompletionSource<T> : IUniTaskSource<T>
{
    public UniTaskCompletionSourceCore<T> core;

    public short Version => core.Version;

    public UniTask<T> Task => new(this, core.Version);

    public bool TrySetResult(T value) => core.TrySetResult(value);

    public bool TrySetException(Exception ex) => core.TrySetException(ex);

    public bool TrySetCanceled(CancellationToken ct) => core.TrySetCanceled(ct);

    public bool TrySetCanceled() => core.TrySetCanceled();

    public void Reset() => core.Reset();

    public T GetResult(short token) => core.GetResult(token);

    public UniTaskStatus GetStatus(short token) => core.GetStatus(token);

    public UniTaskStatus UnsafeGetStatus() => core.UnsafeGetStatus();

    public void OnCompleted(Action<object> continuation, object state, short token) =>
        core.OnCompleted(continuation, state, token);

    void IUniTaskSource.GetResult(short token) => core.GetResult(token);
}
