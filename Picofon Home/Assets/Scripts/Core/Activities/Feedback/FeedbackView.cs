using System;
using UnityEngine;
using UnityEngine.UI;

public enum FeedbackType
{
    Positive,
    Neutral,
}

public class FeedbackView : MonoBehaviour
{
    [Space(15)]
    public GameObject FeedbackPositive;
    public GameObject FeedbackNeutral;

    [Space(15)]
    public WordItemController WordItemLeft;
    public WordItemController WordItemRight;

    public event Action OnContinueClicked;

    public void Start()
    {
        Button positiveButton = FeedbackPositive.GetComponentInChildren<Button>();
        Button neutralButton = FeedbackNeutral.GetComponentInChildren<Button>();

        positiveButton.onClick.AddListener(HandleContinueClicked);
        neutralButton.onClick.AddListener(HandleContinueClicked);
    }

    public void DisplayFeedbackType(FeedbackType feedbackType)
    {
        bool isPositive = feedbackType == FeedbackType.Positive;

        FeedbackPositive.SetActive(isPositive);
        FeedbackNeutral.SetActive(!isPositive);

        WordItemLeft.ConfigureItemByType(feedbackType);
        WordItemRight.ConfigureItemByType(feedbackType);
    }

    private void HandleContinueClicked()
    {
        OnContinueClicked?.Invoke();
    }
}
