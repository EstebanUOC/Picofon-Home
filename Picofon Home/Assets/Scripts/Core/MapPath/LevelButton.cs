using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    public event Action OnClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClicked?.Invoke();
    }
}
