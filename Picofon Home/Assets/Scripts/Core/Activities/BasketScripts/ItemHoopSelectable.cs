using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ItemHoopSelectable
    : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    [SerializeField]
    private Transform _itemTransform;

    public event Action OnItemSelected;

    private Sequence _hoverSequence;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke();
        _hoverSequence.PlayBackwards();
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
        const float duration = 0.2f;
        const float scaleUp = 1.05f;
        const float rotation = 4f;

        _hoverSequence = DOTween.Sequence().SetRecyclable(true).SetAutoKill(false).Pause();

        bool randomFlip = UnityEngine.Random.value > 0.5f;
        float rotationZ = randomFlip ? rotation : -rotation;

        Tween transformTween = _itemTransform.DOScale(scaleUp, duration);
        Tween rotateTween = _itemTransform.DORotate(new Vector3(0, 0, rotationZ), duration);

        _hoverSequence.Append(transformTween).Join(rotateTween);
    }
}
