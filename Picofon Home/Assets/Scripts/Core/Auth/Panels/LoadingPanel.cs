using PrimeTween;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public CanvasGroup CanvasContent;
    public RectTransform RectContent;

    private CanvasGroup _canvasGroup;
    private Panel _panel;

    public void Awake()
    {
        _panel = GetComponent<Panel>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _panel.OnShow += OnShow;
        _panel.OnHide += OnHide;
    }

    private void OnShow()
    {
        CanvasContent.alpha = 0f;
        RectContent.localScale = Vector3.zero;

        Sequence seq = Sequence.Create();

        Tween fadeIn = Tween.Alpha(CanvasContent, 1, 0.5f);
        Tween scaleUp = Tween.Scale(RectContent, 1f, 0.6f, Ease.OutBack);

#if UNITY_ANDROID
        seq.ChainDelay(1f);
#endif

        seq.Group(fadeIn).Group(scaleUp);
    }

    private void OnHide()
    {
        Tween
            .Alpha(_canvasGroup, 0, 0.5f, startDelay: 0.3f)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
