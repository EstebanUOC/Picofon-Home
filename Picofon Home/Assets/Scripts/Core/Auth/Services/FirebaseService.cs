using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseService : MonoBehaviour
{
    private FirebaseAuth auth;
    private bool _isFirebaseReady = false;

    public bool IsFirebaseReady => _isFirebaseReady;

    public void InitFirebase()
    {
        Debug.Log("Firebase initialized.");
        FirebaseApp
            .CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                var status = task.Result;
                if (status == DependencyStatus.Available)
                {
                    auth = FirebaseAuth.DefaultInstance;
                    _isFirebaseReady = true;
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
