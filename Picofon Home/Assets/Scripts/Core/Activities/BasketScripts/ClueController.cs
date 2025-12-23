using System;
using UnityEngine;
using UnityEngine.UI;

public class ClueController : MonoBehaviour
{
    public event Action<bool> OnClueActived;

    private Button _buttonComponent;

    public void Awake()
    {
        _buttonComponent = GetComponent<Button>();
        _buttonComponent.onClick.AddListener(OnClueButtonClicked);

        BasketManager.Instance.OnActivityChange += Reset;
    }

    private void Reset(in BasketResponses.BasketActivity _)
    {
        _buttonComponent.interactable = true;
    }

    private void OnClueButtonClicked()
    {
        OnClueActived?.Invoke(true);
        _buttonComponent.interactable = false;
    }
}
