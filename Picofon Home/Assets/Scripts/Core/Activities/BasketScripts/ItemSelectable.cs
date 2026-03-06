using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public readonly struct TweenHelper
{
    public readonly Transform Transform { get; init; }
    public readonly RectTransform RectTransform { get; init; }

    public void AnimateHoverRotation()
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float rotation = 0f;
        //
        // if (isHovering)
        // {
        //     scaleUp = 1.05f;
        //     rotation = 4f;
        // }

        Tween transformTween = Tween.Scale(Transform, scaleUp, duration);

        Tween rotateTween = Tween.Rotation(Transform, new Vector3(0, 0, rotation), duration);

        Sequence.Create().Group(transformTween).Group(rotateTween);
    }

    public void AnimateHoverTranslate()
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float translationY = 0f;

        // if (isHovering)
        // {
        //     translationY = Transform.localPosition.y + 0.1f;
        //     scaleUp = 1.05f;
        // }

        Tween transformTween = Tween.Scale(Transform, scaleUp, duration);

        Tween translateTween = Tween.LocalPositionY(Transform, translationY, duration);

        Sequence.Create().Group(transformTween).Group(translateTween);
    }

    public void UIAnimateHoverTranslate()
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float translationY = 0f;

        // if (isHovering)
        // {
        //     float traslation = 5f;
        //     translationY = RectTransform.anchoredPosition.y + traslation;
        //     scaleUp = 1.05f;
        // }

        Tween transformTween = Tween.Scale(Transform, scaleUp, duration);

        Tween translateTween = Tween.UIAnchoredPositionY(RectTransform, translationY, duration);

        Sequence.Create().Group(transformTween).Group(translateTween);
    }
}

public enum AnimationType : byte
{
    RotateAndScale,
    TranslateAndScale,
    None,
}

public sealed class ItemSelectable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Transform _itemTransform;

    [SerializeField]
    private AnimationType _animationType;

    public ItemSelectedEventChannel ItemSelectedEventChannel { get; set; }

    public int ItemIndex { get; set; }

    private Action _animateHover;

    public void Awake()
    {
        if (_animationType == AnimationType.None)
            return;

        TweenHelper tweenHelper = new()
        {
            Transform = _itemTransform,
            RectTransform = _itemTransform as RectTransform,
        };

        if (_animationType == AnimationType.RotateAndScale)
        {
            _animateHover = tweenHelper.AnimateHoverRotation;
            return;
        }

        if (_itemTransform is RectTransform)
        {
            RectTransform rectTransform = _itemTransform as RectTransform;
            _animateHover = tweenHelper.UIAnimateHoverTranslate;
            return;
        }

        _animateHover = tweenHelper.AnimateHoverTranslate;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemSelectedEventChannel?.Raise(ItemIndex);

        _animateHover?.Invoke();
    }
}
