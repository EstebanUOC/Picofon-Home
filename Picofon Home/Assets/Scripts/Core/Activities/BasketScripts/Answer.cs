using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    public event Action OnClick;

    public bool Enabled
    {
        get => _button.interactable;
        set
        {
            _button.interactable = value;
            _text.color = value ? _button.colors.normalColor : _button.colors.disabledColor;
        }
    }

    [SerializeField]
    private TMP_Text _text;

    private Button _button;

    public void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        OnClick?.Invoke();
    }
}
