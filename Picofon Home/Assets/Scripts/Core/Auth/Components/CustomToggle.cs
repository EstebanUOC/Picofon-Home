using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool _isSelectedByDefault = false;

    [SerializeField]
    private Image _target;

    [SerializeField]
    private Sprite _selectedSprite;

    [SerializeField]
    private Image _selectedBackground;

    [SerializeField]
    private CustomToggleGroup _group;

    private Sprite _defaultSprite;

    private bool _isSelected = false;
    private bool _wasSelected = true;

    private Color _defaultBackgroundColor;

    public void Awake()
    {
        _defaultSprite = _target.sprite;

        _defaultBackgroundColor = _selectedBackground.color;
        _selectedBackground.color = Color.clear;

        if (_isSelectedByDefault)
        {
            Toggle();
            _group.SelectToggle(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
        _group.SelectToggle(this);
    }

    public void Toggle()
    {
        if (_wasSelected == _isSelected)
        {
            return;
        }

        _isSelected = !_isSelected;
        _wasSelected = _isSelected;

        Sprite sprite = _defaultSprite;

        _selectedBackground.color = Color.clear;

        if (_isSelected)
        {
            sprite = _selectedSprite;
            _selectedBackground.color = _defaultBackgroundColor;
        }

        _target.sprite = sprite;
    }

    public void ToggleOff()
    {
        _isSelected = false;
        _wasSelected = true;

        _target.sprite = _defaultSprite;
        _selectedBackground.color = Color.clear;
    }
}
