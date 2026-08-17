using Picofon.Utils;

namespace Picofon.Activities.CrossRiver
{
    using System;
    using PrimeTween;
    using UnityEngine;

    public class FloatItem : MonoBehaviour
    {
        #region Constants

        private const int _stateIdle = 0;
        private const int _stateFloating = 1;
        private const int _stateLanding = 2;
        private const int _stateMoving = 3;

        private const float _speed = 5f;
        private const float _amplitude = 0.05f;

        #endregion

        #region References

        [SerializeField]
        private Transform _cheap;

        [SerializeField]
        private Transform _sprite;

        [SerializeField]
        private FloatSelectable _selectable;

        [SerializeField]
        private FloatManager _manager;

        #endregion

        // Variables

        private float _startY;
        private float _time;

        private Sequence _cheapSeq;

        private StateMachine _stateMachine;

        // Actions

        private Action _onMovingComplete;

        public void Awake()
        {
            _stateMachine = new StateMachine(4);

            _stateMachine.SetCallback(_stateIdle, IdleUpdate);
            _stateMachine.SetCallback(_stateFloating, FloatingUpdate);
            _stateMachine.SetCallback(_stateLanding, LandingUpdate);
            _stateMachine.SetCallback(_stateMoving, MovingUpdate, begin: BeginMoving);

            _stateMachine.ForceState(_stateIdle);

            _startY = transform.position.y;

            _selectable.OnClick += OnClick;

            _cheap.localScale = Vector3.zero;
            _cheap.localPosition = Vector3.zero;

            _onMovingComplete = () =>
            {
                _startY = transform.position.y;
                _stateMachine.ForceState(_stateFloating);
            };
        }

        public void FixedUpdate()
        {
            _stateMachine.Update();
        }

        public void Landing()
        {
            _time = Mathf.PI;

            HideCheap();

            _stateMachine.ForceState(_stateLanding);
        }

        public void Floating()
        {
            _time = 0;

            HideCheap();

            _stateMachine.ForceState(_stateFloating);
        }

        public void Drown()
        {
            _stateMachine.ForceState(_stateIdle);

            HideCheap();

            Tween
                .Scale(target: _sprite, endValue: Vector3.zero, duration: 0.5f, ease: Ease.InBack)
                .OnComplete(target: gameObject, target => target.SetActive(false));
        }

        public void Revive()
        {
            _startY = transform.position.y;

            gameObject.SetActive(true);

            Tween
                .Scale(
                    target: _sprite,
                    startValue: Vector3.zero,
                    endValue: Vector3.one,
                    duration: 0.5f,
                    ease: Ease.OutBack
                )
                .OnComplete(target: this, target => target.ShowCheap());

            _stateMachine.ForceState(_stateIdle);
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

        public void ShowCheap()
        {
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
                    Tween.LocalPositionY(
                        _cheap,
                        endValue: 1f,
                        duration: duration,
                        ease: Ease.OutBack
                    )
                )
                .ChainDelay(2)
                .Chain(
                    Tween.Scale(
                        _cheap,
                        endValue: Vector3.zero,
                        duration: duration,
                        ease: Ease.InBack
                    )
                )
                .Group(
                    Tween.LocalPositionY(
                        _cheap,
                        endValue: 0f,
                        duration: duration,
                        ease: Ease.InBack
                    )
                )
                .ChainDelay(2);
        }

        private int MovingUpdate()
        {
            return _stateMoving;
        }

        private void BeginMoving()
        {
            Tween
                .LocalPositionY(transform, endValue: -1.7f, duration: 1)
                .OnComplete(_onMovingComplete);
        }

        private int IdleUpdate()
        {
            return _stateIdle;
        }

        private int FloatingUpdate()
        {
            _time += Time.deltaTime;

            float offset = Mathf.Sin(_time * _speed) * _amplitude;

            transform.position = new Vector3(
                transform.position.x,
                _startY + offset,
                transform.position.z
            );

            return _stateFloating;
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

        private void OnClick()
        {
            if (_stateMachine.State == _stateLanding)
            {
                PerformanceLog.Log("Clicked on floating item, ignoring.");
                return;
            }

            Sequence
                .Create()
                .Group(Tween.ScaleY(_sprite, 0.9f, 0.15f))
                .Chain(Tween.ScaleY(_sprite, 1f, 0.15f));

            _manager.OnFloatItemClicked(this);
        }
    }
}
