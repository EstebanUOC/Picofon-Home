using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ItemSelectable : MonoBehaviour, IPointerClickHandler
{
    public event Action OnItemSelected;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke();
    }
}
