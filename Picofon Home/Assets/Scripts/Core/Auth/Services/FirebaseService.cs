using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;

public class FirebaseService : ILoadTaskSimple
{
    public bool IsCritical => true;

    public async UniTask<bool> RunAsync(CancellationToken ct = default)
    {
        var (isCancelled, dependencyStatus) = await FirebaseApp
            .CheckAndFixDependenciesAsync()
            .AsUniTask()
            .AttachExternalCancellation(ct)
            .SuppressCancellationThrow();

        if (isCancelled)
        {
            Debug.LogWarning("Firebase dependency check was cancelled.");
            return false;
        }

        if (dependencyStatus != DependencyStatus.Available)
        {
            Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            return false;
        }

        Debug.Log("Firebase dependencies are available.");

        return true;
    }
}
