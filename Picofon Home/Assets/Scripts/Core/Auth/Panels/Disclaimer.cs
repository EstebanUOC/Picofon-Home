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
    }

    private void OnAccept()
    {
#if !UNITY_EDITOR
        GamePrefs.HasAcceptedTerms = true;
#endif

        UIManager.ShowUserChildren();
    }

    private void OnDecline()
    {
        UIManager.ShowLogin();
    }
}
