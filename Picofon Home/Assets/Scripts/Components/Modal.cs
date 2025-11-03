using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Modal : MonoBehaviour
{
    [Header("Properties")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI messageText;
    public Button confirmButton;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(string title, string message, UnityEngine.Events.UnityAction onConfirm)
    {
        titleText.text = title;
        messageText.text = message;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(onConfirm);
        confirmButton.onClick.AddListener(Hide);

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
