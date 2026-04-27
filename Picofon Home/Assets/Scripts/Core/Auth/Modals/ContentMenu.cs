using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ContentMenu : MonoBehaviour
{
    [Space]
    [SerializeField]
    private TMP_Text _title;

    [SerializeField]
    private TMP_Text _message;

    [SerializeField]
    private CustomButtonBase _button;

    public GenericEventChannel EventChannel;

    public ReusableCompletionSource<bool> TaskCompletion;

    public void Awake()
    {
        _button.OnClick += HandleClick;
    }

    public async UniTask<bool> Show(ModalData data)
    {
        gameObject.SetActive(true);

        TaskCompletion.Reset();

        _title.text = data.Title;
        _message.text = data.Message;

        return await TaskCompletion.Task;
    }

    private void HandleClick()
    {
        TaskCompletion.TrySetResult(true);
        EventChannel.Raise();
    }
}
