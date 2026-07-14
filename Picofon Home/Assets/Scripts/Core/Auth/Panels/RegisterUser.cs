using System;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class RegisterUser : MonoBehaviour
{
    #region Constants

    private const int HeightLogin = 1020;
    private const int HeightRegister = 1225;

    #endregion

    // Common

    [SerializeField]
    private RectTransform _contentPanel;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [SerializeField]
    private RectTransform _panel;

    // Login

    [SerializeField]
    private GameObject _loginPanel;

    [SerializeField]
    private TMP_InputField _loginEmailInput;

    [SerializeField]
    private TMP_InputField _loginPasswordInput;

    [SerializeField]
    private CustomButtonLoading _loginButton;

    [SerializeField]
    private CustomButton _goRegisterButton;

    // Register

    [Space]
    [SerializeField]
    private GameObject _registerPanel;

    [SerializeField]
    private TMP_InputField _registerEmailInput;

    [SerializeField]
    private TMP_InputField _registerPasswordInput;

    [SerializeField]
    private TMP_InputField _registerConfirmPasswordInput;

    [SerializeField]
    private CustomButtonLoading _registerButton;

    [SerializeField]
    private CustomButton _goLoginButton;

    // Actions

    private Action _changePanelComplete;

    // Variables

    private GameObject _currentPanel;
    private GameObject _nextPanel;

    public void Start()
    {
        _loginButton.OnClick += Login;
        _goRegisterButton.OnClick += ShowRegister;

        _registerButton.OnClick += Register;
        _goLoginButton.OnClick += ShowLogin;

        _changePanelComplete = () =>
        {
            _currentPanel.SetActive(false);
            _nextPanel.SetActive(true);
        };
    }

    public void OnEnable()
    {
        _registerPanel.SetActive(false);
        _loginPanel.SetActive(true);

        _contentPanel.sizeDelta = new Vector2(_contentPanel.sizeDelta.x, HeightLogin);

        _loginEmailInput.text = string.Empty;

        _loginPasswordInput.text = string.Empty;

        _registerEmailInput.text = string.Empty;

        _registerPasswordInput.text = string.Empty;

        _registerConfirmPasswordInput.text = string.Empty;
    }

    private void ShowRegister()
    {
        _currentPanel = _loginPanel;

        _nextPanel = _registerPanel;

        Tween
            .UISizeDelta(
                _contentPanel,
                endValue: new Vector2(_contentPanel.sizeDelta.x, HeightRegister),
                duration: 0.2f,
                ease: Ease.InOutSine
            )
            .OnComplete(_changePanelComplete);
    }

    private void ShowLogin()
    {
        _currentPanel = _registerPanel;

        _nextPanel = _loginPanel;

        Tween
            .UISizeDelta(
                _contentPanel,
                endValue: new Vector2(_contentPanel.sizeDelta.x, HeightLogin),
                duration: 0.2f,
                ease: Ease.InOutSine
            )
            .OnComplete(_changePanelComplete);
    }

    private async UniTaskVoid LoginAsync()
    {
        _goRegisterButton.Interactable = false;

        if (
            string.IsNullOrEmpty(_loginEmailInput.text)
            || Regex.IsMatch(
                _loginEmailInput.text,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            ) == false
        )
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    "LOG-LOG-CORR-ERR"
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _loginButton.EndLoading();
            _goRegisterButton.Interactable = true;

            return;
        }

        if (string.IsNullOrEmpty(_loginPasswordInput.text))
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    "LOG-LOG-PASS-NULL-ERR"
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _loginButton.EndLoading();
            _goRegisterButton.Interactable = true;

            return;
        }

        ApiResult<LoginData> result = await _authManager.UserService.LoginWithCredentials(
            _loginEmailInput.text,
            _loginPasswordInput.text
        );

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    result.Message
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _loginButton.EndLoading();
            _goRegisterButton.Interactable = true;

            return;
        }

        UserModel user = result.Data.User;

        _authManager.SetCurrentUser(user);

        _loginButton.EndLoading();
        _goRegisterButton.Interactable = true;

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

    private void Login()
    {
        LoginAsync().Forget();
    }

    private async UniTaskVoid RegisterAsync()
    {
        _goLoginButton.Interactable = false;

        if (
            string.IsNullOrEmpty(_registerEmailInput.text)
            || Regex.IsMatch(
                _registerEmailInput.text,
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
            ) == false
        )
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    "LOG-LOG-CORR-ERR"
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _registerButton.EndLoading();
            _goLoginButton.Interactable = true;

            return;
        }

        if (
            string.IsNullOrEmpty(_registerPasswordInput.text)
            || string.IsNullOrEmpty(_registerConfirmPasswordInput.text)
            || _registerPasswordInput.text.Length < 6
            || _registerConfirmPasswordInput.text.Length < 6
        )
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    "LOG-REG-PASS-NULL-ERR"
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _registerButton.EndLoading();
            _goLoginButton.Interactable = true;

            return;
        }

        if (_registerPasswordInput.text != _registerConfirmPasswordInput.text)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    "LOG-REG-PASS-NOT-MATCH-ERR"
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _registerButton.EndLoading();
            _goLoginButton.Interactable = true;

            return;
        }

        ApiResult<RegisterResponse> result = await _authManager.UserService.RegisterCredentials(
            _registerEmailInput.text,
            _registerPasswordInput.text
        );

        if (!result.Success)
        {
            ModalData modalData = new()
            {
                Title = "Error",
                Message = LocalizationSettings.StringDatabase.GetLocalizedString(
                    "UI",
                    result.Message
                ),
                Panel = _panel,
            };

            await _uiManager.ShowModal(modalData);

            _registerButton.EndLoading();
            _goLoginButton.Interactable = true;

            return;
        }

        UserModel user = result.Data.User;

        _authManager.SetCurrentUser(user);

        _registerButton.EndLoading();
        _goLoginButton.Interactable = true;

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

    private void Register()
    {
        RegisterAsync().Forget();
    }
}
