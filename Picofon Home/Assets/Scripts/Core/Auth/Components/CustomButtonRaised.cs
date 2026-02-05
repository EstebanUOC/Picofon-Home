using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButtonRaised : CustomButtonBase, IInteractableButton
{
    [Space(15)]
    public float Duration = 0.1f;

    [Space(15)]
    public RectTransform BackgroundRect;
    public GameObject Content;

    private RectTransform _contentRect;
    private Image _contentImage;
    protected CanvasGroup _contentCanvasGroup;

    [Space(15)]
    public Color HoverColor = Color.blue;

    public bool Interactable
    {
        get => _interactable;
        set
        {
            if (_interactable == value)
                return;

            _interactable = value;
            float fade = _interactable ? 1f : 0.6f;
            _contentCanvasGroup.DOFade(fade, Duration);

            if (_interactable)
            {
                _hoverSequence.PlayBackwards();
                return;
            }

            if (!_isHovered)
                _hoverSequence.Restart();
        }
    }

    private bool _interactable = true;
    private bool _isHovered = false;

    private Sequence _hoverSequence;

    public void Awake()
    {
        _contentRect = Content.GetComponent<RectTransform>();
        _contentCanvasGroup = Content.GetComponent<CanvasGroup>();
        _contentImage = Content.GetComponent<Image>();

        const float HoverMoveY = -11f;
        const float BackgroundMoveY = -5.5f;

        _hoverSequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        Tween colorTween = _contentImage
            .DOColor(HoverColor, Duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween posTween = _contentRect
            .DOAnchorPosY(_contentRect.anchoredPosition.y + HoverMoveY, Duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween bgAnchorTween = BackgroundRect
            .DOSizeDelta(HoverMoveY * Vector2.up, Duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween bgAnchorPosTween = BackgroundRect
            .DOAnchorPos(BackgroundMoveY * Vector2.up, Duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        _hoverSequence.Append(colorTween).Join(posTween).Join(bgAnchorTween).Join(bgAnchorPosTween);
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
        _hoverSequence.Restart();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _isHovered = false;
        _hoverSequence.PlayBackwards();
    }

    public void OnDestroy()
    {
        _hoverSequence.Kill();
    }
}
