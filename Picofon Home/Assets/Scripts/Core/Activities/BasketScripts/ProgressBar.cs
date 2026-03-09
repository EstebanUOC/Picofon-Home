using PrimeTween;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform fillRect;

    private float _maxHeight;
    private float _partHeight;

    private Vector2 _size = Vector2.zero;

    public void Awake()
    {
        _maxHeight = transform.GetComponent<RectTransform>().rect.height;
    }

    public void Initialize(int parts)
    {
        _partHeight = _maxHeight / parts;
    }

    public void SetProgress(int progress)
    {
        _size.y = _partHeight * progress;
        Tween.UISizeDelta(fillRect, endValue: _size, duration: 0.5f);
    }
}
