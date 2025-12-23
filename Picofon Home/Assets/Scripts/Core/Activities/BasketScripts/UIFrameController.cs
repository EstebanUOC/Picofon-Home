using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFrameController : MonoBehaviour
{
    [Space(15)]
    public GameObject Image;
    public TMP_Text Text;

    private Image _imageComponent;

    public void Awake()
    {
        _imageComponent = Image.GetComponent<Image>();
    }

    public void UpdateFrame(Sprite sprite, string word)
    {
        _imageComponent.sprite = sprite;

        Text.text = word;
    }
}
