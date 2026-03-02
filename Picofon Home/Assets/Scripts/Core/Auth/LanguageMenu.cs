using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private GameObject _selector;

    [Space]
    [SerializeField]
    private LanguageData _data;

    private bool _isOpen = false;

    private RectTransform _rectTransform;

    public void Start()
    {
        _rectTransform = _selector.GetComponent<RectTransform>();

        LanguageSelector selector = _selector.GetComponent<LanguageSelector>();

        selector.OnLanguageSelected += HandleLanguageSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isOpen = !_isOpen;

        AnimateMenuOpened(_isOpen);
    }

    public void OnValidate()
    {
        _icon.sprite = _data.Flag;
    }

    private void HandleLanguageSelected(LanguageData data)
    {
        _data = data;
        _icon.sprite = _data.Flag;
    }

    private void AnimateMenuOpened(bool isOpen)
    {
        _isOpen = isOpen;

        const float duration = 0.25f;

        int scale = 0;

        if (_isOpen)
            scale = 1;

        Tween.ScaleY(_rectTransform, scale, duration, Ease.InCubic);
    }
}
