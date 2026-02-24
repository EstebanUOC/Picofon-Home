using PrimeTween;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform fillRect;

    private float _maxWidth;
    private float _partWidth;

    private Vector2 _size = Vector2.zero;

    public void Awake()
    {
        _maxWidth = transform.GetComponent<RectTransform>().rect.width;
    }

    public void Initialize(int parts)
    {
        _partWidth = _maxWidth / parts;
    }

    public void SetProgress(int progress)
    {
        _size.x = _partWidth * progress;
        Tween.UISizeDelta(fillRect, endValue: _size, duration: 0.5f);
    }
}
