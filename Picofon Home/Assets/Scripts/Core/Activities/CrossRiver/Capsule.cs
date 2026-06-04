using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField]
    private float _jumpDuration = 0.5f;

    [SerializeField]
    private FloatItem _floatItem1;

    [SerializeField]
    private FloatItem _floatItem2;

    [SerializeField]
    private FloatItem _currentFloat;

    private float _targetY;

    private float _startY;

    private float _jumpHeight;
    private float _jumpTime;
    private float _sineTime;

    private bool _isJumping = false;

    private FloatItem _currentFloatItem;

    private const float _offset = 1f;
    private const float _compensation = 4f; // Normaliza la parábola para que el máximo sea exactamente jumpHeight

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    public void Start()
    {
        _startY = transform.position.y;

        _currentFloatItem = _currentFloat;
        _currentFloatItem.SetFloating(true);

        _sineTime = Mathf.PI;
    }

    public void FixedUpdate()
    {
        if (_isJumping)
        {
            _jumpTime += Time.deltaTime;
            float t = _jumpTime / _jumpDuration;

            if (t >= 1f)
            {
                _isJumping = false;
                _jumpTime = 0f;

                transform.position = new Vector3(
                    transform.position.x,
                    _targetY,
                    transform.position.z
                );

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

        if (!_isJumping)
        {
            _sineTime += Time.deltaTime;

            float offset = Mathf.Sin(_sineTime * _speed) * _amplitude;

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) && !_isJumping)
        {
            _currentFloatItem.SetFloating(false);

            _isJumping = true;
            _targetY = 1.3f + _offset;

            _jumpHeight = 3f;

            if (_targetY == _startY)
            {
                _jumpHeight = 1f;
            }

            _currentFloatItem = _floatItem1;
        }

        if (Input.GetKeyDown(KeyCode.B) && !_isJumping)
        {
            _currentFloatItem.SetFloating(false);

            _isJumping = true;
            _targetY = -3.4f + _offset;

            _jumpHeight = 3f;

            if (_targetY == _startY)
            {
                _jumpHeight = 1f;
            }

            _currentFloatItem = _floatItem2;
        }

        if (Input.GetKeyDown(KeyCode.C) && !_isJumping)
        {
            transform.position = new Vector3(transform.position.x, _startY, transform.position.z);
        }
    }
}
