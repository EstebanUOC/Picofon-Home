using Cysharp.Threading.Tasks;
using UnityEngine;

public class ModalGame : MonoBehaviour
{
    [SerializeField]
    private SimpleButton _buttonSummary;

    [SerializeField]
    private SimpleButton _buttonWarning;

    [SerializeField]
    private GameObject _summary;

    [SerializeField]
    private GameObject _warning;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        _buttonSummary.OnClick += OnButtonClicked;
        _buttonWarning.OnClick += OnButtonClicked;
    }

    public async UniTask<bool> ShowSummary()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _summary.SetActive(true);

        bool result = await _taskCompletion.Task;

        _summary.SetActive(false);

        _taskCompletion.Reset();

        gameObject.SetActive(false);
        return result;
    }

    public async UniTask<bool> ShowWarning()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _warning.SetActive(true);

        bool result = await _taskCompletion.Task;

        _warning.SetActive(false);

        _taskCompletion.Reset();

        gameObject.SetActive(false);
        return result;
    }

    private void OnButtonClicked()
    {
        _taskCompletion.TrySetResult(true);
    }
}
