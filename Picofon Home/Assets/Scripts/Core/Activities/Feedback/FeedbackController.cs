using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public enum FeedbackType
{
    Positive,
    Negative,
}

public class FeedbackController : MonoBehaviour
{
    [Space(15)]
    public Button ContinueButton;

    public FeedbackView FeedbackView;

    public UniTask WaitUntilClicked => _taskCompletion.Task;

    private UniTaskCompletionSource _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new UniTaskCompletionSource();

        ContinueButton.onClick.AddListener(OnContinueButtonClicked);
    }

    public void OnDestroy()
    {
        ContinueButton.onClick.RemoveListener(OnContinueButtonClicked);
        _taskCompletion.TrySetCanceled();
    }

    public async UniTask ShowFeedback(FeedbackType feedbackType)
    {
        gameObject.SetActive(true);
        FeedbackView.ShowFeedback(feedbackType);
        await WaitUntilClicked;
    }

    private void OnContinueButtonClicked()
    {
        gameObject.SetActive(false);
        _taskCompletion?.TrySetResult();
    }
}
