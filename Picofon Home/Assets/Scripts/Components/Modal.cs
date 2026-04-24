using Cysharp.Threading.Tasks;
using UnityEngine;

public struct ModalData
{
    public string Title;
    public string Message;
}

public class Modal : MonoBehaviour
{
    [Space]
    [SerializeField]
    private GameObject _contentObject;

    [SerializeField]
    private GameObject _debugMenuObject;

    [SerializeField]
    private GameObject _optionsMenuObject;

    private GenericEventChannel _eventChannel;
    private ReusableCompletionSource<bool> _taskCompletion;

    private DebugMenu _debugMenu;

    private OptionsMenu _optionsMenu;

    private ContentMenu _contentMenu;

    public void Awake()
    {
        _eventChannel = new GenericEventChannel();

        _eventChannel.OnRaised += HandleClose;

        _taskCompletion = new ReusableCompletionSource<bool>();
    }

    public async UniTask<bool> Show(ModalData data)
    {
        gameObject.SetActive(true);

        _contentObject.SetActive(true);

        if (_contentMenu == null)
        {
            _contentMenu = _contentObject.GetComponent<ContentMenu>();

            _contentMenu.EventChannel = _eventChannel;
            _contentMenu.TaskCompletion = _taskCompletion;
        }

        return await _contentMenu.Show(data);
    }

    public async UniTask<bool> ShowOptions()
    {
        gameObject.SetActive(true);

        _optionsMenuObject.SetActive(true);

        if (_optionsMenu == null)
        {
            _optionsMenu = _optionsMenuObject.GetComponent<OptionsMenu>();

            _optionsMenu.EventChannel = _eventChannel;
            _optionsMenu.TaskCompletion = _taskCompletion;
        }

        return await _optionsMenu.Show();
    }

    public async UniTask<DebugMenuResult> ShowDebugMenu()
    {
        gameObject.SetActive(true);

        _debugMenuObject.SetActive(true);

        if (_debugMenu == null)
        {
            _debugMenu = _debugMenuObject.GetComponent<DebugMenu>();

            _debugMenu.EventChannel = _eventChannel;
        }

        return await _debugMenu.Show();
    }

    private void HandleClose()
    {
        gameObject.SetActive(false);
    }
}
