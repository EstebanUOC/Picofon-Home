using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ItemSelectable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private AudioClip _clickSound;

    public event Action<ItemView> OnItemSelected;

    private ItemView _itemView;

    public void Awake()
    {
        _itemView = GetComponent<ItemView>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.StopUI();
        AudioManager.Instance.PlayUI(_clickSound);
        OnItemSelected?.Invoke(_itemView);
    }
}
