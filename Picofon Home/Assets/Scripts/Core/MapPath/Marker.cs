using PrimeTween;
using UnityEngine;

public class Marker : MonoBehaviour
{
    private readonly Vector2 offset = new(0f, 240f);

    private RectTransform _rectTransform;

    public void Awake()
    {
        const float moveAmount = 30f;

        Transform content = transform.GetChild(0);

        Tween.LocalPositionY(
            content,
            moveAmount,
            0.8f,
            cycles: -1,
            cycleMode: CycleMode.Yoyo,
            ease: Ease.InOutSine
        );

        _rectTransform = transform.GetComponent<RectTransform>();
    }

    public void PositionMarker(Vector2 position)
    {
        _rectTransform.anchoredPosition = position + offset;
    }

    public void MoveMarker(Vector2 position, in Sequence sequence)
    {
        sequence.Chain(
            Tween.UIAnchoredPosition(_rectTransform, position + offset, 1f, Ease.InOutBack)
        );
    }
}
