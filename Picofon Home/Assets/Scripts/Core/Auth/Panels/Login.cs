using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;

public class Login : Panel
{
    public UIManager UIManager;

    [Header("Buttons")]
    public CustomButtonLoading LoginButton;
    public CustomButtonBase DebugLoginButton;

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

        LoginButton.OnClickAsync += AuthenticateWithGoogle;
        DebugLoginButton.OnClick += OnDebugLogin;

        OnHide += () => gameObject.SetActive(false);
    }

    private async UniTask AuthenticateWithGoogle()
    {
        // Prune any previous sessions
        GoogleSignIn.DefaultInstance.Disconnect();
        GoogleSignIn.DefaultInstance.SignOut();

        GoogleSignInUser googleUser;

        try
        {
            googleUser = await GoogleSignIn.DefaultInstance.SignIn().AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogError("<DEBUG> Google sign-in failed, Error: " + e.Message);
            return;
        }

        string googleIdToken = googleUser.IdToken;
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

        FirebaseUser firebaseUser;

        try
        {
            firebaseUser = await UIManager
                .FirebaseAuthInstance.SignInWithCredentialAsync(credential)
                .AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogError("<DEBUG> Firebase authentication failed, Error: " + e.Message);
            return;
        }

        string firebaseIdToken;
        try
        {
            firebaseIdToken = await firebaseUser.TokenAsync(true).AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogError("<DEBUG> Failed to retrieve Firebase ID token, Error: " + e.Message);
            return;
        }

        UserModel user;

        try
        {
            user = await UIManager.UserService.LoginWithFirebaseToken(firebaseIdToken);
        }
        catch (Exception e)
        {
            Debug.LogError("<DEBUG> User service login failed, Error: " + e.Message);
            return;
        }

        Debug.Log("<DEBUG> User logged in successfully: " + user.Email);
    }

    private void OnLoginSuccess(UserModel user)
    {
        UserDataDTO userData = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = "John Doe",
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
