using UnityEngine;

public class Disclaimer : Panel
{
    public UIManager UIManager;

    [Space(15)]
    public CustomButtonBase AcceptButton;
    public CustomButtonBase DeclineButton;

    public void Start()
    {
        OnHide += () => gameObject.SetActive(false);

        AcceptButton.OnClick += OnAccept;
        DeclineButton.OnClick += OnDecline;

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
