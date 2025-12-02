using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonLoading : CustomButtonRaised
{
    [Space(15)]
    public RectTransform LoadingRect;
    public CanvasGroup InfoCanvasGroup;
    public CanvasGroup LoadingCanvasGroup;

    private Sequence fadeSequence;
    private Tween loadingTween;

    private bool _loading = false;
    private bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;

            if (_loading)
            {
                InfoCanvasGroup.alpha = 0;
                fadeSequence.Restart();

                loadingTween.Restart();
            }
            else
            {
                fadeSequence.PlayBackwards();
                loadingTween.Pause();
                InfoCanvasGroup.alpha = 1;
            }
        }
    }

    public void Start()
    {
        fadeSequence = DOTween.Sequence().SetAutoKill(false).Pause();

        fadeSequence
            .Append(ContentCanvasGroup.DOFade(0.6f, Duration))
            .Append(LoadingCanvasGroup.DOFade(1, Duration));

        loadingTween = LoadingRect
            .DORotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear)
            .SetAutoKill(false)
            .Pause();
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (Loading)
            return;

        Loading = true;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (Loading)
            return;

        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (Loading)
            return;

        base.OnPointerExit(eventData);
    }
}
