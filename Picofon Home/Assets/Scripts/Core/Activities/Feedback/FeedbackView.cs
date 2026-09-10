namespace Picofon.Activities.Feedback
{
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
        [SerializeField]
        private Image _imageComponent;

        public GameObject FeedbackPositive;
        public GameObject FeedbackNeutral;

        public event Action OnContinueClicked;

        public void Start()
        {
            Button positiveButton = FeedbackPositive.GetComponentInChildren<Button>();
            Button neutralButton = FeedbackNeutral.GetComponentInChildren<Button>();

            positiveButton.onClick.AddListener(HandleContinueClicked);
            neutralButton.onClick.AddListener(HandleContinueClicked);

            if (_imageComponent != null)
            {
                _imageComponent.enabled = false;
            }
        }

        public void DisplayFeedbackType(FeedbackType feedbackType)
        {
            bool isPositive = feedbackType == FeedbackType.Positive;

            FeedbackPositive.SetActive(isPositive);
            FeedbackNeutral.SetActive(!isPositive);

            if (_imageComponent != null)
            {
                _imageComponent.enabled = isPositive;
            }
        }

        private void HandleContinueClicked()
        {
            OnContinueClicked?.Invoke();
        }
    }
}
