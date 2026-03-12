using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegisterChild : Panel
{
    [SerializeField]
    private UIManager _uiManager;

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

        _emailText.text = _uiManager.CurrentUser.Email;
        _usernameText.text = _uiManager.CurrentUser.Username;
        _registerForm.ParentId = _uiManager.CurrentUser.Id;
        _registerForm.Relationship = _uiManager.CurrentUser.Role;
    }

    private async UniTask HandleSubmit(ChildCreateDTO data)
    {
        UserService userService = _uiManager.UserService;
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
