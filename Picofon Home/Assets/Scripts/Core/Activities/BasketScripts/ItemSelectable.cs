using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TweenHelper
{
    public static void AnimateHover(
        bool isHovering,
        Transform itemTransform,
        AnimationType animationType = AnimationType.RotateAndScale
    )
    {
        switch (animationType)
        {
            case AnimationType.RotateAndScale:
                AnimateHoverRotation(isHovering, itemTransform);
                break;
            case AnimationType.TranslateAndScale:
                AnimateHoverTranslate(isHovering, itemTransform);
                break;
        }
    }

    private static void AnimateHoverRotation(bool isHovering, Transform itemTransform)
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float rotation = 0f;

        if (isHovering)
        {
            scaleUp = 1.05f;
            rotation = 4f;
        }

        Tween transformTween = Tween.Scale(itemTransform, scaleUp, duration);

        Tween rotateTween = Tween.Rotation(itemTransform, new Vector3(0, 0, rotation), duration);

        Sequence.Create().Group(transformTween).Group(rotateTween);
    }

    private static void AnimateHoverTranslate(bool isHovering, Transform _itemTransform)
    {
        const float duration = 0.2f;

        float scaleUp = 1f;
        float translationY = 0f;

        if (isHovering)
        {
            float traslation = _itemTransform is RectTransform ? 5f : 0.1f;
            translationY = _itemTransform.localPosition.y + traslation;
            scaleUp = 1.05f;
        }

        Tween transformTween = Tween.Scale(_itemTransform, scaleUp, duration);

        Tween translateTween = Tween.LocalPositionY(_itemTransform, translationY, duration);

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

    public void Start()
    {
        Debug.Log(
            $"ItemSelectable Start: ItemIndex={_itemTransform.localPosition}, Name={_itemTransform.name}"
        );
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemSelectedEventChannel?.Raise(ItemIndex);

        if (_animationType == AnimationType.None)
            return;

        TweenHelper.AnimateHover(false, _itemTransform, _animationType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_animationType == AnimationType.None)
            return;

        TweenHelper.AnimateHover(true, _itemTransform, _animationType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_animationType == AnimationType.None)
            return;

        TweenHelper.AnimateHover(false, _itemTransform, _animationType);
    }
}
