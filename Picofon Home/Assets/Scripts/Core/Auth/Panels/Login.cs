using System;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;
using UnityEngine.UI;

public class Login : Panel
{
    public UIManager UIManager;

    [Header("Buttons")]
    public Button LoginButton;
    public Button DebugSignInButton;

    private FirebaseService firebaseService;
    private bool isSigningIn = false;

    private readonly string googleAPI =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    public void Start()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = googleAPI,
            RequestEmail = true,
        };

        firebaseService = new FirebaseService();

        LoginButton.onClick.AddListener(AuthenticateWithGoogle);
        DebugSignInButton.onClick.AddListener(OnDebugLogin);
    }

    private void AuthenticateWithGoogle()
    {
        // if (!firebaseService.IsFirebaseReady)
        // {
        //     Debug.LogWarning("⚠️ Firebase not ready yet. Please wait...");
        //     return;
        // }

        if (isSigningIn)
        {
            Debug.LogWarning("⚠️ Already signing in. Please wait.");
            return;
        }

        isSigningIn = true;
        Debug.Log("🚀 Starting Google Sign-In...");

        // Make sure there’s no existing session
        GoogleSignIn.DefaultInstance.Disconnect();
        GoogleSignIn.DefaultInstance.SignOut();

        GoogleSignIn
            .DefaultInstance.SignIn()
            .ContinueWithOnMainThread(async googleTask =>
            {
                isSigningIn = false;

                if (googleTask.IsFaulted)
                {
                    Debug.LogError($"<DEBUG> Google Sign-In failed: {googleTask.Exception}");
                    return;
                }

                if (googleTask.IsCanceled)
                {
                    Debug.LogWarning("<DEBUG> Google Sign-In canceled by user.");
                    return;
                }

                Debug.Log("✅ Google Sign-In success. Exchanging with Firebase...");

                // Get the Google Sign-In IdToken (OAuth token)
                string googleIdToken = googleTask.Result.IdToken;
                Debug.Log($"googleIdToken (OAuth token) : {googleIdToken}");

                // Use that token to sign in with Firebase
                Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);
                Debug.Log($"token: {credential}");

                try
                {
                    FirebaseUser user = await firebaseService.SignIn(credential);
                    Debug.Log($"✅ Firebase Auth success. Logged in as: {user.DisplayName}");

                    // ✅ Get Firebase's ID token (NOT the Google OAuth token)
                    string firebaseIdToken = await user.TokenAsync(true);
                    Debug.Log($"Important firebaseIdToken {firebaseIdToken}");

                    Debug.Log($"📤 Sending Firebase ID Token to backend for {user.Email}");

                    // Send the ID token to your backend
                    StartCoroutine(
                        new LoginAPI().SendFirebaseToken(
                            firebaseIdToken,
                            (success, user) =>
                            {
                                if (!success)
                                {
                                    Debug.LogError("❌ Backend login failed.");
                                    return;
                                }

                                Debug.Log("Backend login successful, user: " + user);
                            }
                        )
                    );

                    OnLoginSuccess(user);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"🔥 Firebase Auth exception: {ex.Message}");
                }
            });
    }

    private void OnLoginSuccess(FirebaseUser user)
    {
        UserDataDTO userData = new()
        {
            Id = user.UserId,
            Email = user.Email,
            Username = user.DisplayName,
        };

        UIManager.CurrentUser = userData;
        UIManager.ShowDisclaimer();
    }

    private void OnDebugLogin()
    {
        UserDataDTO debugUser = new()
        {
            Id = "AwgdI1xsu5RoU6zgLvTfAZeklbn2",
            Email = "test@gmail.com",
            Username = "Debug User",
        };

        UIManager.CurrentUser = debugUser;
        UIManager.ShowDisclaimer();
    }
}
