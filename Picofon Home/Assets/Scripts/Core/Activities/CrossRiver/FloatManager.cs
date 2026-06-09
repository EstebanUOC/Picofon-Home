using UnityEngine;

public class FloatManager : MonoBehaviour
{
    [SerializeField]
    private FloatItem _standFloat;

    [SerializeField]
    private FloatItem _floatItem1;

    [SerializeField]
    private FloatItem _floatItem2;

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

        FloatItem other = _floatItem1;

        if (other == floatItem)
        {
            other = _floatItem2;
        }

        _standFloat.SetFloating(true);
        other.HideCheap();
    }
}
