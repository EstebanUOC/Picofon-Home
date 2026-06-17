using Cysharp.Threading.Tasks;
using UnityEngine;

public class Disclaimer : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private CustomButton _acceptButton;

    [SerializeField]
    private CustomButton _declineButton;

    private RectTransform _panel;

    public void Start()
    {
        _acceptButton.OnClick += OnAccept;
        _declineButton.OnClick += OnDecline;

        _panel = GetComponent<RectTransform>();
    }

    private void OnAccept()
    {
        if (_authManager.IsNewUser)
        {
            RegisterNewUser().Forget();
            return;
        }

        _uiManager.ShowPanel(PanelEnum.Children);
    }

    private void OnDecline()
    {
        _authManager.Logout();
    }

    private async UniTaskVoid RegisterNewUser()
    {
        _uiManager.ShowLoading(LoadingEnum.Normal);

        UserService service = _authManager.UserService;

        ApiResult result = await service.Register(
            firebaseToken: _authManager.NewUserFirebaseToken,
            disclaimerAccepted: true,
            role: UserRole.Parent
        );

        _uiManager.HideLoading(LoadingEnum.Normal);

        if (!result.Success)
        {
            await _uiManager.ShowModal(
                new ModalData()
                {
                    Title = "Error",
                    Message = "There was an error creating your account. Please try again later.",
                    Panel = _panel,
                }
            );

            _authManager.Logout();

            return;
        }

        await _uiManager.ShowModal(
            new ModalData()
            {
                Title = "Account Created",
                Message =
                    "Your account has been created successfully. You can now log in to the app.",
                Panel = _panel,
            }
        );

        _uiManager.ShowPanel(PanelEnum.Children);
    }
}
