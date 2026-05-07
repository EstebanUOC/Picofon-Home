using PrimeTween;
using UnityEngine;

public class BootLoading : MonoBehaviour
{
    [SerializeField]
    private RectTransform _rectContent;

    private CanvasGroup _canvasGroup;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        _rectContent.localScale = Vector3.zero;

        Tween.Scale(_rectContent, 1f, 0.6f, Ease.OutBack);
    }

    public void Hide()
    {
        Tween
            .Alpha(_canvasGroup, endValue: 0, duration: 0.5f, startDelay: 0.3f)
            .OnComplete(target: gameObject, go => go.SetActive(false));
    }
}
