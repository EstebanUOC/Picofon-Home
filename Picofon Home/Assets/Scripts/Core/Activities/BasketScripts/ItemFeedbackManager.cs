using System.Text;
using BasketResponses;
using UnityEngine;

public enum ActivitySkill : byte
{
    Initial = 1,
    Medial = 2,
    Final = 3,
}

public class ItemFeedbackManager : MonoBehaviour
{
    private readonly StringBuilder _builder = new();
    private readonly Color32 _positiveColor = new(255, 255, 255, 255);
    private readonly Color32 _negativeColor = new(22, 20, 65, 255);

    private ItemManager _itemManager;
    private GameObject[] _items;
    private ActivitySkill _skill;

    public void Awake()
    {
        _itemManager = GetComponent<ItemManager>();
        _items = _itemManager.Items;
    }

    public void Init(ActivitySkill skill)
    {
        _skill = skill;
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
        Color32 textColor = feedbackType == FeedbackType.Positive ? _positiveColor : _negativeColor;

        for (int i = 0; i < _items.Length; i++)
        {
            ItemFeedback item = _items[i].GetComponent<ItemFeedback>();
            string word = item.SyllabifiedWord;

            _builder.Clear();

            switch (_skill)
            {
                case ActivitySkill.Initial:
                {
                    int sep = word.IndexOf('#');
                    ColorWord(word, 0, sep, feedbackType);
                    _builder.Append(word, sep + 1, word.Length - sep - 1);
                    break;
                }
                case ActivitySkill.Medial:
                {
                    int firstSep = word.IndexOf('#');
                    int lastSep = word.LastIndexOf('#');

                    _builder.Append(word, 0, firstSep);
                    ColorWord(word, firstSep + 1, lastSep - firstSep - 1, feedbackType);
                    _builder.Append(word, lastSep + 1, word.Length - lastSep - 1);
                    break;
                }
                case ActivitySkill.Final:
                {
                    int sep = word.LastIndexOf('#');
                    _builder.Append(word, 0, sep);
                    ColorWord(word, sep + 1, word.Length - sep - 1, feedbackType);
                    break;
                }
            }

            item.ConfigureItem(_builder, textColor);
        }
    }

    private void ColorWord(string word, int startIndex, int length, FeedbackType feedbackType)
    {
        _builder.Append("<color=");

        string colorStr = feedbackType == FeedbackType.Positive ? "green>" : "orange>";

        _builder.Append(colorStr);

        _builder.Append(word, startIndex, length);

        _builder.Append("</color>");
    }
}
