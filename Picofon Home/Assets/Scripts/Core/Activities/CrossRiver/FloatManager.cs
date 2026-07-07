using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class FloatManager : MonoBehaviour
{
    [SerializeField]
    private Capsule _capsule;

    [SerializeField]
    private FloatItem[] _floats;

    [SerializeField]
    private Transform _items;

    [SerializeField]
    private Transform _background;

    private int _currentFloatIndex;

    public void Start()
    {
        CalledDefered().Forget();

        _floats[1].gameObject.SetActive(true);
        _floats[1].gameObject.SetActive(false);

        _floats[2].ShowCheap();
        _floats[3].ShowCheap();
    }

    private async UniTaskVoid CalledDefered()
    {
        await UniTask.WaitForEndOfFrame(this);

        _currentFloatIndex = 0;
        _floats[_currentFloatIndex].Floating();
    }

    public void NotifyLanding()
    {
        _floats[_currentFloatIndex].Landing();
    }

    public void NotifyMovingComplete()
    {
        Span<int> moved = stackalloc int[2];

        if (_currentFloatIndex > 1)
        {
            moved[0] = 0;
            moved[1] = 1;
        }
        else
        {
            moved[0] = 2;
            moved[1] = 3;
        }

        foreach (int index in moved)
        {
            Vector3 oldTransform = _floats[index].transform.position;

            oldTransform.x = 5.5f;

            if ((index & 1) == 0)
            {
                oldTransform.y = 0;
            }
            else
            {
                oldTransform.y = -3.4f;
            }

            _floats[index].transform.localPosition = oldTransform;

            _floats[index].Revive();
        }
    }

    public void OnFloatItemClicked(FloatItem floatItem)
    {
        Span<int> drowned = stackalloc int[2];

        for (int i = 0; i < _floats.Length; i++)
        {
            if (_floats[i] != floatItem)
            {
                continue;
            }

            drowned[0] = _currentFloatIndex;

            bool isPair = (i & 1) == 0;

            drowned[1] = i + (isPair ? 1 : -1);

            _currentFloatIndex = i;
            break;
        }

        Tween.LocalPositionX(_floats[_currentFloatIndex].transform, endValue: -4, duration: 0.5f);

        foreach (int index in drowned)
        {
            _floats[index].Drown();

            Transform floatTransform = _floats[index].transform;

            Tween.LocalPositionX(
                floatTransform,
                endValue: floatTransform.position.x - 9.5f,
                duration: 0.5f
            );
        }

        Tween.LocalPositionX(_items, endValue: -9.5f, duration: 0.5f);
        Tween.LocalPositionX(_background, endValue: -9.5f, duration: 0.5f);

        _capsule.JumpTo(floatItem);
    }
}
