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

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        ChildRegistrationForm.OnSubmit += HandleSubmit;
    }

    public override void Show()
    {
        base.Show();

        EmailText.text = UIManager.CurrentUser.Email;
        UsernameText.text = UIManager.CurrentUser.Username;
        ChildRegistrationForm.ParentId = UIManager.CurrentUser.Id;
    }

    private async UniTask HandleSubmit(ChildCreateDTO data)
    {
        UserService userService = UIManager.UserService;
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

        await UIManager.ShowModal(modalData);

        if (result.Success)
        {
            SceneManager.LoadScene("MapPathScene");
        }
    }
}
