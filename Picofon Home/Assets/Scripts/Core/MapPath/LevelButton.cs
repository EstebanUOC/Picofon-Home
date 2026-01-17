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
    private float _duration = 0.1f;

    private Sequence _hoverSequence;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }

    private void CreateHoverSequence()
    {
        const float moveY = -11f;
        const float bgMoveY = -5.5f;

        _hoverSequence = DOTween.Sequence().SetAutoKill(false).Pause();
    }
}
