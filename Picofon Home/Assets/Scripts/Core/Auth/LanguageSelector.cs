using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LanguageSelector : MonoBehaviour, IPointerClickHandler
{
    public event Action<LanguageData> OnLanguageSelected;

    public void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 0, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        LanguageItem item = eventData.rawPointerPress.GetComponent<LanguageItem>();

        if (item != null)
        {
            OnLanguageSelected?.Invoke(item.Data);
        }
    }
}
