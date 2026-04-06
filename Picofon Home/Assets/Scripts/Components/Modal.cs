using Cysharp.Threading.Tasks;
using TMPro;
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
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _message;

    [SerializeField]
    private CustomButtonBase _button;

    [Space]
    [SerializeField]
    private GameObject _content;

    [SerializeField]
    private GameObject _menu;

    [SerializeField]
    private GameObject _options;

    private ReusableCompletionSource<bool> _taskCompletion;
    private ReusableCompletionSource<DebugMenuResult> _taskDebug;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();
        _taskDebug = new ReusableCompletionSource<DebugMenuResult>();

        _button.OnClick += OnConfirmButtonClicked;
    }

    public async UniTask<DebugMenuResult> ShowDebugMenu()
    {
        gameObject.SetActive(true);

        _content.SetActive(false);
        _menu.SetActive(true);

        DebugMenu debugMenu = _menu.GetComponent<DebugMenu>();
        debugMenu.OnClose -= OnMenuClose;
        debugMenu.OnClose += OnMenuClose;

        _taskDebug.Reset();

        return await _taskDebug.Task;
    }

    public async UniTask<bool> Show(ModalData data)
    {
        gameObject.SetActive(true);
        _title.text = data.Title;
        _message.text = data.Message;

        return await _taskCompletion.Task;
    }

    public async UniTask<bool> ShowOptions()
    {
        gameObject.SetActive(true);

        _options.SetActive(true);

        OptionsMenu optionsMenu = _options.GetComponent<OptionsMenu>();
        optionsMenu.OnClose += OnOptionsMenuClose;

        return await _taskCompletion.Task;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
    }

    private void OnConfirmButtonClicked()
    {
        _taskCompletion.TrySetResult(true);

        gameObject.SetActive(false);
    }

    private void OnMenuClose(DebugMenuResult result)
    {
        _taskDebug.TrySetResult(result);

        _content.SetActive(true);
        _menu.SetActive(false);
        gameObject.SetActive(false);
    }

    private void OnOptionsMenuClose()
    {
        _taskCompletion.TrySetResult(true);

        _options.SetActive(false);
        gameObject.SetActive(false);
    }
}
