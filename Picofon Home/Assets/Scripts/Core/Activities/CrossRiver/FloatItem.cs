using PrimeTween;
using UnityEngine;

public class FloatItem : MonoBehaviour
{
    [SerializeField]
    private Transform _cheap;

    [SerializeField]
    private Transform _sprite;

    [SerializeField]
    private FloatSelectable _selectable;

    [SerializeField]
    private FloatManager _manager;

    private bool _isFloating = false;

    private float _startY;

    private float _time;

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    private Sequence _cheapSeq;

    public void Start()
    {
        _startY = transform.position.y;

        _selectable.OnClick += OnClick;

        _cheap.localScale = Vector3.zero;
        _cheap.localPosition = Vector3.zero;
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

        if (isFloating)
        {
            HideCheap();
            return;
        }

        _cheap.gameObject.SetActive(true);

        float duration = 0.5f;

        _cheapSeq = Sequence
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

    public void HideCheap()
    {
        if (_cheap.gameObject.activeInHierarchy)
        {
            _cheap.gameObject.SetActive(false);
        }

        if (_cheapSeq.isAlive)
        {
            _cheapSeq.Complete();
        }
    }

    private void OnClick()
    {
        if (_isFloating)
        {
            PerformanceLog.Log("Clicked on floating item, ignoring.");
            return;
        }

        Sequence
            .Create()
            .Group(Tween.ScaleY(_sprite, 0.9f, 0.15f))
            .Chain(Tween.ScaleY(_sprite, 1f, 0.15f));

        _manager.OnFloatItemClicked(this);

        SetFloating(true);
    }
}
