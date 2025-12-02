using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CustomButtonBase
    : MonoBehaviour,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler
{
    public Action OnClick;

    public virtual void OnPointerClick(PointerEventData eventData) { }

    public virtual void OnPointerEnter(PointerEventData eventData) { }

    public virtual void OnPointerExit(PointerEventData eventData) { }
}

public interface IInteractableButton
{
    public bool Interactable { get; set; }
}
