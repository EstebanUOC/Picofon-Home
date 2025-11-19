using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject DisclaimerPanel;
    public GameObject UserChildrenPanel;
    public GameObject RegisterChildPanel;

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

    private void HideAllPanels()
    {
        LoginPanel.SetActive(false);
        RegisterChildPanel.SetActive(false);
        UserChildrenPanel.SetActive(false);
        DisclaimerPanel.SetActive(false);
    }

    public void ShowLogin()
    {
        HideAllPanels();
        LoginPanel.SetActive(true);
    }

    public void ShowRegister()
    {
        HideAllPanels();
        RegisterChildPanel.SetActive(true);
    }

    public void ShowDisclaimer()
    {
        HideAllPanels();
        DisclaimerPanel.SetActive(true);
    }

    public void ShowUserChildren()
    {
        HideAllPanels();
        UserChildrenPanel.SetActive(true);
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
        RegisterChildPanel.GetComponent<ChildRegister>().SetParentInfo(email, username);
    }
}
