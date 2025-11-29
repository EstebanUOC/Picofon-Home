using System.Threading;
using System.Threading.Tasks;

public interface ILoadTask
{
    public Task RunAsync(CancellationToken ct);
    public bool IsCritical { get; }
}
