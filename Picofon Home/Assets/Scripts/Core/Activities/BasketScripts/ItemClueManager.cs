using UnityEngine;

public class ItemClueManager : MonoBehaviour
{
    private GameObject[] _items;

    private bool _isClueActive = false;

    public void Awake()
    {
        ItemManager _itemManager = GetComponent<ItemManager>();
        _items = _itemManager.Items;
    }

    public void ToggleClueVisibility()
    {
        SetClueVisibility(!_isClueActive);
    }

    public void SetClueVisibility(bool showClue)
    {
        _isClueActive = showClue;

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
