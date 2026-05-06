using System.Threading;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class RolePanel : MonoBehaviour
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private RoleCard _therapistCard;

    [SerializeField]
    private RoleCard _parentCard;

    [SerializeField]
    private SimpleButton _backButton;

    private RectTransform _panel;

    public void Start()
    {
        GenericEventChannel<UserRole> eventChannel = new();

        eventChannel.OnRaised += OnRoleSelected;

        _therapistCard.EventChannel = eventChannel;
        _parentCard.EventChannel = eventChannel;

        _backButton.OnClick += _authManager.Logout;

        _panel = GetComponent<RectTransform>();
    }

    private void OnRoleSelected(UserRole roleType)
    {
        OnRoleSelectedAsync(roleType).Forget();
    }

    private async UniTaskVoid OnRoleSelectedAsync(UserRole roleType)
    {
        _uiManager.ShowLoading(LoadingEnum.Normal);

        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result = await _authManager.UserService.UpdateUserRole(
            _authManager.CurrentUser.Id,
            roleType,
            token
        );

        _uiManager.HideLoading(LoadingEnum.Normal);

        if (!result.Success)
        {
            await _uiManager.ShowModal(
                new ModalData()
                {
                    Title = "Error",
                    Message = "There was an error updating your role. Please try again.",
                    Panel = _panel,
                }
            );
            return;
        }

        await _uiManager.ShowModal(
            new ModalData()
            {
                Title = "Success",
                Message = "Your role has been updated successfully.",
                Panel = _panel,
            }
        );

        if (!_authManager.CurrentUser.ProfileComplete)
        {
            await _uiManager.ShowModal(
                new ModalData()
                {
                    Title = "Profile Incomplete",
                    Message =
                        "Your profile is incomplete. Please complete your profile in the web portal to access the app.",
                    Panel = _panel,
                }
            );

            _authManager.Logout();

            return;
        }

        _uiManager.ShowPanel(PanelEnum.Children);
    }
}
