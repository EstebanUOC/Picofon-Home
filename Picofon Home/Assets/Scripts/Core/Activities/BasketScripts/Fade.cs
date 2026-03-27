using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    [SerializeField]
    private GameObject _rocket;

    public void FirstLoad()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        Image image = _rocket.GetComponent<Image>();

        Sequence
            .Create()
            .Group(Tween.Alpha(target: image, startValue: 0, endValue: 1, duration: 0.5f))
            .Chain(
                Tween.UIAnchoredPositionY(
                    target: image.rectTransform,
                    endValue: 100,
                    duration: 0.5f,
                    ease: Ease.OutBack
                )
            );
    }

    public void Load()
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

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
            .OnComplete(target: this, onComplete: target => target.gameObject.SetActive(false));
    }

    public void Stop() { }
}
