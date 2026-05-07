using UnityEngine;

public enum DebugMenuResult : byte
{
    Map,
    Children,
}

public class DebugMenu : MonoBehaviour
{
    [SerializeField]
    private AuthManager _authManager;

    [Space]
    [SerializeField]
    private CustomButton _mapButton;

    [SerializeField]
    private CustomButton _childrenButton;

    public GenericEventChannel EventChannel;

    public void Awake()
    {
        _mapButton.OnClick += () => HandleClose(DebugMenuResult.Map);

        _childrenButton.OnClick += () => HandleClose(DebugMenuResult.Children);
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
