using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

public class RegisterUser : MonoBehaviour
{
    #region Constants

    private const int HeightLogin = 1020;
    private const int HeightRegister = 1225;

    #endregion

    // Common

    [SerializeField]
    private RectTransform _contentPanel;

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

        await UniTask.WaitForSeconds(1.2f);

        _loginButton.EndLoading();
        _goRegisterButton.Interactable = true;
    }

    private void Login()
    {
        LoginAsync().Forget();
    }

    private async UniTaskVoid RegisterAsync()
    {
        _goLoginButton.Interactable = false;

        await UniTask.WaitForSeconds(1.2f);

        _registerButton.EndLoading();
        _goLoginButton.Interactable = true;
    }

    private void Register()
    {
        RegisterAsync().Forget();
    }
}
