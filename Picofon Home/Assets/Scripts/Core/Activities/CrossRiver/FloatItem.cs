using PrimeTween;
using UnityEngine;

public class FloatItem : MonoBehaviour
{
    [SerializeField]
    private Transform _cheap;

    private bool _isFloating = false;

    private float _startY;

    private float _time;

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    public void Start()
    {
        _startY = transform.position.y;

        if (_cheap is null)
        {
            return;
        }

        _cheap.localScale = Vector3.zero;
        _cheap.localPosition = Vector3.zero;

        float duration = 0.5f;

        Sequence
            .Create(cycles: -1, cycleMode: Sequence.SequenceCycleMode.Restart)
            .Chain(
                Tween.Scale(
                    _cheap,
                    endValue: Vector3.one * 0.25f,
                    duration: duration,
                    ease: Ease.OutBack
                )
            )
            .Group(
                Tween.LocalPositionY(_cheap, endValue: 1f, duration: duration, ease: Ease.OutBack)
            )
            .ChainDelay(2)
            .Chain(
                Tween.Scale(_cheap, endValue: Vector3.zero, duration: duration, ease: Ease.InBack)
            )
            .Group(
                Tween.LocalPositionY(_cheap, endValue: 0f, duration: duration, ease: Ease.InBack)
            )
            .ChainDelay(2);
    }

    public void FixedUpdate()
    {
        if (!_isFloating)
            return;

        _time += Time.deltaTime;

        float offset = Mathf.Sin(_time * _speed) * _amplitude;

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );
    }

    public void SetFloating(bool isFloating)
    {
        _isFloating = isFloating;

        _time = Mathf.PI;
    }
}
