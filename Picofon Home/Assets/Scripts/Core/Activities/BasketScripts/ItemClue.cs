using System;
using PrimeTween;
using UnityEngine;

public sealed class ItemClue : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private float _moveOffsetY = 28;

    [SerializeField]
    private float _transitionDuration = 0.2f;

    [SerializeField]
    private int _targetSize = 128;

    private GameObject _text;

    private RectTransform _imageRect;

    private Action _onComplete;

    private bool _isClueShown = false;
    private float _originalSize;

    public void Awake()
    {
        ItemView item = GetComponent<ItemView>();

        GameObject icon = item.Icon;
        _imageRect = icon.GetComponent<RectTransform>();
        _originalSize = _imageRect.sizeDelta.x;

        _text = item.Text;
        _text.SetActive(false);

        _onComplete = () => _text.SetActive(true);
    }

    public void ShowClue()
    {
        SetClueShown(true);

        _isClueShown = true;
    }

    public void HideClue()
    {
        SetClueShown(false);

        _isClueShown = false;
        _text.SetActive(false);
    }

    private void SetClueShown(bool shown)
    {
        if (shown == _isClueShown)
            return;

        float moveY = shown ? _moveOffsetY : 0;
        Vector2 targetSize = Vector2.one * (shown ? _targetSize : _originalSize);

        Tween moveTween = Tween.LocalPositionY(_imageRect, moveY, _transitionDuration);

        Tween resizeTween = Tween.UISizeDelta(_imageRect, targetSize, _transitionDuration);

        Sequence seq = Sequence.Create().Group(moveTween).Group(resizeTween);

        if (shown)
            seq.OnComplete(_onComplete);
    }
}
