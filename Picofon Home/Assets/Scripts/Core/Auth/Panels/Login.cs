using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;

public class Login : Panel
{
    [SerializeField]
    public UIManager _uiManager;

    [SerializeField]
    public AuthManager _authManager;

    [Space]
    [SerializeField]
    private CustomButtonLoading _loginButton;

    [SerializeField]
    private CustomButton _debugButton;

    [SerializeField]
    private SimpleButton _optionsButton;

    private RectTransform _panel;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        _panel = GetComponent<RectTransform>();

        _optionsButton.OnClick += ShowOptions;

        _debugButton.OnClick += ShowDebugMenu;

        if (Application.isEditor)
        {
            _loginButton.Interactable = false;
            return;
        }

        _loginButton.OnClick += LoginWithGoogle;
    }

    private async UniTaskVoid AuthenticateWithGoogle()
    {
        FirebaseAuth firebaseInstance = FirebaseAuth.DefaultInstance;
        GoogleSignIn googleInstance = GoogleSignIn.DefaultInstance;

        googleInstance.SignOut();

        GoogleSignInUser googleUser;

        try
        {
            googleUser = await googleInstance.SignIn().AsUniTask();
        }
        catch (Exception e)
        {
            PerformanceLog.LogError("<DEBUG> Google sign-in failed, Error: " + e.Message);
            return;
        }

        string googleIdToken = googleUser.IdToken;
        Credential credential = GoogleAuthProvider.GetCredential(googleIdToken, null);

        FirebaseUser firebaseUser;

        try
        {
            firebaseUser = await firebaseInstance.SignInWithCredentialAsync(credential).AsUniTask();
        }
        catch (Exception e)
        {
            PerformanceLog.LogError("<DEBUG> Firebase authentication failed, Error: " + e.Message);
            return;
        }

        string firebaseIdToken;

        try
        {
            firebaseIdToken = await firebaseUser.TokenAsync(true).AsUniTask();
        }
        catch (Exception e)
        {
            PerformanceLog.LogError(
                "<DEBUG> Failed to retrieve Firebase ID token, Error: " + e.Message
            );
            return;
        }

        ApiResult<LoginData> result = await _authManager.UserService.LoginWithFirebaseToken(
            firebaseIdToken
        );

        PerformanceLog.Log($"Profile completed: {result.Data.User.ProfileCompleted}");

        if (result.Data.User.Role == UserRole.Therapist && !result.Data.User.ProfileCompleted)
        {
            ModalData modalData = new()
            {
                Title = "Profile Incomplete",
                Message =
                    "Your profile is incomplete. Please complete your profile in the web portal to access the app.",
                Panel = _panel,
            };
            await _uiManager.ShowModal(modalData);

            _authManager.Logout();
            return;
        }

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not log in. Please try again later.",
                Panel = _panel,
            };
            await _uiManager.ShowModal(modalData);
            return;
        }

        OnLoginSuccess(result.Data.User);
    }

    private void OnLoginSuccess(UserModel user)
    {
        _authManager.SetCurrentUser(user);
        _loginButton.EndLoading();

        if (!user.LegalAccepted)
        {
            _uiManager.ShowDisclaimer();
            return;
        }

        if (user.Role == UserRole.Invited)
        {
            _uiManager.ShowRolePanel();
            return;
        }

        _uiManager.ShowUserChildren();
    }

    private void LoginWithGoogle()
    {
        AuthenticateWithGoogle().Forget();

        _loginButton.EndLoading();
    }

    private void ShowOptions()
    {
        _uiManager.ShowOptions(_panel);
    }

    private void ShowDebugMenu()
    {
        _uiManager.ShowDebugMenu(_panel);
    }
}
