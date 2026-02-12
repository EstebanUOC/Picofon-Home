using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [Space(10)]
    [SerializeField]
    private FeedbackView _feedbackView;

    [SerializeField]
    private ItemFeedbackManager _itemManager;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        _feedbackView.OnContinueClicked += OnContinueButtonClicked;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
    }

    public void Init(ActivitySkill skill)
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
        _itemManager.Init(skill);
    }

    public async UniTask<bool> Show(FeedbackType feedbackType)
    {
        gameObject.SetActive(true);
        _feedbackView.DisplayFeedbackType(feedbackType);
        _itemManager.ConfigureItemsByType(feedbackType);

        _taskCompletion.Reset();

        return await _taskCompletion.Task;
    }

    public void SetItemsContent(in ViewContentDTO content)
    {
        _itemManager.SetItemsContent(in content);
    }

    private void OnContinueButtonClicked()
    {
        _taskCompletion.TrySetResult(true);

        gameObject.SetActive(false);
    }
}
