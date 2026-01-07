using BasketResponses;
using UnityEngine;

public class ItemFeedbackManager : MonoBehaviour
{
    private ItemManager _itemManager;

    private GameObject[] _items;

    public void Awake()
    {
        _itemManager = GetComponent<ItemManager>();
        _items = _itemManager.Items;
    }

    public void SetItemsContent(in ViewContentDTO content)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            ItemFeedback feedback = _items[i].GetComponent<ItemFeedback>();
            feedback.SetItemContent(content.Icons[i], content.Texts[i]);
        }
    }

    public void ConfigureItemsByType(FeedbackType feedbackType)
    {
        for (int i = 0; i < _items.Length; i++)
        {
            ItemFeedback feedback = _items[i].GetComponent<ItemFeedback>();
            feedback.ConfigureItemByType(feedbackType);
        }
    }
}
