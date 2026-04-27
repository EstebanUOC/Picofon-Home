using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class AuthManager : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private RectTransform _panel;

    public UserDataDTO CurrentUser { get; private set; }

    public UserService UserService { get; private set; }

    private const string GoogleClientId =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    public void Start()
    {
        _uiManager.LoadingPanel.Show();

        BootstrapApplicacion().Forget();
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        GamePrefs.ClearAll();

        CurrentUser = null;

        _uiManager.ShowLogin();
    }

    public void SetCurrentUser(UserModel user)
    {
        CurrentUser = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            ProfileComplete = user.ProfileCompleted,
            Role = user.Role,
        };
    }

    public void HandleDebugMenu(DebugMenuResult result)
    {
        GamePrefs.DebugMode = true;

        UserDataDTO user;

        switch (result)
        {
            case DebugMenuResult.Children:
                user = new()
                {
                    Id = "noXJSkWJnCW5iSEu32n5Kvofq5a2",
                    Email = "test@gmail.com",
                    Username = "Debug User",
                    Role = UserRole.Therapist,
                };

                CurrentUser = user;
                _uiManager.ShowUserChildren();
                break;
            case DebugMenuResult.Map:
                UnityEngine.SceneManagement.SceneManager.LoadScene("MapPathScene");
                break;
            case DebugMenuResult.Role:
                user = new()
                {
                    Id = "STrmT4YxH2PiAObWJh9l0USKVZ53",
                    Email = "test@gmail.com",
                    Username = "Debug User",
                    Role = UserRole.Invited,
                };

                CurrentUser = user;
                _uiManager.ShowRolePanel();
                break;
            default:
                break;
        }
    }

    private async UniTaskVoid BootstrapApplicacion()
    {
        bool existsConnection = await ApiConfig.Ping();

        if (!existsConnection)
        {
            _ = _uiManager.ShowModal(
                new ModalData
                {
                    Title = "Error",
                    Message =
                        "No internet connection detected. Please check your connection and try again, or you can use the app in debug mode.",
                    Panel = _panel,
                }
            );

            _uiManager.LoadingPanel.Hide();
            return;
        }

        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleClientId,
            RequestIdToken = true,
            RequestEmail = true,
        };

        UserService = new UserService();

        string preferredLanguage = GamePrefs.PreferredLanguage;

        if (preferredLanguage[0] != 'C')
        {
            int index = 1;

            await LocalizationSettings.InitializationOperation;

            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[
                index
            ];
        }

        if (Application.isEditor)
        {
            _uiManager.LoadingPanel.Hide();
            return;
        }

        CancellationToken ct = this.GetCancellationTokenOnDestroy();

        bool success = await CheckFirebaseDependencies(ct);

        if (!success)
        {
            await _uiManager.ShowModal(
                new ModalData
                {
                    Title = "Error",
                    Message =
                        "Failed to initialize Firebase services. Please check your internet connection and try again, or you can use the app in debug mode.",
                    Panel = _panel,
                }
            );

            Application.Quit();
        }

        await UniTask.WaitForSeconds(1f);

        FirebaseAuth firebaseInstance = FirebaseAuth.DefaultInstance;

        if (firebaseInstance.CurrentUser == null)
        {
            _uiManager.LoadingPanel.Hide();
            _uiManager.ShowLogin();
            return;
        }

        string firebaseIdToken = await firebaseInstance
            .CurrentUser.TokenAsync(false)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        ApiResult<LoginData> result = await UserService.LoginWithFirebaseToken(firebaseIdToken);
        UserModel user = result.Data.User;

        if (!result.Success)
        {
            _uiManager.LoadingPanel.Hide();

            await _uiManager.ShowModal(
                new ModalData
                {
                    Title = "Error",
                    Message =
                        "Failed to log in with existing session. Please try logging in again, or you can use the app in debug mode.",
                    Panel = _panel,
                }
            );

            Logout();
            return;
        }

        CurrentUser = new UserDataDTO
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            ProfileComplete = user.ProfileCompleted,
        };

        _uiManager.LoadingPanel.Hide();

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

    private async UniTask<bool> CheckFirebaseDependencies(CancellationToken ct)
    {
        FirebaseService firebaseService = new();

        bool success = await firebaseService.RunAsync(ct);

        if (!success)
        {
            PerformanceLog.LogError("Firebase failed to initialize.");
            return false;
        }

        return true;
    }
}
