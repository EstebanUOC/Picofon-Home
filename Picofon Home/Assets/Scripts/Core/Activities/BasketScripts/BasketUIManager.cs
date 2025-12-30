using BasketResponses;
using UnityEngine;

public class BasketUIManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private ItemManager _itemManager;

    [SerializeField]
    private ClueController _clueController;

    public void Start()
    {
        _clueController.OnClueChanged += HandleClueChanged;
    }

    public void SetViewContent(in ViewContentDTO content)
    {
        _clueController.Reset();

        _itemManager.UpdateViewContent(in content);
    }

    private void HandleClueChanged(bool showClue)
    {
        _itemManager.SetClueVisibility(showClue);
    }
}
