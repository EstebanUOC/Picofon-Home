using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public Button ConfirmButton;

    public void Show(ModalData data)
    {
        gameObject.SetActive(true);
        Title.text = data.Title;
        Message.text = data.Message;

        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(() => data.OnClose?.Invoke());
        ConfirmButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Start()
    {
        ConfirmButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    public void Hide()
    {
        EventSystem.current?.SetSelectedGameObject(null);

        var anim = ConfirmButton.animator;
        anim?.Rebind();

        gameObject.SetActive(false);
    }
}
