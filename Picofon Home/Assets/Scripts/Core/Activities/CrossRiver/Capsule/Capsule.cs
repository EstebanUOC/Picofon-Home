using PrimeTween;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    #region Constants

    private const float _offset = 1f;
    private const float _compensation = 4f; // Normaliza la parábola para que el máximo sea exactamente jumpHeight

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    private const int _stateIdle = 0;
    private const int _stateJump = 1;
    private const int _stateLanding = 2;

    #endregion

    // Times

    private float _jumpTime;
    private float _sineTime;

    private float _jumpDuration = 0.5f;
    private float _jumpHeight;

    private float _targetY;
    private float _startY;

    private FloatItem _currentFloatItem;

    private StateMachine _stateMachine;

    public void Start()
    {
        _stateMachine = new StateMachine(3);

        _stateMachine.SetCallback(_stateIdle, IdleUpdate);

        _stateMachine.SetCallback(_stateJump, JumpUpdate);

        _stateMachine.SetCallback(_stateLanding, LandingUpdate);

        _stateMachine.ForceState(_stateIdle);

        _startY = transform.position.y;

        _sineTime = Mathf.PI;
    }

    private int IdleUpdate()
    {
        _sineTime += Time.deltaTime;

        float offset = Mathf.Sin(_sineTime * _speed) * _amplitude;

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );

        return _stateIdle;
    }

    private int JumpUpdate()
    {
        _jumpTime += Time.deltaTime;
        float t = _jumpTime / _jumpDuration;

        if (t >= 1f)
        {
            _jumpTime = 0f;

            transform.position = new Vector3(transform.position.x, _targetY, transform.position.z);

            _startY = transform.position.y;
            _sineTime = Mathf.PI;

            _currentFloatItem.SetFloating(true);

            return _stateLanding;
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

        return _stateJump;
    }

    private int LandingUpdate()
    {
        _sineTime += Time.deltaTime;

        float offset = Mathf.Sin(_sineTime * _speed) * _amplitude;

        if (offset >= 0)
        {
            transform.position = new Vector3(transform.position.x, _startY, transform.position.z);

            _startY = transform.position.y;

            Tween.Delay(
                target: _currentFloatItem,
                duration: 2,
                target =>
                {
                    _jumpTime = 0f;
                    _sineTime = Mathf.PI;

                    target.SetFloating(true);
                }
            );

            return _stateIdle;
        }

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );

        return _stateLanding;
    }

    public void FixedUpdate()
    {
        _stateMachine.Update();
    }

    public void JumpTo(FloatItem floatItem)
    {
        _stateMachine.ForceState(_stateJump);

        _targetY = floatItem.transform.position.y + _offset;

        _jumpHeight = 3f;

        if (_currentFloatItem is null || _currentFloatItem == floatItem)
        {
            _jumpHeight = 1f;
        }

        _currentFloatItem = floatItem;
    }
}
