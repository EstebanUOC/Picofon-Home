using UnityEngine;

public enum DebugMenuResult : byte
{
    Map,
    Children,
    Role,
    Close,
}

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private CustomButtonBase _mapButton;

    [SerializeField]
    private CustomButtonBase _childrenButton;

    [SerializeField]
    private CustomButtonBase _closeButton;

    [SerializeField]
    private CustomButtonBase _roleButton;

    public GenericEventChannel EventChannel;

    public void Awake()
    {
        _mapButton.OnClick += () => HandleClose(DebugMenuResult.Map);

        _childrenButton.OnClick += () => HandleClose(DebugMenuResult.Children);

        _closeButton.OnClick += () => HandleClose(DebugMenuResult.Close);

        _roleButton.OnClick += () => HandleClose(DebugMenuResult.Role);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void HandleClose(DebugMenuResult result)
    {
        EventChannel.Raise();

        _authManager.HandleDebugMenu(result);
    }
}
