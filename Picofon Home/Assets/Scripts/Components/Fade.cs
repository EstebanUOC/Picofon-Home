namespace Picofon.Components
{
    using System;
    using PrimeTween;
    using UnityEngine;

    public class Fade : MonoBehaviour
    {
        [SerializeField]
        private Material _material;

        [SerializeField]
        private GameObject _rocket;

        // Actions

        private Action _onComplete;

        // Variables

        private SpriteRenderer _rocketImage;

        private Sequence _currentSequence;

        public void Awake()
        {
            if (_rocket == null)
                return;

            _rocketImage = _rocket.GetComponent<SpriteRenderer>();
            _rocketImage.color = new Color(1, 1, 1, 0);

            _onComplete = () =>
            {
                _currentSequence.Complete();

                gameObject.SetActive(false);
            };
        }

        public void OnDestroy()
        {
            _material.SetFloat("_Radius", 0);
        }

        public void Active()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void FirstLoad()
        {
            Active();

            _material.SetFloat("_Radius", 0);

            Vector2 rocketPosition = _rocket.transform.localPosition;
            float initialRocketX = rocketPosition.x;

            _currentSequence = Sequence
                .Create()
                .Group(
                    Tween.Alpha(target: _rocketImage, startValue: 0, endValue: 1, duration: 0.45f)
                )
                .Group(
                    Tween.LocalPositionX(
                        target: _rocket.transform,
                        endValue: initialRocketX + 1,
                        duration: 1,
                        cycles: 100,
                        cycleMode: CycleMode.Yoyo,
                        ease: Ease.InOutSine
                    )
                );
        }

        public void Load()
        {
            Active();

            Vector2 rocketPosition = _rocket.transform.localPosition;
            float initialRocketX = rocketPosition.x;

            _currentSequence = Sequence
                .Create()
                .Group(
                    Tween.Custom(
                        target: _material,
                        startValue: 1.2f,
                        endValue: 0,
                        duration: 0.7f,
                        onValueChange: (target, value) => target.SetFloat("_Radius", value)
                    )
                )
                .Chain(
                    Tween.Alpha(target: _rocketImage, startValue: 0, endValue: 1, duration: 0.45f)
                )
                .Group(
                    Tween.LocalPositionX(
                        target: _rocket.transform,
                        endValue: initialRocketX + 1,
                        duration: 1,
                        cycles: 100,
                        cycleMode: CycleMode.Yoyo,
                        ease: Ease.InOutSine
                    )
                );
        }

        public Sequence Stop<T>(T target, Action<T> onComplete)
            where T : class
        {
            Active();

            return Sequence
                .Create()
                .Group(
                    Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f)
                )
                .OnComplete(target: target, onComplete: onComplete);
        }

        public Sequence Stop(Action onComplete)
        {
            Active();

            return Sequence
                .Create()
                .Group(
                    Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f)
                )
                .OnComplete(onComplete: onComplete);
        }

        public void StopAndZoom()
        {
            Active();

            Sequence
                .Create()
                .Group(
                    Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f)
                )
                .Chain(
                    Tween.Custom(
                        target: _material,
                        startValue: 0,
                        endValue: 1.2f,
                        duration: 0.6f,
                        onValueChange: (target, value) => target.SetFloat("_Radius", value)
                    )
                )
                .OnComplete(_onComplete);
        }

        public Sequence ZoomIn()
        {
            Active();

            _material.SetFloat("_Radius", 0);

            Sequence sequence = Sequence
                .Create()
                .OnComplete(target: this, onComplete: target => target.gameObject.SetActive(false))
                .Group(
                    Tween.Custom(
                        target: _material,
                        startValue: 0,
                        endValue: 1.2f,
                        duration: 0.6f,
                        onValueChange: (target, value) => target.SetFloat("_Radius", value)
                    )
                );

            return sequence;
        }
    }
}
