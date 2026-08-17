namespace Picofon.Activities.CrossRiver
{
    using System;
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
        private const int _stateMoving = 3;

        #endregion

        [SerializeField]
        private FloatManager _floatManager;

        // Times

        private float _time;

        private float _jumpDuration = 0.5f;
        private float _jumpHeight;

        private float _targetY;
        private float _startY;

        private StateMachine _stateMachine;

        // Actions

        private Action _onMovingComplete;

        public void Start()
        {
            _stateMachine = new StateMachine(4);

            _stateMachine.SetCallback(_stateIdle, IdleUpdate);

            _stateMachine.SetCallback(_stateJump, JumpUpdate);

            _stateMachine.SetCallback(_stateLanding, LandingUpdate);

            _stateMachine.SetCallback(_stateMoving, MovingUpdate, begin: BeginMoving);

            _stateMachine.ForceState(_stateIdle);

            _startY = transform.position.y;

            _time = 0;

            _onMovingComplete = () =>
            {
                _startY = transform.position.y;

                _stateMachine.ForceState(_stateIdle);

                _floatManager.NotifyMovingComplete();
            };
        }

        public void FixedUpdate()
        {
            _stateMachine.Update();
        }

        public void JumpTo(FloatItem floatItem)
        {
            _stateMachine.ForceState(_stateJump);

            _time = 0f;

            _targetY = floatItem.transform.position.y + _offset;

            _jumpHeight = 3f;
        }

        private void BeginMoving()
        {
            Tween
                .LocalPositionY(transform, endValue: -0.7f, duration: 1)
                .OnComplete(_onMovingComplete);
        }

        private int MovingUpdate()
        {
            return _stateMoving;
        }

        private int IdleUpdate()
        {
            _time += Time.deltaTime;

            float offset = Mathf.Sin(_time * _speed) * _amplitude;

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );

            return _stateIdle;
        }

        private int JumpUpdate()
        {
            _time += Time.deltaTime;
            float t = _time / _jumpDuration;

            if (t >= 1f)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    _targetY,
                    transform.position.z
                );

                _startY = transform.position.y;
                _time = Mathf.PI;

                _floatManager.NotifyLanding();

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
            _time += Time.deltaTime;

            float offset = Mathf.Sin(_time * 5) * 0.3f;

            if (offset >= 0)
            {
                _time = 0;

                transform.position = new Vector3(
                    transform.position.x,
                    _startY,
                    transform.position.z
                );

                _startY = transform.position.y;

                return _stateMoving;
            }

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );

            return _stateLanding;
        }
    }
}
