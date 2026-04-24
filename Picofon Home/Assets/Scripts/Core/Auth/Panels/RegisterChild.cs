using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class RegisterChild : Panel
{
    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private TMP_Text _emailText;

    [SerializeField]
    private TMP_Text _usernameText;

    [Space]
    [SerializeField]
    private Form _registerForm;

    [SerializeField]
    private SimpleButton _returnButton;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        _registerForm.OnSubmit += HandleSubmit;
        _returnButton.OnClick += HandleReturn;
    }

    public override void Show()
    {
        base.Show();

        UserDataDTO currentUser = _authManager.CurrentUser;

        _emailText.text = currentUser.Email;
        _usernameText.text = currentUser.Username;
        _registerForm.ParentId = currentUser.Id;
        _registerForm.Relationship = currentUser.Role;
    }

    private async UniTask HandleSubmit(ChildCreateDTO data)
    {
        UserService userService = _authManager.UserService;
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult result = await userService.RegisterChild(data, token);

        string message;

        if (result.Success)
        {
            message = "Les dades del nen s'han enviat correctament.";
        }
        else
        {
            message = result.Message;
        }

        ModalData modalData = new() { Title = "Registre de nen", Message = message };

        await _uiManager.ShowModal(modalData);

        if (result.Success)
        {
            _uiManager.ShowUserChildren();
        }
    }

    private void HandleReturn()
    {
        _uiManager.ShowUserChildren();
    }
}
