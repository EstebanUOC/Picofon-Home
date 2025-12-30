using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private ItemClue[] _items;

    [SerializeField]
    private ClueController _clueController;

    public void Awake()
    {
        _clueController.OnClueChanged += OnClueChanged;
    }

    private void OnClueChanged(bool showClue)
    {
        foreach (var item in _items)
        {
            if (showClue)
                item.ShowClue();
            else
                item.HideClue();
        }
    }
}
