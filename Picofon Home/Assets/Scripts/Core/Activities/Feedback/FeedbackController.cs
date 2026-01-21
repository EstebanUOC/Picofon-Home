using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [Space(15)]
    public FeedbackView FeedbackView;

    [Space(15)]
    [SerializeField]
    public ItemFeedbackManager _itemManager;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        FeedbackView.OnContinueClicked += OnContinueButtonClicked;
    }

    public void Init(ActivitySkill skill)
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
        _itemManager.Init(skill);
    }

    public async UniTask<bool> ShowFeedback(FeedbackType feedbackType)
    {
        gameObject.SetActive(true);
        FeedbackView.DisplayFeedbackType(feedbackType);
        _itemManager.ConfigureItemsByType(feedbackType);

        _taskCompletion.Reset();

        return await _taskCompletion.Task;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
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
