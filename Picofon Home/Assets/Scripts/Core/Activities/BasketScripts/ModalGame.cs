using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class ModalGame : MonoBehaviour
{
    [SerializeField]
    private SimpleButton _buttonSummary;

    [SerializeField]
    private SimpleButton _buttonWarning;

    [SerializeField]
    private SimpleButton _buttonFinal;

    [SerializeField]
    private GameObject _summary;

    [SerializeField]
    private GameObject _warning;

    [Space]
    [SerializeField]
    private GameObject _final;

    [SerializeField]
    private TMP_Text _finalText;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        _buttonSummary.OnClick += OnButtonClicked;
        _buttonWarning.OnClick += OnButtonClicked;
        _buttonFinal.OnClick += OnButtonClicked;

        _summary.SetActive(false);
        _warning.SetActive(false);
        _final.SetActive(false);
    }

    public async UniTask<bool> ShowFinal(string text)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _finalText.text = text;

        _final.SetActive(true);

        bool result = await _taskCompletion.Task;

        _final.SetActive(false);

        _taskCompletion.Reset();

        return result;
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
