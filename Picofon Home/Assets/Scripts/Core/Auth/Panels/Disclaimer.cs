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

    public void Start()
    {
        _acceptButton.OnClick += OnAccept;
        _declineButton.OnClick += OnDecline;
    }

    private void OnAccept()
    {
        if (_authManager.CurrentUser.Role == UserRole.Invited)
        {
            _uiManager.ShowPanel(PanelEnum.Role);
            return;
        }

        _uiManager.ShowPanel(PanelEnum.Children);
    }

    private void OnDecline()
    {
        _authManager.Logout();
    }
}
