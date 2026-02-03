using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LanguageMenu : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private Image _icon;

    [SerializeField]
    private GameObject _selector;

    [Space(10)]
    [SerializeField]
    private LanguageData _data;

    private Tween _tween;
    private bool _isOpen = false;

    public void Start()
    {
        RectTransform rectTransform = _selector.GetComponent<RectTransform>();

        _tween = rectTransform.DOScaleY(1, 0.25f).SetAutoKill(false).Pause().SetEase(Ease.InCubic);

        LanguageSelector selector = _selector.GetComponent<LanguageSelector>();

        selector.OnLanguageSelected += HandleLanguageSelected;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _isOpen = !_isOpen;

        if (_isOpen)
        {
            _tween.PlayForward();
        }
        else
        {
            _tween.PlayBackwards();
        }
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
}
