using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public Panel LoginPanel;
    public Panel DisclaimerPanel;
    public Panel UserChildrenPanel;
    public Panel RegisterChildPanel;

    [Space(15)]
    public Panel LoadingPanel;

    public UserDataDTO CurrentUser { get; set; }

    public UserService UserService = new();

    public void Start()
    {
        LoginPanel.Show();
        // ShowRegisterChild();
    }

    private void HideAllPanels()
    {
        LoginPanel.Hide();
        RegisterChildPanel.Hide();
        UserChildrenPanel.Hide();
        DisclaimerPanel.Hide();
    }

    public void ShowLogin()
    {
        HideAllPanels();
        LoginPanel.Show();
    }

    public void ShowRegisterChild()
    {
        HideAllPanels();
        RegisterChildPanel.Show();
    }

    public void ShowDisclaimer()
    {
        HideAllPanels();
        DisclaimerPanel.Show();
    }

    public void ShowUserChildren()
    {
        HideAllPanels();
        UserChildrenPanel.Show();
    }
}
