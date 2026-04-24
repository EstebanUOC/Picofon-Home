using Cysharp.Threading.Tasks;
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
    private CustomButtonBase _mapButton;

    [SerializeField]
    private CustomButtonBase _childrenButton;

    [SerializeField]
    private CustomButtonBase _closeButton;

    [SerializeField]
    private CustomButtonBase _roleButton;

    public GenericEventChannel EventChannel;

    private ReusableCompletionSource<DebugMenuResult> _taskDebug;

    public void Awake()
    {
        _taskDebug = new ReusableCompletionSource<DebugMenuResult>();

        _mapButton.OnClick += () => HandleClose(DebugMenuResult.Map);

        _childrenButton.OnClick += () => HandleClose(DebugMenuResult.Children);

        _closeButton.OnClick += () => HandleClose(DebugMenuResult.Close);

        _roleButton.OnClick += () => HandleClose(DebugMenuResult.Role);
    }

    public async UniTask<DebugMenuResult> Show()
    {
        gameObject.SetActive(true);

        _taskDebug.Reset();

        return await _taskDebug.Task;
    }

    private void HandleClose(DebugMenuResult result)
    {
        _taskDebug.TrySetResult(result);
        EventChannel.Raise();
    }
}
