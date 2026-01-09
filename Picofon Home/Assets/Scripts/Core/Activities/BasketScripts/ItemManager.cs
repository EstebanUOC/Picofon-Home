using BasketResponses;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private AudioClip _clip;

    [SerializeField]
    private GameObject[] _items;

    public GameObject[] Items => _items;

    public void Start()
    {
        foreach (GameObject item in _items)
        {
            ItemSelectable selectable = item.GetComponent<ItemSelectable>();

            if (selectable is null)
                break;

            selectable.OnItemSelected += PlayItemSound;
        }
    }

    public void SetItemsContent(in ViewContentDTO content)
    {
        if (content.Icons.Length != _items.Length)
            return;

        for (int i = 0; i < _items.Length; i++)
        {
            ItemView view = _items[i].GetComponent<ItemView>();
            view.SetContent(content.Icons[i], content.Texts[i]);
        }
    }

    public void PlayItemSound()
    {
        // TODO: Re-enable audio
        // AudioManager.Instance.StopUI();
        // AudioManager.Instance.PlayUI(_clip);
    }
}
