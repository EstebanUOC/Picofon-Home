using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomToggle : MonoBehaviour, IPointerClickHandler
{
    # region References

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

    #endregion

    // Properties

    public bool IsSelected => _isSelected;
    public int Index => _index;

    // Events

    public event Action<bool> OnToggle;

    // Variables

    private Color _defaultBackgroundColor;

    private Sprite _defaultSprite;

    private bool _isSelected = false;

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

            _group?.SelectToggle(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();

        _group?.SelectToggle(this);
    }

    public void Toggle()
    {
        if (_group?.ShouldToggle(this) == false)
        {
            return;
        }

        _isSelected = !_isSelected;

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

        _selectedBackground.color = Color.clear;

        if (_target is null)
        {
            return;
        }

        _target.sprite = _defaultSprite;
    }
}
