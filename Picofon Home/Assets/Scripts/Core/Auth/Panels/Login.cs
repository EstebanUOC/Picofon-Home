using System;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;

public class Login : MonoBehaviour
{
    [SerializeField]
    public UIManager _uiManager;

    [SerializeField]
    public AuthManager _authManager;

    [SerializeField]
    public RectTransform _contentPanel;

    [SerializeField]
    private SimpleButton _optionsButton;

    [Space]
    [SerializeField]
    private CustomButtonLoading _loginButton;

    [SerializeField]
    private CustomButton _loginMailButton;

    [SerializeField]
    private CustomButton _debugButton;

    private RectTransform _panel;

    public void Start()
    {
        _panel = GetComponent<RectTransform>();

        _optionsButton.OnClick += ShowOptions;

        _debugButton.OnClick += ShowDebugMenu;

        if (!Debug.isDebugBuild)
        {
            _contentPanel.sizeDelta = new Vector2(_contentPanel.sizeDelta.x, 480);
            _debugButton.gameObject.SetActive(false);
        }

        if (Application.isEditor)
        {
            _loginButton.Interactable = false;
            return;
        }

        _loginButton.OnClick += AuthenticateWithGoogle;
    }

    private void OnLoginSuccess(UserModel user)
    {
        _authManager.SetCurrentUser(user);
        _loginButton.EndLoading();

        if (!user.LegalAccepted)
        {
            _uiManager.ShowPanel(PanelEnum.Disclaimer);
            return;
        }

        if (user.Role == UserRole.Invited)
        {
            _uiManager.ShowPanel(PanelEnum.Role);
            return;
        }

        _uiManager.ShowPanel(PanelEnum.Children);
    }

    private async UniTaskVoid AuthenticateWithGoogleAsync()
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

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = "Could not log in. Please try again later.",
                Panel = _panel,
            };

            _loginButton.EndLoading();

            await _uiManager.ShowModal(modalData);

            return;
        }

        if (result.Data.IsNewUser)
        {
            _authManager.IsNewUser = true;

            _authManager.NewUserFirebaseToken = firebaseIdToken;

            _loginButton.EndLoading();

            _uiManager.ShowPanel(PanelEnum.Role);

            return;
        }

        _authManager.IsNewUser = false;

        if (result.Data.User.Role == UserRole.Therapist && !result.Data.User.ProfileCompleted)
        {
            ModalData modalData = new()
            {
                Title = "Profile Incomplete",
                Message =
                    "Your profile is incomplete. Please complete your profile in the web portal to access the app.",
                Panel = _panel,
            };

            _loginButton.EndLoading();

            await _uiManager.ShowModal(modalData);

            _authManager.Logout();
            return;
        }

        _loginButton.EndLoading();

        OnLoginSuccess(result.Data.User);
    }

    private void AuthenticateWithGoogle()
    {
        AuthenticateWithGoogleAsync().Forget();
    }

    private void ShowOptions()
    {
        _uiManager.ShowModal(_panel, ModalEnum.Options);
    }

    private void ShowDebugMenu()
    {
        _uiManager.ShowModal(_panel, ModalEnum.DebugMenu);
    }
}
