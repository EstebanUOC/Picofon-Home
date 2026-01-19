using System;
using DG.Tweening;
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

    private Sequence _hoverSequence;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_hoverSequence == null)
        {
            CreateHoverSequence();
        }

        _hoverSequence.Restart();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hoverSequence.PlayBackwards();
    }

    private void CreateHoverSequence()
    {
        const float moveY = -24;
        const float bgMoveY = -11.5f;
        const float bgSizeDeltaY = -31;

        _hoverSequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        Tween posTween = _contentRect
            .DOAnchorPosY(_contentRect.anchoredPosition.y + moveY, _duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween bgAnchorTween = _backgroundRect
            .DOSizeDelta(bgSizeDeltaY * Vector2.up, _duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        Tween bgAnchorPosTween = _backgroundRect
            .DOAnchorPos(bgMoveY * Vector2.up, _duration)
            .SetAutoKill(false)
            .SetRecyclable(true);

        _hoverSequence.Append(posTween).Join(bgAnchorTween).Join(bgAnchorPosTween);
    }
}
