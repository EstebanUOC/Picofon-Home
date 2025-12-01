using System.Threading;
using Cysharp.Threading.Tasks;

public interface ILoadTask
{
    public bool IsCritical { get; }
}

public struct LoadTaskResult<T>
{
    public bool Success;
    public T Result;
}

public interface ILoadTaskGeneric<T> : ILoadTask
{
    public UniTask<LoadTaskResult<T>> RunAsync(
        CancellationToken ct,
        CancellationToken timeoutCt = default
    );
}

public interface ILoadTaskSimple : ILoadTask
{
    public UniTask<bool> RunAsync(CancellationToken ct, CancellationToken timeoutCt = default);
}
