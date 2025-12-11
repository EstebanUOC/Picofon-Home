using System;
using TMPro;
using UnityEngine;

public struct ModalData
{
    public string Title;
    public string Message;
    public Action OnClose;
}

public class Modal : MonoBehaviour
{
    [Space(15)]
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Message;
    public CustomButtonBase ConfirmButton;

    public void Show(ModalData data)
    {
        gameObject.SetActive(true);
        Title.text = data.Title;
        Message.text = data.Message;

        ConfirmButton.RemoveAllListeners();
        ConfirmButton.OnClick += () => data.OnClose?.Invoke();
        ConfirmButton.OnClick += () => Hide();
    }

    public void Start()
    {
        ConfirmButton.OnClick += () => Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
