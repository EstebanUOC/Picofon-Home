using System;
using UnityEngine;
using UnityEngine.UI;

public class ClueController : MonoBehaviour
{
    public event Action<bool> OnClueChanged;

    private Button _buttonComponent;

    public void Awake()
    {
        _buttonComponent = GetComponent<Button>();
        _buttonComponent.onClick.AddListener(HandleButtonClick);
    }

    public void EnableClue(bool enable)
    {
        _buttonComponent.interactable = enable;
    }

    public void Reset()
    {
        _buttonComponent.interactable = true;
    }

    private void HandleButtonClick()
    {
        OnClueChanged?.Invoke(false);
    }
}
