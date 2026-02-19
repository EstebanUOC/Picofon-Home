using DG.Tweening;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private readonly Vector2 offset = new(0f, 240f);

    private RectTransform _rectTransform;

    public void Awake()
    {
        const float moveAmount = 30f;

        Transform content = transform.GetChild(0);
        content.DOLocalMoveY(moveAmount, 0.8f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);

        _rectTransform = transform.GetComponent<RectTransform>();
    }

    public void PositionMarker(Vector2 position)
    {
        _rectTransform.anchoredPosition = position + offset;
    }

    public void MoveMarker(Vector2 position)
    {
        _rectTransform.DOAnchorPos(position + offset, 1f).SetEase(Ease.InOutBack);
    }
}
