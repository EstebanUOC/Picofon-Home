using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ItemSelectable : MonoBehaviour, IPointerClickHandler
{
    public event Action<ItemView> OnItemSelected;

    private ItemView _itemView;

    public void Awake()
    {
        _itemView = GetComponent<ItemView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnItemSelected?.Invoke(_itemView);
    }
}
