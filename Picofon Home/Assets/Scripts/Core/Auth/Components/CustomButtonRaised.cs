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
    public RectTransform ContentRect;
    public CanvasGroup ContentCanvasGroup;
    public Image ContentImage;

    [Space(15)]
    public Color HoverColor = Color.blue;

    public bool Interactable
    {
        get => _interactable;
        set
        {
            _interactable = value;
            if (_interactable)
            {
                _hoverSequence.PlayBackwards();
                ContentCanvasGroup.DOFade(1f, Duration);
            }
            else
            {
                _hoverSequence.Restart();
                ContentCanvasGroup.DOFade(0.6f, Duration);
            }
        }
    }

    private bool _interactable = true;

    private Sequence _hoverSequence;

    public void Awake()
    {
        const float HoverMoveY = -11f;
        const float BackgroundMoveY = -5.5f;

        _hoverSequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        Tween colorTween = ContentImage
            .DOColor(HoverColor, Duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween posTween = ContentRect
            .DOAnchorPosY(ContentRect.anchoredPosition.y + HoverMoveY, Duration)
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

        Debug.Log("CustomButtonRaised Clicked");
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _hoverSequence.Restart();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        _hoverSequence.PlayBackwards();
    }

    public void OnDestroy()
    {
        _hoverSequence.Kill();
    }
}
