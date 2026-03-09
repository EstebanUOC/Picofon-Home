using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButtonBase : MonoBehaviour, IPointerClickHandler
{
    public Action OnClick;

    public virtual void OnPointerClick(PointerEventData eventData) { }

    public virtual void RemoveAllListeners()
    {
        OnClick = null;
    }
}

public interface IInteractableButton
{
    public bool Interactable { get; set; }
}
