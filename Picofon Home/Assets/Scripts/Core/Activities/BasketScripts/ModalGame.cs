using Cysharp.Threading.Tasks;
using UnityEngine;

public class ModalGame : MonoBehaviour
{
    [SerializeField]
    private SimpleButton _button;

    [SerializeField]
    private GameObject _modal;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();
        _button.OnClick += OnButtonClicked;
    }

    public async UniTask<bool> ShowModal()
    {
        _modal.SetActive(true);
        bool result = await _taskCompletion.Task;

        _modal.SetActive(false);

        _taskCompletion.Reset();
        return result;
    }

    private void OnButtonClicked()
    {
        _taskCompletion.TrySetResult(true);
    }
}
