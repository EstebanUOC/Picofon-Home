using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordItemController : MonoBehaviour
{
    [Space(15)]
    public GameObject Image;
    public TMP_Text Text;

    private Image _imageComponent;
    private string _syllabifiedWord;
    private readonly StringBuilder _stringBuilder = new();

    private readonly Color32 _positiveColor = new(255, 255, 255, 255);
    private readonly Color32 _negativeColor = new(22, 20, 65, 255);

    public void Awake()
    {
        _imageComponent = Image.GetComponent<Image>();
    }

    public void UpdateItem(Sprite sprite, string word, string syllabifiedWord)
    {
        _syllabifiedWord = syllabifiedWord;
        _imageComponent.sprite = sprite;

        Text.text = word;
    }

    public void ConfigureItemByType(FeedbackType feedbackType)
    {
        _stringBuilder.Clear();

        int sep = _syllabifiedWord.IndexOf('#');

        _stringBuilder.Append("<color=");

        if (feedbackType == FeedbackType.Positive)
        {
            Text.color = _positiveColor;
            _stringBuilder.Append("green>");
        }
        else
        {
            Text.color = _negativeColor;
            _stringBuilder.Append("orange>");
        }

        _stringBuilder.Append(_syllabifiedWord, 0, sep);

        _stringBuilder.Append("</color>");

        _stringBuilder.Append(_syllabifiedWord, sep + 1, _syllabifiedWord.Length - sep - 1);

        Text.SetText(_stringBuilder);
    }
}
