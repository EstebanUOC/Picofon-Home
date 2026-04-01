using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    [SerializeField]
    private GameObject _rocket;

    private Image _rocketImage;

    public void Awake()
    {
        if (_rocket == null)
            return;

        _rocketImage = _rocket.GetComponent<Image>();
        _rocketImage.color = new Color(1, 1, 1, 0);
    }

    public void FirstLoad()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _material.SetFloat("_Radius", 0);

        Vector2 rocketPosition = _rocket.transform.localPosition;
        float initialRocketX = rocketPosition.x;

        Sequence
            .Create()
            .Group(Tween.Alpha(target: _rocketImage, startValue: 0, endValue: 1, duration: 0.45f))
            .Group(
                Tween.Custom(
                    target: _rocket.transform,
                    startValue: 0,
                    endValue: 1,
                    duration: 1,
                    onValueChange: (target, value) =>
                    {
                        rocketPosition.x = initialRocketX + value * 30;

                        target.localPosition = rocketPosition;
                    },
                    cycles: 100,
                    cycleMode: CycleMode.Yoyo,
                    ease: Ease.InOutSine
                )
            );
    }

    public void StopAndZoom()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Sequence
            .Create()
            .Group(Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f))
            .Chain(
                Tween.Custom(
                    target: _material,
                    startValue: 0,
                    endValue: 1.2f,
                    duration: 0.6f,
                    onValueChange: (target, value) => target.SetFloat("_Radius", value)
                )
            )
            .OnComplete(target: this, onComplete: target => target.gameObject.SetActive(false));
    }

    public Sequence Stop<T>(T target, Action<T> onComplete)
        where T : class
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        return Sequence
            .Create()
            .Group(Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f))
            .OnComplete(target: target, onComplete: onComplete);
    }

    public Sequence Stop(Action onComplete)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        return Sequence
            .Create()
            .Group(Tween.Alpha(target: _rocketImage, startValue: 1, endValue: 0, duration: 0.5f))
            .OnComplete(onComplete: onComplete);
    }

    public void ZoomIn()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        _material.SetFloat("_Radius", 0);

        Sequence
            .Create()
            .Group(
                Tween.Custom(
                    target: _material,
                    startValue: 0,
                    endValue: 1.2f,
                    duration: 0.6f,
                    onValueChange: (target, value) => target.SetFloat("_Radius", value)
                )
            )
            .OnComplete(target: this, onComplete: target => target.gameObject.SetActive(false));
    }

    public void Load()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Vector2 rocketPosition = _rocket.transform.localPosition;
        float initialRocketX = rocketPosition.x;

        Sequence
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
            .Chain(Tween.Alpha(target: _rocketImage, startValue: 0, endValue: 1, duration: 0.45f))
            .Group(
                Tween.Custom(
                    target: _rocket.transform,
                    startValue: 0,
                    endValue: 1,
                    duration: 1,
                    onValueChange: (target, value) =>
                    {
                        rocketPosition.x = initialRocketX + value * 30;

                        target.localPosition = rocketPosition;
                    },
                    cycles: 100,
                    cycleMode: CycleMode.Yoyo,
                    ease: Ease.InOutSine
                )
            );
    }

    public void OnDestroy()
    {
        _material.SetFloat("_Radius", 0);
    }
}
