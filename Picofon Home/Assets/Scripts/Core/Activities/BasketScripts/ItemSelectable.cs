using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TweenHelper
{
    public static void AnimateHoverRotation(bool isHovering, Transform transform)
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float rotation = 0f;

        if (isHovering)
        {
            scaleUp = 1.05f;
            rotation = 4f;
        }

        Tween transformTween = Tween.Scale(transform, scaleUp, duration);

        Tween rotateTween = Tween.Rotation(transform, new Vector3(0, 0, rotation), duration);

        Sequence.Create().Group(transformTween).Group(rotateTween);
    }

    public static void AnimateHoverTranslate(bool isHovering, Transform transform)
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float translationY = 0f;

        if (isHovering)
        {
            translationY = transform.localPosition.y + 0.1f;
            scaleUp = 1.05f;
        }

        Tween transformTween = Tween.Scale(transform, scaleUp, duration);

        Tween translateTween = Tween.LocalPositionY(transform, translationY, duration);

        Sequence.Create().Group(transformTween).Group(translateTween);
    }

    public static void AnimateHoverTranslate(bool isHovering, RectTransform transform)
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float translationY = 0f;

        if (isHovering)
        {
            float traslation = 5f;
            translationY = transform.anchoredPosition.y + traslation;
            scaleUp = 1.05f;
        }

        Tween transformTween = Tween.Scale(transform, scaleUp, duration);

        Tween translateTween = Tween.UIAnchoredPositionY(transform, translationY, duration);

        Sequence.Create().Group(transformTween).Group(translateTween);
    }
}

public enum AnimationType : byte
{
    RotateAndScale,
    TranslateAndScale,
    None,
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

    public ItemSelectedEventChannel ItemSelectedEventChannel { get; set; }

    public int ItemIndex { get; set; }

    private Action<bool> _animateHover;

    public void Awake()
    {
        if (_animationType == AnimationType.None)
            return;

        if (_animationType == AnimationType.RotateAndScale)
        {
            _animateHover = isHovering =>
            {
                TweenHelper.AnimateHoverRotation(isHovering, _itemTransform);
            };
            return;
        }

        if (_itemTransform is RectTransform)
        {
            RectTransform rectTransform = _itemTransform as RectTransform;
            _animateHover = isHovering =>
            {
                TweenHelper.AnimateHoverTranslate(isHovering, rectTransform);
            };
            return;
        }

        _animateHover = isHovering =>
        {
            TweenHelper.AnimateHoverTranslate(isHovering, _itemTransform);
        };
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemSelectedEventChannel?.Raise(ItemIndex);

        _animateHover?.Invoke(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _animateHover?.Invoke(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _animateHover?.Invoke(false);
    }
}
