using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton
    : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    public event Action OnClick;

    [Space(15)]
    [SerializeField]
    private RectTransform _contentRect;

    [SerializeField]
    private RectTransform _backgroundRect;

    [SerializeField]
    private float _duration = 0.1f;

    private float _defaultContentY;
    private Vector2 _defaultBackgroundSizeDelta;

    public void Awake()
    {
        _defaultContentY = _contentRect.anchoredPosition.y;
        _defaultBackgroundSizeDelta = _backgroundRect.sizeDelta;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateHover(false);
    }

    private void AnimateHover(bool isHovered)
    {
        float moveY = _defaultContentY;
        Vector2 bgSizeDeltaY = _defaultBackgroundSizeDelta;
        Vector2 bgMoveY = Vector2.zero;

        if (isHovered)
        {
            moveY = _defaultContentY - 24f;
            bgSizeDeltaY = -31 * Vector2.up;
            bgMoveY = -11.5f * Vector2.up;
        }

        Tween posTween = Tween.UIAnchoredPositionY(_contentRect, moveY, _duration);

        Tween bgAnchorTween = Tween.UISizeDelta(_backgroundRect, bgSizeDeltaY, _duration);

        Tween bgAnchorPosTween = Tween.UIAnchoredPosition(_backgroundRect, bgMoveY, _duration);

        Sequence.Create().Group(posTween).Group(bgAnchorTween).Group(bgAnchorPosTween);
    }
}
