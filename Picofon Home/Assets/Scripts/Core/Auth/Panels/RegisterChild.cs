using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterChild : Panel
{
    public UIManager UIManager;

    [Space(15)]
    public TMP_Text EmailText;
    public TMP_Text UsernameText;

    [Space(15)]
    public Form ChildRegistrationForm;
    public CustomButtonLoading ContinueButton;

    private CancellationTokenSource cts;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        ContinueButton.OnClickAsync += OnContinue;
        ContinueButton.Interactable = false;
    }

    public override void Show()
    {
        base.Show();

        EmailText.text = UIManager.CurrentUser.Email;
        UsernameText.text = UIManager.CurrentUser.Username;
        ChildRegistrationForm.ParentId = UIManager.CurrentUser.Id;
    }

    private async UniTask OnContinue()
    {
        ChildCreateDTO childData = ChildRegistrationForm.GatherChildData();

        UserService userService = UIManager.UserService;
        cts = new CancellationTokenSource();

        UserRegisterChildResponse response;

        response = await userService.RegisterChild(childData, cts);

        string message = string.Empty;

        if (response.Success)
        {
            message = "Les dades del nen s'han enviat correctament.";
        }
        else
        {
            foreach (var error in response.Message.Content)
            {
                Debug.LogError($"Error registering child: {error}");
                message += error + "\n";
            }
        }

        Action callback = response.Success
            ? () => SceneManager.LoadScene("MapPathScene")
            : () => { };

        ModalData modalData = new()
        {
            Title = "Registre de nen",
            Message = message,
            OnClose = callback,
        };

        UIManager.ShowModal(modalData);
    }
}
