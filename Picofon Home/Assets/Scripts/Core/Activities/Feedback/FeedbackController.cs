using Cysharp.Threading.Tasks;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [Space(15)]
    public FeedbackView FeedbackView;

    [Space(15)]
    public WordItemController WordItemLeft;
    public WordItemController WordItemRight;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        FeedbackView.OnContinueClicked += OnContinueButtonClicked;
        // BasketManager.Instance.OnActivityChange += UpdateFrames;
    }

    public void Init()
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public async UniTask<bool> ShowFeedback(FeedbackType feedbackType)
    {
        gameObject.SetActive(true);
        FeedbackView.DisplayFeedbackType(feedbackType);

        _taskCompletion.Reset();

        return await _taskCompletion.Task;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
    }

    private void UpdateFrames(in BasketResponses.BasketActivity activity)
    {
        WordItemLeft.UpdateItem(
            activity.LeftImage,
            activity.LeftWord,
            activity.LeftSyllabifiedWord
        );
        WordItemRight.UpdateItem(
            activity.RightImage,
            activity.RightWord,
            activity.RightSyllabifiedWord
        );
    }

    private void OnContinueButtonClicked()
    {
        _taskCompletion.TrySetResult(true);

        gameObject.SetActive(false);
    }
}
