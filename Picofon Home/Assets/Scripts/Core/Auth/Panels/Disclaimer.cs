using UnityEngine;

public class Disclaimer : Panel
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
        OnHide += () => gameObject.SetActive(false);

        _acceptButton.OnClick += OnAccept;
        _declineButton.OnClick += OnDecline;
    }

    private void OnAccept()
    {
        if (_authManager.CurrentUser.Role == UserRole.Invited)
        {
            _uiManager.ShowRolePanel();
            return;
        }

        _uiManager.ShowUserChildren();
    }

    private void OnDecline()
    {
        _uiManager.ShowLogin();
    }
}
