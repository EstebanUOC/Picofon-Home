using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField]
    private float _jumpDuration = 0.5f;

    private float _targetY;

    private float _jumpHeight;
    private float _time;
    private float _startY;

    private bool _isJumping = false;

    private const float _offset = 1f;
    private const float _compensation = 4f; // Normaliza la parábola para que el máximo sea exactamente jumpHeight

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    public void Start()
    {
        _startY = transform.position.y;
    }

    public void FixedUpdate()
    {
        float offset = Mathf.Sin(Time.time * _speed) * _amplitude;

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );

        if (Input.GetKeyDown(KeyCode.A) && !_isJumping)
        {
            _isJumping = true;
            _targetY = 1.3f + _offset;
            _startY = transform.position.y;

            _jumpHeight = 3f;

            if (_targetY == _startY)
            {
                _jumpHeight = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.B) && !_isJumping)
        {
            _isJumping = true;
            _targetY = -3.4f + _offset;
            _startY = transform.position.y;

            _jumpHeight = 3f;

            if (_targetY == _startY)
            {
                _jumpHeight = 1f;
            }
        }

        if (Input.GetKeyDown(KeyCode.C) && !_isJumping)
        {
            transform.position = new Vector3(transform.position.x, _startY, transform.position.z);
        }

        if (_isJumping)
        {
            _time += Time.deltaTime;
            float t = _time / _jumpDuration;

            if (t >= 1f)
            {
                _isJumping = false;
                _time = 0f;

                transform.position = new Vector3(
                    transform.position.x,
                    _targetY,
                    transform.position.z
                );
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
    }
}
