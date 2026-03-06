using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space]
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
        LevelItemView itemView = GetComponent<LevelItemView>();

        AnimateClick(itemView);
    }

    private void AnimateClick(LevelItemView itemView)
    {
        float moveY;
        Vector2 bgSizeDeltaY;
        Vector2 bgMoveY;

        moveY = _defaultContentY - 24f;
        bgSizeDeltaY = -31 * Vector2.up;
        bgMoveY = -11.5f * Vector2.up;

        Sequence sequence = Sequence.Create();

        sequence
            .Group(Tween.UIAnchoredPositionY(_contentRect, moveY, _duration))
            .Group(Tween.UISizeDelta(_backgroundRect, bgSizeDeltaY, _duration))
            .Group(Tween.UIAnchoredPosition(_backgroundRect, bgMoveY, _duration));

        moveY = _defaultContentY;
        bgSizeDeltaY = _defaultBackgroundSizeDelta;
        bgMoveY = Vector2.zero;

        sequence
            .Chain(Tween.UIAnchoredPositionY(_contentRect, moveY, _duration))
            .Group(Tween.UISizeDelta(_backgroundRect, bgSizeDeltaY, _duration))
            .Group(Tween.UIAnchoredPosition(_backgroundRect, bgMoveY, _duration));

        sequence.OnComplete(() =>
        {
            _eventChannel.Raise(itemView.Config, itemView.Index);
        });
    }
}
