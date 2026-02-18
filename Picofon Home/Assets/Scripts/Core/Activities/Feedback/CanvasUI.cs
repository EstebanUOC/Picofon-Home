using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasUI : MonoBehaviour
{
    [Space(10)]
    [SerializeField]
    private GameObject _feedback;

    [SerializeField]
    private GameObject _summary;

    [SerializeField]
    private GameObject _warning;

    private FeedbackController _feedbackController;

    public void Awake()
    {
        _feedbackController = _feedback.GetComponent<FeedbackController>();

        Summary summaryComponent = _summary.GetComponent<Summary>();
        summaryComponent.OnSummaryCompleted += HandleBackward;

        Summary summaryWarning = _warning.GetComponent<Summary>();
        summaryWarning.OnSummaryCompleted += HandleBackward;
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

    public void ShowWarning()
    {
        _warning.SetActive(true);
    }

    private void HandleBackward()
    {
        SceneManager.LoadScene("MapPathScene");
    }
}
