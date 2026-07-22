using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class FloatManager : MonoBehaviour
{
    public event Action<int> OnFloatClicked;

    #region References

    [SerializeField]
    private Capsule _capsule;

    [SerializeField]
    private FloatItem[] _floats;

    [SerializeField]
    private Transform _background;

    [SerializeField]
    private FrameManager _frameManager;

    [SerializeField]
    private ParticleSystem _jumpParticles;

    #endregion

    // Variables

    private int _currentFloatIndex;
    private bool _interactable;

    public void Start()
    {
        CalledDefered().Forget();

        _interactable = false;

        _floats[2].Drown();
        _floats[3].Drown();
    }

    private async UniTaskVoid CalledDefered()
    {
        await UniTask.WaitForEndOfFrame(this);

        _currentFloatIndex = 0;
        _floats[_currentFloatIndex].Floating();
    }

    public void EnableInteraction()
    {
        _interactable = true;
    }

    public void DisableInteraction()
    {
        _interactable = false;
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
            Vector3 pos = _floats[index].transform.position;

            pos.x = 5.5f;

            if ((index & 1) == 0)
            {
                pos.y = 0;
            }
            else
            {
                pos.y = -3.4f;
            }

            _floats[index].transform.localPosition = pos;

            _floats[index].Revive();
        }

        _frameManager.ShowFrames();
    }

    public void OnFloatItemClicked(FloatItem floatItem)
    {
        if (!_interactable)
            return;

        int clickedIndex = -1;

        for (int i = 0; i < _floats.Length; i++)
        {
            if (_floats[i] != floatItem)
                continue;

            clickedIndex = i;
            break;
        }

        if (clickedIndex < 0)
            return;

        Span<int> drowned = stackalloc int[2];
        drowned[0] = _currentFloatIndex;
        drowned[1] = (clickedIndex & 1) == 0 ? clickedIndex + 1 : clickedIndex - 1;
        _currentFloatIndex = clickedIndex;

        OnFloatClicked?.Invoke(clickedIndex);

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

        _frameManager.HideFrames();

        Tween.LocalPositionX(_background, endValue: -9.5f, duration: 0.5f);

        _jumpParticles.Play();
        _capsule.JumpTo(floatItem);
    }

    public void ReviveInitialFloats()
    {
        _interactable = true;

        _floats[2].Revive();
        _floats[3].Revive();
    }
}
