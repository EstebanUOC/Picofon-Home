using BasketResponses;
using PrimeTween;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _items;

    public GameObject[] Items => _items;

    private ItemSelectedEventChannel _itemSelectedEventChannel;

    private AudioClip[] _audioClips;

    public void Awake()
    {
        _itemSelectedEventChannel = new ItemSelectedEventChannel();

        int index = 0;
        foreach (GameObject item in _items)
        {
            ItemSelectable selectable = item.GetComponent<ItemSelectable>();

            if (selectable is null)
                break;

            selectable.ItemSelectedEventChannel = _itemSelectedEventChannel;
            selectable.ItemIndex = index;
            index++;
        }

        _itemSelectedEventChannel.OnItemSelected += PlayItemSound;
    }

    public void Prueba()
    {
        const float duration = 0.5f;

        Sequence allItemsTween = Sequence.Create();

        Tween move = Tween.LocalPositionY(transform, transform.localPosition.y + 130, duration);

        allItemsTween.Group(move);

        foreach (GameObject item in _items)
        {
            RectTransform itemTransform = item.GetComponent<RectTransform>();

            Tween itemSize = Tween.UISizeDelta(itemTransform, 240 * Vector2.one, duration);

            allItemsTween.Group(itemSize);
        }
    }

    public void SetItemsAudio(AudioClip[] clips)
    {
        _audioClips = clips;
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

    public void PlayItemSound(int index)
    {
        AudioManager.Instance.StopUI();
        AudioManager.Instance.PlayUI(_audioClips[index], 1.5f);
    }
}
