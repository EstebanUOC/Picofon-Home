using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject LoginCanvas;
    public GameObject DisclaimerCanvas;
    public GameObject ChildDataCanvas;

    [Header("Buttons")]
    public Button LoginButton;
    public Button DebugSignInButton;
    public Button AcceptDisclaimerButton;
    public Button DeclineDisclaimerButton;

    public void Start()
    {
        DebugSignInButton?.onClick.AddListener(ShowDisclaimer);
        LoginButton?.onClick.AddListener(ShowDisclaimer);

        AcceptDisclaimerButton?.onClick.AddListener(ShowRegister);
        DeclineDisclaimerButton?.onClick.AddListener(ShowLogin);
        ShowLogin();
    }

    private void ShowLogin()
    {
        LoginCanvas.SetActive(true);
        ChildDataCanvas.SetActive(false);
        DisclaimerCanvas.SetActive(false);
    }

    private void ShowRegister()
    {
        ChildDataCanvas.SetActive(true);
        LoginCanvas.SetActive(false);
        DisclaimerCanvas.SetActive(false);
    }

    private void ShowDisclaimer()
    {
        DisclaimerCanvas.SetActive(true);
        LoginCanvas.SetActive(false);
        ChildDataCanvas.SetActive(false);
    }

    public void SetLoginAction(UnityAction action)
    {
        LoginButton.onClick.AddListener(action);
    }

    public void SetDebugSignInAction(UnityAction action)
    {
        DebugSignInButton.onClick.AddListener(action);
    }

    public void SetParentInfo(string email, string username)
    {
        ChildDataCanvas.GetComponent<ChildRegister>().SetParentInfo(email, username);
    }
}
