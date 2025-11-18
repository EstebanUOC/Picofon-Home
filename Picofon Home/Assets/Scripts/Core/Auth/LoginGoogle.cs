using System;
using System.Collections;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using UnityEngine;

public class LoginWithGoogle : MonoBehaviour
{
    [Header("Firebase")]
    public string GoogleAPI =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    [Header("Form")]
    public Form ChildForm;

    [Header("UI Manager")]
    public UIManager UIManager;

    private FirebaseService firebaseService;

    private bool isSigningIn = false;

    public void Start()
    {
        firebaseService = new FirebaseService();
        firebaseService.InitFirebase();

        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = GoogleAPI,
            RequestEmail = true,
        };

        UIManager.SetLoginAction(Login);
        UIManager.SetDebugSignInAction(DebugLogin);
    }

    private void Login()
    {
        if (!firebaseService.IsFirebaseReady)
        {
            Debug.LogWarning("⚠️ Firebase not ready yet. Please wait...");
            return;
        }

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
        UIManager.SetParentInfo(user.Email, user.DisplayName);

        string parentId = user.UserId;
        ChildForm.SetParentId(parentId);
        ChildForm.SetContinueAction(OnContinue);
    }

    private void DebugLogin()
    {
        UIManager.SetParentInfo("test@gmail.com", "Test User");

        string parentId = "AwgdI1xsu5RoU6zgLvTfAZeklbn2";
        ChildForm.SetParentId(parentId);
        ChildForm.SetContinueAction(OnContinue);
    }

    private void OnContinue(ChildModel child)
    {
        bool valid = ChildModel.Validate(child);

        if (!valid)
        {
            Debug.LogError("Validation failed for ChildModel fields.");
            return;
        }

        Debug.Log("Child Model is valid.");
        Debug.Log("ChildModel JSON: " + child.ToJson());
        StartCoroutine(SendChildData(child));
    }

    IEnumerator SendChildData(ChildModel data)
    {
        static void onComplete(bool success)
        {
            string message = success
                ? "Les dades del nen s'han enviat correctament."
                : "Hi ha hagut un error en enviar les dades del nen. Si us plau, torna-ho a intentar més tard.";

            Debug.Log("Msg::::: " + message);

            // modal.Show(
            //     success ? "Èxit" : "Error",
            //     message,
            //     success ? () => SceneManager.LoadScene("MapPathScene") : () => { }
            // );
        }

        yield return new ChildService().SendChildData(data, onComplete);
    }
}
