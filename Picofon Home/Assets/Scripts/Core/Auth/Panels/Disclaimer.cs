using UnityEngine;
using UnityEngine.UI;

public class Disclaimer : Panel
{
    public UIManager UIManager;

    [Space(15)]
    public Button AcceptButton;
    public Button DeclineButton;

    public void Start()
    {
        AcceptButton.onClick.AddListener(OnAccept);
        DeclineButton.onClick.AddListener(OnDecline);

        OnHide += () => gameObject.SetActive(false);

        if (Debug.isDebugBuild)
            return;

        if (GamePrefs.HasAcceptedTerms)
            UIManager.ShowUserChildren();
    }

    private void OnAccept()
    {
        if (!Debug.isDebugBuild)
            GamePrefs.HasAcceptedTerms = true;

        UIManager.ShowUserChildren();
    }

    private void OnDecline()
    {
        UIManager.ShowLogin();
    }
}
