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

        _loginButton.EndLoading();

        OnLoginSuccess(result.Data.User);
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

    private void LoginWithGoogle()
    {
        // AuthenticateWithGoogle().Forget();

        Prueba().Forget();
    }

    private async UniTaskVoid Prueba()
    {
        string firebaseIdToken =
            "eyJhbGciOiJSUzI1NiIsImtpZCI6ImVlOTA0NmVhZDJlMDUwMDAxMGVkNTA0M2I0ODNkODRiMGM1MmM3YzQiLCJ0eXAiOiJKV1QifQ.eyJuYW1lIjoiRGFSa0FyIDUyMSIsInBpY3R1cmUiOiJodHRwczovL2xoMy5nb29nbGV1c2VyY29udGVudC5jb20vYS9BQ2c4b2NJMTdQSnVuZWRrSU5La0NuNGlsRkJZbUs4RVFSSFg3T3cwM3BnaFpldm1tMkRJMGpuLT1zOTYtYyIsImlzcyI6Imh0dHBzOi8vc2VjdXJldG9rZW4uZ29vZ2xlLmNvbS9waWNvZm9uLTI4MmQ4IiwiYXVkIjoicGljb2Zvbi0yODJkOCIsImF1dGhfdGltZSI6MTc4MTM5Mjk2MCwidXNlcl9pZCI6Ik9sdkVUbjJub05PWWduUmF6SU42VFRIbzFjNTIiLCJzdWIiOiJPbHZFVG4ybm9OT1lnblJheklONlRUSG8xYzUyIiwiaWF0IjoxNzgxMzkyOTYwLCJleHAiOjE3ODEzOTY1NjAsImVtYWlsIjoibHVpc2FkYW5pZWw3MTdAZ21haWwuY29tIiwiZW1haWxfdmVyaWZpZWQiOnRydWUsImZpcmViYXNlIjp7ImlkZW50aXRpZXMiOnsiZ29vZ2xlLmNvbSI6WyIxMDY2MTQ5NzAxNTkwMTQ1NjI0NzUiXSwiZW1haWwiOlsibHVpc2FkYW5pZWw3MTdAZ21haWwuY29tIl19LCJzaWduX2luX3Byb3ZpZGVyIjoiZ29vZ2xlLmNvbSJ9fQ.vfWnwfTxWkpgfukMJJnb1CXp4YfuHeQvFVCDxW69awPOCZKzMo6jnwsvxujm6Lt4K49srzsJJEoQLUHmVFcSkoRdPGLoJHXV-aYC3y7GAkQJ__dGvyr2Vrq64W68BV4kUJ7ClbPgQaPqT8xlyTNVJS04s9alUHTBa1_XF6Fq7hWojfaG_R5Tkiw4rOPEUuVOs_ii7wD3ALKRlm-OWUgmbk5jig27ZmeLrvV2g8jjHt96hN00eBa_dGomBrVyMzaNQ_xobbV16AbFOAKVvziQIKXFt-w8KtYx15byHp0zJ1Z60Q6XAjlT9iad03R1S9lL5MolUrGpATUlwyBecfPuNQ";

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
            _uiManager.ShowPanel(PanelEnum.Role);

            _authManager.IsNewUser = true;

            _authManager.NewUserFirebaseToken = firebaseIdToken;

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
