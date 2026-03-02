using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonRaised : CustomButtonBase, IInteractableButton
{
    [Space]
    public float Duration = 0.1f;

    [Space]
    public RectTransform BackgroundRect;
    public GameObject Content;

    private RectTransform _contentRect;
    private Image _contentImage;
    protected CanvasGroup _contentCanvasGroup;

    [Space]
    public Color HoverColor = Color.blue;

    public bool Interactable
    {
        get => _interactable;
        set
        {
            if (_interactable == value)
                return;

            _interactable = value;

            float fade = 0.6f;

            if (_interactable)
                fade = 1f;

            Tween.Alpha(_contentCanvasGroup, fade, Duration);

            if (_interactable)
            {
                AnimateHover(false);
                return;
            }

            if (!_isHovered)
                AnimateHover(true);
        }
    }

    private bool _interactable = true;
    private bool _isHovered = false;

    private Color _defaultColor;
    private float _defaultContentY;

    public void Awake()
    {
        _contentRect = Content.GetComponent<RectTransform>();
        _contentCanvasGroup = Content.GetComponent<CanvasGroup>();
        _contentImage = Content.GetComponent<Image>();

        _defaultColor = _contentImage.color;
        _defaultContentY = _contentRect.anchoredPosition.y;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        OnClick?.Invoke();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _isHovered = true;
        AnimateHover(true);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _isHovered = false;
        AnimateHover(false);
    }

    private void AnimateHover(bool isHovering)
    {
        float moveY = _defaultContentY;
        Color targetColor = _defaultColor;

        Vector2 bgSize = Vector2.zero;
        Vector2 bgMoveY = Vector2.zero;

        if (isHovering)
        {
            moveY = _defaultContentY - 11f;
            targetColor = HoverColor;
            bgSize = -11f * Vector2.up;
            bgMoveY = -5.5f * Vector2.up;
        }

        Tween colorTween = Tween.Color(_contentImage, targetColor, Duration);

        Tween contentPositionTween = Tween.UIAnchoredPositionY(_contentRect, moveY, Duration);

        Tween backgroundSizeTween = Tween.UISizeDelta(BackgroundRect, bgSize, Duration);

        Tween backgroundPositionTween = Tween.UIAnchoredPosition(BackgroundRect, bgMoveY, Duration);

        Sequence
            .Create()
            .Group(colorTween)
            .Group(contentPositionTween)
            .Group(backgroundSizeTween)
            .Group(backgroundPositionTween);
    }
}
