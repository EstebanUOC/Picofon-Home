using Cysharp.Threading.Tasks;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [Space(15)]
    public FeedbackView FeedbackView;

    [Space(15)]
    public UIFrameController FrameLeft;
    public UIFrameController FrameRight;

    private ReusableCompletionSource<bool> _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new ReusableCompletionSource<bool>();

        FeedbackView.OnContinueClicked += OnContinueButtonClicked;
        BasketManager.Instance.OnActivityChange += UpdateFrames;
    }

    public void Init()
    {
        gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public async UniTask<bool> ShowFeedback(FeedbackType feedbackType)
    {
        gameObject.SetActive(true);
        FeedbackView.ShowFeedback(feedbackType);

        _taskCompletion.Reset();

        return await _taskCompletion.Task;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
    }

    private void UpdateFrames(in BasketResponses.BasketActivity activity)
    {
        FrameLeft.UpdateFrame(activity.LeftImage, activity.LeftWord);
        FrameRight.UpdateFrame(activity.RightImage, activity.RightWord);
    }

    private void OnContinueButtonClicked()
    {
        _taskCompletion.TrySetResult(true);

        gameObject.SetActive(false);
    }
}
