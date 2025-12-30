using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class ItemView : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private GameObject IconObject;

    [SerializeField]
    private GameObject TextObject;

    public GameObject Icon => IconObject;

    public GameObject Text => TextObject;

    public void SetContent(Sprite sprite, string word)
    {
        Image _imageComponent = Icon.GetComponent<Image>();
        _imageComponent.sprite = sprite;

        TMP_Text _textComponent = Text.GetComponent<TMP_Text>();
        _textComponent.text = word;
    }
}
