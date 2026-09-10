using Picofon.Activities.Basket;
using Picofon.Activities.Basket.DTOs.Responses;
using Picofon.Core.MapPath;
using Picofon.Utils;

namespace Picofon.Activities.Feedback
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class FeedbackController : MonoBehaviour
    {
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

        public async UniTask<bool> ShowSL(FeedbackType feedbackType, int correctIndex)
        {
            gameObject.SetActive(true);
            _feedbackView.DisplayFeedbackType(feedbackType);
            _itemManager.ConfigureItemsByTypeSL(feedbackType, correctIndex);

            _taskCompletion.Reset();

            return await _taskCompletion.Task;
        }

        public async UniTask<bool> ShowSegmentation(FeedbackType feedbackType)
        {
            gameObject.SetActive(true);
            _feedbackView.DisplayFeedbackType(feedbackType);
            _itemManager.ConfigureItemsByTypeSegmentation();

            _taskCompletion.Reset();

            return await _taskCompletion.Task;
        }

        public void SetItemsContent(in ViewContentDTO content, int length = 0)
        {
            _itemManager.SetItemsContent(in content, length);
        }

        private void OnContinueButtonClicked()
        {
            _taskCompletion.TrySetResult(true);

            gameObject.SetActive(false);
        }
    }
}
