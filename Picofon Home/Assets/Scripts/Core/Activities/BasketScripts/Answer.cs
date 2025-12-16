using System;
using UnityEngine;
using UnityEngine.UI;

public class Answer : MonoBehaviour
{
    public event Action OnClick;

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
