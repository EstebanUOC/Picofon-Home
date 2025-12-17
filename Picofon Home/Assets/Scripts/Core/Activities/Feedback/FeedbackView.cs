using UnityEngine;

public class FeedbackView : MonoBehaviour
{
    [Space(15)]
    public GameObject FeedbackPositive;
    public GameObject FeedbackNeutral;

    public void ShowFeedback(FeedbackType feedbackType)
    {
        bool isPositive = feedbackType == FeedbackType.Positive;

        FeedbackPositive.SetActive(isPositive);
        FeedbackNeutral.SetActive(!isPositive);
    }
}
