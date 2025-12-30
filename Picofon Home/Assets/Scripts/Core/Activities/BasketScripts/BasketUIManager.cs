using BasketResponses;
using UnityEngine;

public class BasketUIManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private ItemManager _itemManager;

    [SerializeField]
    private ItemClueManager _itemClueManager;

    [SerializeField]
    private ClueController _clueController;

    public void OnEnable()
    {
        _clueController.OnClueChanged += HandleClueChanged;
    }

    public void SetViewContent(in ViewContentDTO content)
    {
        _clueController.Reset();

        _itemManager.SetItemsContent(in content);
    }

    public void Reset()
    {
        _clueController.Reset();
        _itemClueManager.SetClueVisibility(false);
    }

    private void HandleClueChanged(bool showClue)
    {
        _itemClueManager.SetClueVisibility(showClue);
    }
}
