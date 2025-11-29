using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseService : ILoadTask
{
    public bool IsCritical => true;

    private FirebaseAuth auth;

    public FirebaseService()
    {
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    Debug.Log("Firebase ready for authentication.");
                }
                else
                {
                    Debug.LogError("Firebase dependencies not available: " + status);
                }
            });
    }

    public Task RunAsync(CancellationToken ct)
    {
        throw new System.NotImplementedException();
    }

    public async Task<FirebaseUser> SignIn(Credential credential)
    {
        FirebaseUser user = await auth.SignInWithCredentialAsync(credential);

        return user;
    }
}
