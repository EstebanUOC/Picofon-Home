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
    [SerializeField]
    private GameObject _loginButton;

    [SerializeField]
    private GameObject _debugButton;

    [Space(15)]
    [SerializeField]
    private TMPro.TMP_Text _versionText;

    private readonly string _googleAPI =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    public void Start()
    {
        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            RequestIdToken = true,
            WebClientId = _googleAPI,
            RequestEmail = true,
        };

        CustomButtonLoading loginButton = _loginButton.GetComponent<CustomButtonLoading>();
        CustomButtonBase debugLoginButton = _debugButton.GetComponent<CustomButtonBase>();

        loginButton.OnClickAsync += AuthenticateWithGoogle;
        debugLoginButton.OnClick += OnDebugLogin;

        OnHide += () => gameObject.SetActive(false);

        _versionText.text = UIManager.VersionNumber.ToString("0.00");
    }

    private void OnDebugLogin()
    {
        ShowDebugMenu().Forget();
    }

    private async UniTask AuthenticateWithGoogle()
    {
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

        ApiResult<LoginData> result = await UIManager.UserService.LoginWithFirebaseToken(
            firebaseIdToken
        );

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not log in. Please try again later.",
            };
            await UIManager.ShowModal(modalData);
            return;
        }

        OnLoginSuccess(result.Data.User);
    }

    private void OnLoginSuccess(UserModel user)
    {
        UserDataDTO userData = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            Role = user.Role,
        };

        Debug.Log("<DEBUG> User data: " + userData.Username + ", Role: " + userData.Role);

        UIManager.CurrentUser = userData;
        UIManager.ShowDisclaimer();
    }

    private async UniTaskVoid ShowDebugMenu()
    {
        DebugMenuResult result = await UIManager.ShowDebugMenu();

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
