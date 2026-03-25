using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Image _background;

    [SerializeField]
    private GameObject _icon;

    private Tween _rotationTween;

    public void Load()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        RectTransform iconRect = _icon.GetComponent<RectTransform>();

        _rotationTween = Tween.EulerAngles(
            iconRect,
            startValue: Vector3.zero,
            endValue: Vector3.forward * 360f,
            duration: 1f,
            cycles: -1
        );
    }

    public void Stop()
    {
        _rotationTween.Stop();

        Image iconImage = _icon.GetComponent<Image>();

        const float duration = 0.5f;

        Sequence
            .Create()
            .Group(Tween.Alpha(iconImage, startValue: 1f, endValue: 0f, duration: duration))
            .Group(Tween.Alpha(_background, startValue: 1f, endValue: 0f, duration: duration));
    }
}
