using DG.Tweening;
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

        Sequence seq = DOTween.Sequence();
        Tween fadeIn = CanvasContent.DOFade(1, 0.5f);
        Tween scaleUp = RectContent.DOScale(1, 0.6f).SetEase(Ease.OutBack);

#if UNITY_ANDROID
        seq.AppendInterval(1f);
#endif

        seq.Append(fadeIn).Join(scaleUp).Play();
    }

    private void OnHide()
    {
        Tween fadeOut = _canvasGroup.DOFade(0, 0.5f).SetDelay(0.3f);

        fadeOut.onComplete += () => gameObject.SetActive(false);
        fadeOut.Play();
    }
}
