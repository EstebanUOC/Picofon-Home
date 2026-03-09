using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public readonly struct TweenHelper
{
    public readonly Transform Transform { get; init; }
    public readonly RectTransform RectTransform { get; init; }

    const float duration = 0.2f;

    public void AnimateHoverRotation()
    {
        float scaleUp;
        float rotation;

        scaleUp = 1.05f;
        rotation = 4f;

        Sequence sequence = Sequence
            .Create()
            .Group(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.Rotation(Transform, new Vector3(0, 0, rotation), duration));

        scaleUp = 1f;
        rotation = 0f;

        sequence
            .Chain(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.Rotation(Transform, new Vector3(0, 0, rotation), duration));
    }

    public void AnimateHoverTranslate()
    {
        float scaleUp;
        float translationY;

        scaleUp = 1.05f;
        translationY = Transform.localPosition.y + 0.1f;

        Sequence sequence = Sequence
            .Create()
            .Group(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.LocalPositionY(Transform, translationY, duration));

        scaleUp = 1f;
        translationY = 0f;

        sequence
            .Chain(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.LocalPositionY(Transform, translationY, duration));
    }

    public void UIAnimateHoverTranslate()
    {
        float scaleUp;
        float translationY;

        scaleUp = 1.05f;
        translationY = RectTransform.anchoredPosition.y + 5f;

        Sequence sequence = Sequence
            .Create()
            .Group(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.UIAnchoredPositionY(RectTransform, translationY, duration));

        scaleUp = 1f;
        translationY = 0f;

        sequence
            .Chain(Tween.Scale(Transform, scaleUp, duration))
            .Group(Tween.UIAnchoredPositionY(RectTransform, translationY, duration));
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
