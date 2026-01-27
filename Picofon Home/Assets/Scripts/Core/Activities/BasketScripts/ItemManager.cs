using BasketResponses;
using DG.Tweening;
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

    public void Prueba()
    {
        const float duration = 0.5f;

        Sequence allItemsTween = DOTween.Sequence();

        Tween move = transform.DOLocalMoveY(transform.localPosition.y + 130, duration);

        allItemsTween.Append(move);

        foreach (GameObject item in _items)
        {
            RectTransform itemTransform = item.GetComponent<RectTransform>();
            Tween itemSize = itemTransform.DOSizeDelta(240 * Vector2.one, duration);

            allItemsTween.Join(itemSize);
        }

        allItemsTween.Play();
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
