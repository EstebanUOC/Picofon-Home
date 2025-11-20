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
    }

    private void OnContinue()
    {
        // string parentEmail = EmailText.text;
        // string parentUsername = UsernameText.text;
        //
        // if (string.IsNullOrEmpty(parentEmail) || string.IsNullOrEmpty(parentUsername))
        // {
        //     Debug.LogWarning("⚠️ Parent email or username is empty.");
        //     return;
        // }
        //
        // // Here you would typically validate the email format and username
        // Debug.Log($"🚀 Registering child for parent: {parentUsername} ({parentEmail})");
        //
        // // Proceed to the next step in registration
        // UIManager.ShowNextPanel();
    }
}
