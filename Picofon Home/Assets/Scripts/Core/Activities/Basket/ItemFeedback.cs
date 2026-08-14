namespace Picofon.Activities.Basket
{
    using System.Text;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class ItemFeedback : MonoBehaviour
    {
        private Image _image;
        private TMP_Text _text;

        private string _syllabifiedWord;
        private string _word;

        public string SyllabifiedWord => _syllabifiedWord;
        public string Word => _word;

        public void Awake()
        {
            ItemView _item = GetComponent<ItemView>();

            _text = _item.Text.GetComponent<TMP_Text>();
            _image = _item.Icon.GetComponent<Image>();
        }

        public void SetItemContent(Sprite sprite, string syllabifiedWord, string word)
        {
            _syllabifiedWord = syllabifiedWord;
            _word = word;
            _image.sprite = sprite;
        }

        public void ConfigureItem(StringBuilder builder, Color32 color)
        {
            _text.color = color;
            _text.SetText(builder);
        }

        public void ConfigureItem(string text, Color32 color)
        {
            _text.color = color;
            _text.SetText(text);
        }
    }
}
