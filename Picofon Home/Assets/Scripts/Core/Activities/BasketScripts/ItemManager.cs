using BasketResponses;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private GameObject[] _items;

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

    public void UpdateViewContent(in ViewContentDTO content)
    {
        if (content.Icons.Length != _items.Length)
            return;

        for (int i = 0; i < _items.Length; i++)
        {
            ItemView view = _items[i].GetComponent<ItemView>();
            view.SetContent(content.Icons[i], content.Texts[i]);
        }
    }
}
