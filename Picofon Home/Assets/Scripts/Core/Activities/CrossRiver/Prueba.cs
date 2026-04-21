using System;
using PrimeTween;
using UnityEngine;

public class Prueba : MonoBehaviour
{
    [SerializeField]
    private bool _interactable = true;

    [SerializeField]
    private Transform _transform;

    public event Action OnClick;

    public bool Interactable
    {
        get => _interactable;
    }

    public void OnMouseDown()
    {
        Sequence
            .Create()
            .Group(Tween.ScaleY(_transform, 0.9f, 0.15f))
            .Chain(Tween.ScaleY(_transform, 1f, 0.15f));

        OnClick?.Invoke();
    }
}
