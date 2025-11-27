using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseService
{
    private FirebaseAuth auth;

    public bool IsFirebaseReady { get; private set; }

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
                    IsFirebaseReady = true;
                    Debug.Log("Firebase ready for authentication.");
                }
                else
                {
                    Debug.LogError("Firebase dependencies not available: " + status);
                }
            });
    }

    public async Task<FirebaseUser> SignIn(Credential credential)
    {
        FirebaseUser user = await auth.SignInWithCredentialAsync(credential);

        return user;
    }
}
