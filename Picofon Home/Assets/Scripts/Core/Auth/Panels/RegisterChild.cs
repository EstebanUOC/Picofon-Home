using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterChild : MonoBehaviour
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
        UserDataDTO parentData = UIManager.User;

        EmailText.text = parentData.Email;
        UsernameText.text = parentData.Username;
        ChildRegistrationForm.SetParentId(parentData.Id);

        ContinueButton.onClick.AddListener(OnContinue);
    }

    private void OnContinue()
    {
        ChildCreateDTO childData = ChildRegistrationForm.GatherChildData();
    }
}
