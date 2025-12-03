using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonLoading : CustomButtonRaised
{
    [Space(15)]
    public RectTransform LoadingRect;
    public CanvasGroup InfoCanvasGroup;
    public CanvasGroup LoadingCanvasGroup;

    public Func<UniTask> OnClickAsync;

    private bool Loading
    {
        get => _loading;
        set
        {
            if (_loading == value)
                return;

            _loading = value;

            if (_loading)
            {
                Interactable = false;
                InfoCanvasGroup.alpha = 0;
                fadeSequence.Restart();
                loadingTween.Restart();
            }
            else
            {
                Interactable = true;
                InfoCanvasGroup.alpha = 1;
                fadeSequence.PlayBackwards();
                loadingTween.Pause();
            }
        }
    }

    private bool _loading = false;

    private Sequence fadeSequence;
    private Tween loadingTween;

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

        HandleClickAsync().Forget();
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

    private async UniTaskVoid HandleClickAsync()
    {
        Loading = true;
        await (OnClickAsync?.Invoke() ?? UniTask.CompletedTask);
        Loading = false;
    }
}
