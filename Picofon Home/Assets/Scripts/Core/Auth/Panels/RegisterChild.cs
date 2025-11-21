using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterChild : Panel
{
    public UIManager UIManager;

    [Header("Parent Info")]
    public TMP_Text EmailText;
    public TMP_Text UsernameText;

    [Header("Form")]
    public Form ChildRegistrationForm;
    public Button ContinueButton;

    public void Start()
    {
        ContinueButton.onClick.AddListener(OnContinue);
    }

    public override void Show()
    {
        base.Show();
        UserDataDTO parentData = UIManager.User;

        EmailText.text = parentData.Email;
        UsernameText.text = parentData.Username;
        ChildRegistrationForm.ParentId = parentData.Id;
    }

    private async void OnContinue()
    {
        ChildCreateDTO childData = ChildRegistrationForm.GatherChildData();

        UserService userService = UIManager.UserService;
        CancellationTokenSource cts = UIManager.Cts;

        var response = await userService.RegisterChild(childData, cts);

        if (!response.Success)
        {
            Debug.LogError("Child registration failed: ");
            foreach (var error in response.Message.Content)
            {
                Debug.LogError(error);
            }
        }
    }
}
