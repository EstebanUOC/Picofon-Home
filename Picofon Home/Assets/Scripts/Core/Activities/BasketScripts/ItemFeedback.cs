using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemFeedback : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;

    private string _syllabifiedWord;

    public string SyllabifiedWord => _syllabifiedWord;

    public void Awake()
    {
        ItemView _item = GetComponent<ItemView>();

        _text = _item.Text.GetComponent<TMP_Text>();
        _image = _item.Icon.GetComponent<Image>();
    }

    public void SetItemContent(Sprite sprite, string syllabifiedWord)
    {
        _syllabifiedWord = syllabifiedWord;
        _image.sprite = sprite;
    }

    public void ConfigureItem(StringBuilder builder, Color32 color)
    {
        _text.color = color;
        _text.SetText(builder);
    }
}
