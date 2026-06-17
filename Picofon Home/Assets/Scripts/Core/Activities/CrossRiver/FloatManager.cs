<<<<<<< HEAD
=======
using PrimeTween;
>>>>>>> origin/feature/cross-river
using UnityEngine;

public class FloatManager : MonoBehaviour
{
    [SerializeField]
<<<<<<< HEAD
=======
    private Capsule _capsule;

    [SerializeField]
>>>>>>> origin/feature/cross-river
    private FloatItem _standFloat;

    [SerializeField]
    private FloatItem _floatItem1;

    [SerializeField]
    private FloatItem _floatItem2;

<<<<<<< HEAD
=======
    [SerializeField]
    private Transform _floats;

    [SerializeField]
    private Transform _items;

    [SerializeField]
    private Transform _background;

>>>>>>> origin/feature/cross-river
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

<<<<<<< HEAD
=======
        _capsule.JumpTo(floatItem);

>>>>>>> origin/feature/cross-river
        FloatItem other = _floatItem1;

        if (other == floatItem)
        {
            other = _floatItem2;
        }

<<<<<<< HEAD
        _standFloat.SetFloating(true);
        other.HideCheap();
=======
        other.HideCheap();

        Tween.LocalPositionX(_floats, endValue: -9.5f, duration: 0.5f);
        Tween.LocalPositionX(_items, endValue: -9.5f, duration: 0.5f);
        Tween.LocalPositionX(_background, endValue: -9.5f, duration: 0.5f);
>>>>>>> origin/feature/cross-river
    }
}
