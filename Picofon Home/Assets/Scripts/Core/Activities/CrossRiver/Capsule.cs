using PrimeTween;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    private float _jumpDuration = 0.5f;

    private float _targetY;

    private float _startY;

    private float _jumpHeight;
    private float _jumpTime;
    private float _sineTime;

    private bool _isJumping = false;
    private bool _isLanding = false;

    private FloatItem _currentFloatItem;

    private const float _offset = 1f;
    private const float _compensation = 4f; // Normaliza la parábola para que el máximo sea exactamente jumpHeight

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    public void Start()
    {
        _startY = transform.position.y;

        _sineTime = Mathf.PI;
    }

    public void FixedUpdate()
    {
        if (_isLanding)
        {
            _sineTime += Time.deltaTime;

            float offset = Mathf.Sin(_sineTime * _speed) * _amplitude;

            if (offset >= 0)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    _startY,
                    transform.position.z
                );

                _startY = transform.position.y;

                Tween.Delay(
                    target: _currentFloatItem,
                    duration: 2,
                    target =>
                    {
                        _isLanding = false;
                        _jumpTime = 0f;
                        _sineTime = Mathf.PI;

                        target.SetFloating(true);
                    }
                );
                return;
            }

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );

            return;
        }

        if (!_isJumping)
        {
            _sineTime += Time.deltaTime;

            float offset = Mathf.Sin(_sineTime * _speed) * _amplitude;

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );

            return;
        }

        _jumpTime += Time.deltaTime;
        float t = _jumpTime / _jumpDuration;

        if (t >= 1f)
        {
            _isJumping = false;
            _isLanding = true;
            _jumpTime = 0f;

            transform.position = new Vector3(transform.position.x, _targetY, transform.position.z);

            _startY = transform.position.y;
            _sineTime = Mathf.PI;

            _currentFloatItem.SetFloating(true);
            return;
        }

        float a = _compensation * _jumpHeight;

        float targetY = _targetY - _startY;

        float b = a + targetY;

        float height = -a * (t * t) + b * t;

        transform.position = new Vector3(
            transform.position.x,
            _startY + height,
            transform.position.z
        );
    }

    public void JumpTo(FloatItem floatItem)
    {
        _isJumping = true;
        _targetY = floatItem.transform.position.y + _offset;

        _jumpHeight = 3f;

        if (_currentFloatItem is null || _currentFloatItem == floatItem)
        {
            _jumpHeight = 1f;
        }

        _currentFloatItem = floatItem;
    }
}
