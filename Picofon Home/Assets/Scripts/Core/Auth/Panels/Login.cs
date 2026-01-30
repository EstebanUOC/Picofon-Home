using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Login : Panel
{
    public UIManager UIManager;

    [Space(15)]
    public CustomButtonLoading LoginButton;
    public CustomButtonBase DebugLoginButton;

    [Space(15)]
    public TMPro.TMP_Text VersionText;

    [SerializeField]
    private Modal _modal;

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

        VersionText.text = UIManager.VersionNumber.ToString("0.0");
    }

    private void OnDebugLogin()
    {
        ShowDebugMenu().Forget();
    }

    private async UniTask AuthenticateWithGoogle()
    {
        // Prune any previous sessions
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
            // TODO: Show error with modal (Ej. "Usuari no trobat")
            Debug.LogError("<DEBUG> User service login failed, Error: " + e.Message);
            return;
        }

        OnLoginSuccess(user);
    }

    private void OnLoginSuccess(UserModel user)
    {
        UserDataDTO userData = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
        };

        UIManager.CurrentUser = userData;
        UIManager.ShowDisclaimer();
    }

    private async UniTaskVoid ShowDebugMenu()
    {
        DebugMenuResult result = await _modal.ShowDebugMenu();

        GamePrefs.DebugMode = true;
        switch (result)
        {
            case DebugMenuResult.Children:
                UserDataDTO debugUser = new()
                {
                    Id = "AwgdI1xsu5RoU6zgLvTfAZeklbn2",
                    Email = "test@gmail.com",
                    Username = "Debug User",
                };

                UIManager.CurrentUser = debugUser;
                UIManager.ShowUserChildren();
                break;
            case DebugMenuResult.Map:
                SceneManager.LoadScene("MapPathScene");
                break;
            default:
                break;
        }
    }
}
