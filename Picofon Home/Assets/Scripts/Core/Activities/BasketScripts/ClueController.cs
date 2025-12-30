using System;
using UnityEngine;
using UnityEngine.UI;

public class ClueController : MonoBehaviour
{
    public event Action<bool> OnClueChanged;

    private Button _buttonComponent;

    private bool _isClueActive = false;

    public void Awake()
    {
        _buttonComponent = GetComponent<Button>();
        _buttonComponent.onClick.AddListener(HandleButtonClick);
    }

    public void Reset(in BasketResponses.BasketActivity _)
    {
        _buttonComponent.interactable = true;
        _isClueActive = false;
    }

    private void HandleButtonClick()
    {
        _isClueActive = !_isClueActive;

        OnClueChanged?.Invoke(_isClueActive);
    }
}
