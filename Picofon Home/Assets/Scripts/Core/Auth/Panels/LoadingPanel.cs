using DG.Tweening;
using UnityEngine;

public class LoadingPanel : MonoBehaviour
{
    public CanvasGroup CanvasContent;
    public RectTransform RectContent;

    private CanvasGroup _canvasGroup;

    public void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        CanvasContent.alpha = 0f;
        RectContent.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        Tween fadeIn = CanvasContent.DOFade(1, 0.5f);
        Tween scaleUp = RectContent.DOScale(1, 0.6f).SetEase(Ease.OutBack);
        Tween fadeOut = _canvasGroup.DOFade(0, 0.3f).SetDelay(0.1f);

        seq.onComplete += () => gameObject.SetActive(false);

        // Add time delay before starting fade out (Android build)
        seq.AppendInterval(1f);

        seq.Append(fadeIn).Join(scaleUp).Append(fadeOut).Play();
    }
}
