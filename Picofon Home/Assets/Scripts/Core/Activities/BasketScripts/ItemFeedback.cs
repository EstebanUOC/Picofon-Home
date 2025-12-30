using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemFeedback : MonoBehaviour
{
    private Image _image;
    private TMP_Text _text;

    private readonly StringBuilder _stringBuilder = new();
    private readonly Color32 _positiveColor = new(255, 255, 255, 255);
    private readonly Color32 _negativeColor = new(22, 20, 65, 255);

    private string _syllabifiedWord;

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

    public void ConfigureItemByType(FeedbackType feedbackType)
    {
        _stringBuilder.Clear();

        int sep = _syllabifiedWord.IndexOf('#');

        _stringBuilder.Append("<color=");

        if (feedbackType == FeedbackType.Positive)
        {
            _text.color = _positiveColor;
            _stringBuilder.Append("green>");
        }
        else
        {
            _text.color = _negativeColor;
            _stringBuilder.Append("orange>");
        }

        _stringBuilder.Append(_syllabifiedWord, 0, sep);

        _stringBuilder.Append("</color>");

        _stringBuilder.Append(_syllabifiedWord, sep + 1, _syllabifiedWord.Length - sep - 1);

        _text.SetText(_stringBuilder);
    }
}
