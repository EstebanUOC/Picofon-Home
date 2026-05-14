using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool _isSelectedByDefault = false;

    [SerializeField]
    private int _index;

    [SerializeField]
    private Image _target;

    [SerializeField]
    private Sprite _selectedSprite;

    [Space]
    [SerializeField]
    private Image _selectedBackground;

    [SerializeField]
    private CustomToggleGroup _group;

    public event Action<bool> OnToggle;

    public bool IsSelected => _isSelected;
    public int Index => _index;

    private Sprite _defaultSprite;

    private bool _isSelected = false;
    private bool _wasSelected = true;

    private Color _defaultBackgroundColor;

    public void Awake()
    {
        if (_target is not null)
        {
            _defaultSprite = _target.sprite;
        }

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

        OnToggle?.Invoke(_isSelected);

        _selectedBackground.color = Color.clear;

        if (_isSelected)
        {
            _selectedBackground.color = _defaultBackgroundColor;
        }

        if (_target is null)
        {
            return;
        }

        Sprite sprite = _defaultSprite;

        if (_isSelected)
        {
            sprite = _selectedSprite;
        }

        _target.sprite = sprite;
    }

    public void ToggleOff()
    {
        OnToggle?.Invoke(false);

        _isSelected = false;
        _wasSelected = true;

        _selectedBackground.color = Color.clear;

        if (_target is null)
        {
            return;
        }

        _target.sprite = _defaultSprite;
    }
}
