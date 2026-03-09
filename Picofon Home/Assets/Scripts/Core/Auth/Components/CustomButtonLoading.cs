using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class CustomButtonLoading : CustomButtonRaised
{
    [Space]
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

                AnimateFade(true);
                AnimateLoading(true);
            }
            else
            {
                Interactable = true;
                InfoCanvasGroup.alpha = 1;

                AnimateFade(false);
                AnimateLoading(false);
            }
        }
    }

    private bool _loading = false;

    private Tween _loadingTween;

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (Loading || !Interactable)
            return;

        HandleClickAsync().Forget();
    }

    private async UniTaskVoid HandleClickAsync()
    {
        Loading = true;
        await (OnClickAsync?.Invoke() ?? UniTask.CompletedTask);
        Loading = false;
    }

    private void AnimateFade(bool isHovering)
    {
        if (Loading)
            return;

        float contentAlpha = 1f;
        float loadingAlpha = 0f;

        if (isHovering)
        {
            contentAlpha = 0.6f;
            loadingAlpha = 1f;
        }

        Sequence
            .Create()
            .Group(Tween.Alpha(_contentCanvasGroup, contentAlpha, Duration))
            .Group(Tween.Alpha(LoadingCanvasGroup, loadingAlpha, Duration));
    }

    private void AnimateLoading(bool isLoading)
    {
        if (!isLoading)
        {
            _loadingTween.Complete();
            return;
        }

        Vector3 targetRotation = new(0, 0, -360);

        _loadingTween = Tween.Rotation(LoadingRect, targetRotation, 1f, cycles: -1);
    }
}
