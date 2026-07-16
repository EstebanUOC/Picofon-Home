using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;

public class FrameManager : MonoBehaviour
{
    // References

    [SerializeField]
    private Transform _container;

    // Variables

    public void Start()
    {
        ActivityRequestParams @params = LevelPayload.Params;

#if DEBUG
        if (@params.ChildId is null)
        {
            @params = new ActivityRequestParams { PlanId = 112, ChildId = "12345678Z" };

            PerformanceLog.LogWarning("Using default parameters for testing in Unity Editor.");
        }
# endif
    }

    private async UniTaskVoid Prueba()
    {
        await UniTask.Delay(100);
    }

    public void HideFrames()
    {
        float duration = 0.5f;

        Tween.LocalPositionY(_container, endValue: 5, duration, ease: Ease.InBack);

        Tween.LocalPositionX(_container, endValue: -9.5f, duration: duration);
    }

    public void ShowFrames()
    {
        _container.localPosition = new Vector3(0, 5, 0);

        float duration = 0.5f;

        Tween.LocalPositionY(_container, endValue: 0, duration, ease: Ease.OutBack);
    }
}
