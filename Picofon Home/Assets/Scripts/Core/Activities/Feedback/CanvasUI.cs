using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    [Space(10)]
    [SerializeField]
    private GameObject _feedback;

    [SerializeField]
    private GameObject _summary;

    private FeedbackController _feedbackController;

    public void Awake()
    {
        _feedbackController = _feedback.GetComponent<FeedbackController>();
        Debug.Log("CanvasUI Awake: FeedbackController found: " + (_feedbackController != null));
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

    public void ShowSummary()
    {
        _summary.SetActive(true);
    }
}
