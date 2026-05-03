using PrimeTween;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup _canvasContent;

    [SerializeField]
    private RectTransform _rectContent;

    private CanvasGroup _canvasGroup;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _canvasContent.alpha = 0f;
        _rectContent.localScale = Vector3.zero;

        Sequence seq = Sequence.Create();

        Tween fadeIn = Tween.Alpha(_canvasContent, 1, 0.5f);
        Tween scaleUp = Tween.Scale(_rectContent, 1f, 0.6f, Ease.OutBack);

#if UNITY_ANDROID
        seq.ChainDelay(1f);
#endif

        seq.Group(fadeIn).Group(scaleUp);
    }

    public void Hide()
    {
        Tween
            .Alpha(_canvasGroup, endValue: 0, duration: 0.5f, startDelay: 0.3f)
            .OnComplete(target: gameObject, go => go.SetActive(false));
    }
}
