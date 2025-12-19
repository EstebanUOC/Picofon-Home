using Cysharp.Threading.Tasks;
using UnityEngine;

public class FeedbackController : MonoBehaviour
{
    [Space(15)]
    public FeedbackView FeedbackView;

    [Space(15)]
    public ImageFrameController FrameLeft;
    public ImageFrameController FrameRight;

    public UniTask WaitUntilClicked => _taskCompletion.Task;

    private UniTaskCompletionSource _taskCompletion;

    public void Awake()
    {
        _taskCompletion = new UniTaskCompletionSource();

        FeedbackView.OnContinueClicked += OnContinueButtonClicked;
    }

    public void OnDestroy()
    {
        _taskCompletion.TrySetCanceled();
    }

    public async UniTask ShowFeedback(
        FeedbackType feedbackType,
        Sprite leftSprite,
        Sprite rightSprite
    )
    {
        gameObject.SetActive(true);

        FrameLeft.UpdateFrame(leftSprite, "Prueba");
        FrameRight.UpdateFrame(rightSprite, "Prueba");

        FeedbackView.ShowFeedback(feedbackType);

        await WaitUntilClicked;
    }

    private void OnContinueButtonClicked()
    {
        _taskCompletion?.TrySetResult();

        gameObject.SetActive(false);
    }
}
