using Cysharp.Threading.Tasks;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Space]
    public Panel LoginPanel;
    public Panel DisclaimerPanel;
    public Panel RoleSelectionPanel;
    public Panel UserChildrenPanel;
    public Panel RegisterChildPanel;

    [Space]
    public Panel LoadingPanel;
    public Modal ModalPanel;

    [Space]
    public float VersionNumber = 0.2f;

    public void Awake()
    {
        SceneOrientationHelper.LockToPortrait();
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

    public void ShowRolePanel()
    {
        HideAllPanels();
        RoleSelectionPanel.Show();
    }

    public void ShowUserChildren()
    {
        HideAllPanels();
        UserChildrenPanel.Show();
    }

    public async UniTask ShowModal(ModalData data)
    {
        await ModalPanel.Show(data);
    }

    public void ShowOptions(RectTransform panel)
    {
        ModalPanel.ShowOptions(panel, VersionNumber);
    }

    public void ShowDebugMenu(RectTransform panel)
    {
        ModalPanel.ShowDebugMenu(panel);
    }

    private void HideAllPanels()
    {
        LoginPanel.Hide();
        RegisterChildPanel.Hide();
        UserChildrenPanel.Hide();
        DisclaimerPanel.Hide();
        RoleSelectionPanel.Hide();
    }
}
