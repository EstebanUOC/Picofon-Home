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
    public Image ContentImage;
    public RectTransform ContentRect;

    [Space(15)]
    public Color HoverColor = Color.blue;

    public bool Interactable
    {
        get => _interactable;
        set => _interactable = value;
    }

    private bool _interactable = true;

    private Vector2 _originalBackgroundOffsetMax;
    private Vector2 _originalContentAnchoredPos;
    private Color _originalContentColor;

    public void Awake()
    {
        _originalBackgroundOffsetMax = BackgroundRect.offsetMax;
        _originalContentAnchoredPos = ContentRect.anchoredPosition;
        _originalContentColor = ContentImage.color;
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        base.OnPointerClick(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        ContentImage.DOColor(HoverColor, Duration).Play();
        ContentRect.DOAnchorPosY(_originalContentAnchoredPos.y - 11, Duration).Play();
        DOTween
            .To(
                () => BackgroundRect.offsetMax,
                x => BackgroundRect.offsetMax = x,
                _originalBackgroundOffsetMax + Vector2.up * -11,
                Duration
            )
            .Play();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactable)
            return;

        ContentImage.DOColor(_originalContentColor, Duration).Play();
        ContentRect.DOAnchorPosY(_originalContentAnchoredPos.y, Duration).Play();
        DOTween
            .To(
                () => BackgroundRect.offsetMax,
                x => BackgroundRect.offsetMax = x,
                _originalBackgroundOffsetMax,
                Duration
            )
            .Play();
    }
}
