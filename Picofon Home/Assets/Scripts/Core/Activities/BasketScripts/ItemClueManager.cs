using UnityEngine;

public class ItemClueManager : MonoBehaviour
{
    private ItemManager _itemManager;

    private GameObject[] _items;

    public void Awake()
    {
        _itemManager = GetComponent<ItemManager>();
        _items = _itemManager.Items;
    }

    public void SetClueVisibility(bool showClue)
    {
        foreach (var item in _items)
        {
            ItemClue clue = item.GetComponent<ItemClue>();
            if (showClue)
                clue.ShowClue();
            else
                clue.HideClue();
        }
    }
}
