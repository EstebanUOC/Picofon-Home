using PrimeTween;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    public void Start() { }

    public void HideFrames()
    {
        float duration = 0.5f;

        Tween.LocalPositionY(transform, endValue: 5, duration, ease: Ease.InBack);

        Tween.LocalPositionX(transform, endValue: -9.5f, duration: duration);
    }

    public void ShowFrames()
    {
        transform.localPosition = new Vector3(0, 5, 0);

        float duration = 0.5f;

        Tween.LocalPositionY(transform, endValue: 0, duration, ease: Ease.OutBack);
    }
}
