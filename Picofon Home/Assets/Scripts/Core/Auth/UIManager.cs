using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject LoginPanel;
    public GameObject DisclaimerPanel;
    public GameObject UserChildrenPanel;
    public GameObject RegisterChildPanel;

    private UserDataDTO user;

    public UserDataDTO User
    {
        get { return user; }
        set { user = value; }
    }

    public void Start()
    {
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

    public void ShowRegisterChild()
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
}
