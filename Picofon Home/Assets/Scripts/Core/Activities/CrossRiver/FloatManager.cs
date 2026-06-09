using PrimeTween;
using UnityEngine;

public class FloatManager : MonoBehaviour
{
    [SerializeField]
    private Capsule _capsule;

    [SerializeField]
    private FloatItem _standFloat;

    [SerializeField]
    private FloatItem _floatItem1;

    [SerializeField]
    private FloatItem _floatItem2;

    [SerializeField]
    private Transform _floats;

    [SerializeField]
    private Transform _items;

    public void Start()
    {
        _floatItem1.SetFloating(false);
        _floatItem2.SetFloating(false);

        _standFloat.SetFloating(true);
    }

    public void OnFloatItemClicked(FloatItem floatItem)
    {
        _standFloat.SetFloating(false);

        _standFloat = floatItem;

        _capsule.JumpTo(floatItem);

        FloatItem other = _floatItem1;

        if (other == floatItem)
        {
            other = _floatItem2;
        }

        other.HideCheap();

        Tween.LocalPositionX(_floats, endValue: -9.5f, duration: 0.5f);
        Tween.LocalPositionX(_items, endValue: -9.5f, duration: 0.5f);
    }
}
