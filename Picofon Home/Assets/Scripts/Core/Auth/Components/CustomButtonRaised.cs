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
        }
    }

    private bool _interactable = true;

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
        AnimateClick();
    }

    private void AnimateClick()
    {
        float moveY;
        Color targetColor;

        Vector2 bgSize;
        Vector2 bgMoveY;

        moveY = _defaultContentY - 11f;
        targetColor = HoverColor;

        bgSize = -11f * Vector2.up;
        bgMoveY = -5.5f * Vector2.up;

        Sequence sequence = Sequence
            .Create()
            .Group(Tween.Color(_contentImage, targetColor, Duration))
            .Group(Tween.UIAnchoredPositionY(_contentRect, moveY, Duration))
            .Group(Tween.UISizeDelta(BackgroundRect, bgSize, Duration))
            .Group(Tween.UIAnchoredPosition(BackgroundRect, bgMoveY, Duration));

        moveY = _defaultContentY;
        targetColor = _defaultColor;

        bgSize = Vector2.zero;
        bgMoveY = Vector2.zero;

        sequence
            .Chain(Tween.Color(_contentImage, targetColor, Duration))
            .Group(Tween.UIAnchoredPositionY(_contentRect, moveY, Duration))
            .Group(Tween.UISizeDelta(BackgroundRect, bgSize, Duration))
            .Group(Tween.UIAnchoredPosition(BackgroundRect, bgMoveY, Duration));
    }
}
