using System.Threading;
using Cysharp.Threading.Tasks;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.Localization.Settings;

public struct InitializeData
{
    public bool Initialized { get; set; }

    public bool FirebaseReady { get; set; }

    public bool FailedLogin { get; set; }

    public UserDataDTO CurrentUser { get; set; }
}

public class ChildDataDTO
{
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string BirthDate { get; set; }
}

public class AuthManager : MonoBehaviour
{
    private const string GoogleClientId =
        "1068789468608-otkna5ad1hgh9qqn0vt67630k67ri69r.apps.googleusercontent.com";

    private static InitializeData _initializeData;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private RectTransform _panel;

    public UserService UserService => _userService;

    public UserDataDTO CurrentUser => _initializeData.CurrentUser;

    public ChildDataDTO CurrentChild { get; private set; }

    public string NewUserFirebaseToken { get; internal set; }

    public bool IsNewUser { get; internal set; }

    private UserService _userService;

    private bool _existsConnection = false;

    public void Start()
    {
        BootAppProcess().Forget();
    }

    public void Logout()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        GamePrefs.ClearAll();

        _initializeData = new InitializeData();

        _uiManager.ShowPanel(PanelEnum.Login);
    }

    public void SetCurrentUser(UserModel user)
    {
        _initializeData.CurrentUser = new()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            ProfileComplete = user.ProfileCompleted,
            Role = user.Role,
            LegalAccepted = user.LegalAccepted,
        };
    }

    public void SetCurrentChild(ChildDataDTO child)
    {
        CurrentChild = new()
        {
            Id = child.Id,
            FirstName = child.FirstName,
            LastName = child.LastName,
            BirthDate = child.BirthDate,
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
                    Role = UserRole.Parent,
                    ProfileComplete = true,
                };

                _initializeData.CurrentUser = user;

                _uiManager.ShowPanel(PanelEnum.Children);

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
        _initializeData = new InitializeData { Initialized = true };

        GoogleSignIn.Configuration = new GoogleSignInConfiguration
        {
            WebClientId = GoogleClientId,
            RequestIdToken = true,
            RequestEmail = true,
        };

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

        _initializeData.FirebaseReady = await CheckFirebaseDependencies(ct);

        if (!_initializeData.FirebaseReady)
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
            _initializeData.FailedLogin = true;
            return;
        }

        UserModel user = result.Data.User;

        _initializeData.CurrentUser = new UserDataDTO
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.FirstName,
            Role = user.Role,
            ProfileComplete = user.ProfileCompleted,
            LegalAccepted = user.LegalAccepted,
        };
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
            _uiManager.ShowPanel(PanelEnum.Login, animate: false);
            return;
        }

        if (!_initializeData.FirebaseReady)
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

        if (_initializeData.FailedLogin)
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

        if (_initializeData.CurrentUser == null)
        {
            ShowPanel(PanelEnum.Login);
            return;
        }

        if (!_initializeData.CurrentUser.LegalAccepted)
        {
            ShowPanel(PanelEnum.Disclaimer);
            return;
        }

        if (_initializeData.CurrentUser.Role == UserRole.Invited)
        {
            ShowPanel(PanelEnum.Role);
            return;
        }

        ShowPanel(PanelEnum.Children);
    }

    private void ShowPanel(PanelEnum panel)
    {
        _uiManager.ShowPanel(panel, animate: false);
    }

    private async UniTaskVoid BootAppProcess()
    {
        _uiManager.ShowLoading(LoadingEnum.Boot);

        _existsConnection = await ApiConfig.Ping();

        await UniTask.WaitForSeconds(1f);

        if (!_existsConnection)
        {
            _ = CheckAppState();

            Logout();

            return;
        }

        _userService = new UserService();

        if (!_initializeData.Initialized)
        {
            await InitializeApp();
        }

        _uiManager.HideLoading(LoadingEnum.Boot);

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
