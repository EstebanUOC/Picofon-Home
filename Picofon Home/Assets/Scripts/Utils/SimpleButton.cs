using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private bool _interactable = true;

    public event Action OnClick;

    public bool Interactable
    {
        get => _interactable;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }
}
