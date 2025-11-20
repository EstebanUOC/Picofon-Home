using UnityEngine;
using UnityEngine.UI;

public class Disclaimer : Panel
{
    public UIManager UIManager;

    [Header("Buttons")]
    public Button AcceptButton;
    public Button DeclineButton;

    public void Start()
    {
        AcceptButton.onClick.AddListener(OnAccept);
        DeclineButton.onClick.AddListener(OnDecline);
    }

    private void OnAccept()
    {
        UIManager.ShowUserChildren();
    }

    private void OnDecline()
    {
        UIManager.ShowLogin();
    }
}
