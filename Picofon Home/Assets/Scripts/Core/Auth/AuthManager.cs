using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class AuthManager : MonoBehaviour
{
    private static bool _initialized;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private RectTransform _panel;

    public UserDataDTO CurrentUser { get; private set; }

    public UserService UserService { get; private set; }

    private const string GoogleClientId =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    private bool _existsConnection = false;

    private bool _firebaseDependenciesChecked = false;

    private bool _failedLogin = false;

    private bool _legalAccepted = false;

    public void Start()
    {
        BootAppProcess().Forget();
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        GamePrefs.ClearAll();

        CurrentUser = null;

        _uiManager.Show(PanelEnum.Login);
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

                _uiManager.Show(PanelEnum.Children);

                break;
            case DebugMenuResult.Map:

                UnityEngine.SceneManagement.SceneManager.LoadScene("MapPathScene");

                break;
            default:
                break;
        }
    }

    private async UniTask InitializeApp()
    {
        _initialized = true;

        _existsConnection = await ApiConfig.Ping();

        if (!_existsConnection)
        {
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
            return;
        }

        CancellationToken ct = this.GetCancellationTokenOnDestroy();

        _firebaseDependenciesChecked = await CheckFirebaseDependencies(ct);

        if (!_firebaseDependenciesChecked)
        {
            return;
        }

        await UniTask.WaitForSeconds(1f);

        FirebaseAuth firebaseInstance = FirebaseAuth.DefaultInstance;

        if (firebaseInstance.CurrentUser == null)
        {
            return;
        }

        string firebaseIdToken = await firebaseInstance
            .CurrentUser.TokenAsync(false)
            .AsUniTask()
            .AttachExternalCancellation(ct);

        ApiResult<LoginData> result = await UserService.LoginWithFirebaseToken(firebaseIdToken);

        if (!result.Success)
        {
            _failedLogin = true;
            return;
        }

        UserModel user = result.Data.User;

        CurrentUser = new UserDataDTO
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            Role = user.Role,
            ProfileComplete = user.ProfileCompleted,
        };

        _legalAccepted = user.LegalAccepted;
    }

    private async UniTaskVoid CheckAppState()
    {
        if (!_existsConnection)
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
            return;
        }

        if (Application.isEditor)
        {
            _uiManager.Show(PanelEnum.Login, animate: false);
            return;
        }

        if (!_firebaseDependenciesChecked)
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

        if (_failedLogin)
        {
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

        if (CurrentUser == null)
        {
            _uiManager.Show(PanelEnum.Login);
            return;
        }

        if (!_legalAccepted)
        {
            _uiManager.Show(PanelEnum.Disclaimer);
            return;
        }

        if (CurrentUser.Role == UserRole.Invited)
        {
            _uiManager.Show(PanelEnum.Role);
            return;
        }

        _uiManager.Show(PanelEnum.Children);
    }

    private async UniTaskVoid BootAppProcess()
    {
        _uiManager.SetLoadingState(true);

        if (!_initialized)
        {
            await InitializeApp();
        }

        _uiManager.SetLoadingState(false);

        _ = CheckAppState();
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
