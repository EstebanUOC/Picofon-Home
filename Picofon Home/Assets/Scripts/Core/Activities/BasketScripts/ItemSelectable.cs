using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TweenHelper
{
    public static Sequence CreateHoverSequence(
        Transform itemTransform,
        AnimationType animationType = AnimationType.RotateAndScale
    )
    {
        return animationType switch
        {
            AnimationType.RotateAndScale => CreateRotateAndScaleSequence(itemTransform),
            AnimationType.TranslateAndScale => CreateTranslateAndScaleSequence(itemTransform),
            _ => CreateRotateAndScaleSequence(itemTransform),
        };
    }

    private static Sequence CreateRotateAndScaleSequence(Transform itemTransform)
    {
        const float duration = 0.2f;
        const float scaleUp = 1.05f;
        const float rotation = 4f;

        Sequence sequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        bool randomFlip = UnityEngine.Random.value > 0.5f;
        float rotationZ = randomFlip ? rotation : -rotation;

        Tween transformTween = itemTransform.DOScale(scaleUp, duration);
        Tween rotateTween = itemTransform.DORotate(new Vector3(0, 0, rotationZ), duration);

        sequence.Append(transformTween).Join(rotateTween);

        return sequence;
    }

    private static Sequence CreateTranslateAndScaleSequence(Transform _itemTransform)
    {
        const float duration = 0.2f;
        const float scaleUp = 1.05f;
        float translationY = _itemTransform is RectTransform ? 5f : 0.1f;

        Sequence sequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        Tween transformTween = _itemTransform.DOScale(scaleUp, duration);
        Tween translateTween = _itemTransform.DOLocalMoveY(
            _itemTransform.localPosition.y + translationY,
            duration
        );

        sequence.Append(transformTween).Join(translateTween);

        return sequence;
    }
}

public enum AnimationType : byte
{
    RotateAndScale,
    TranslateAndScale,
}

public sealed class ItemSelectable
    : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    [SerializeField]
    private Transform _itemTransform;

    [SerializeField]
    private AnimationType _animationType;

    public event Action OnItemSelected;

    private Sequence _hoverSequence;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke();
        _hoverSequence.PlayBackwards();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _hoverSequence ??= TweenHelper.CreateHoverSequence(_itemTransform, _animationType);

        _hoverSequence.Restart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverSequence.PlayBackwards();
    }
}
