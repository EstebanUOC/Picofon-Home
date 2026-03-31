using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    [Space]
    [SerializeField]
    private GameObject _feedback;

    private FeedbackController _feedbackController;

    public void Awake()
    {
        _feedbackController = _feedback.GetComponent<FeedbackController>();
    }

    public void Init(ActivitySkill skill)
    {
        _feedbackController.Init(skill);
    }

    public void SetFeedbackContent(in ViewContentDTO content)
    {
        _feedbackController.SetItemsContent(in content);
    }

    public async UniTask<bool> ShowFeedback(FeedbackType type)
    {
        return await _feedbackController.Show(type);
    }
}
