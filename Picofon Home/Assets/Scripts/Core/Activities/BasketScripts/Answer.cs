using System;
using UnityEngine;

public class Answer : MonoBehaviour
{
    public Action OnClick;

    public void OnMouseDown()
    {
        OnClick?.Invoke();
    }
}
