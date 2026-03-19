using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTransition : MonoBehaviour
{
    [SerializeField]
    private RectTransform _loading;

    [SerializeField]
    private RectTransform _check;

    [SerializeField]
    private CanvasGroup _orientationGroup;

    [SerializeField]
    private RectTransform _phone;

    public void Start()
    {
        Pruebita().Forget();
    }

    private async UniTaskVoid Pruebita()
    {
        Image iconImage = _loading.GetComponent<Image>();

        Image checkImage = _check.GetComponent<Image>();

        Tween tween = Tween.EulerAngles(
            _loading,
            startValue: Vector3.zero,
            endValue: Vector3.forward * 360f,
            duration: 1f,
            cycles: -1
        );

        await UniTask.WaitForSeconds(1.5f);

        tween.Stop();

        _ = Sequence
            .Create()
            .Group(
                Tween.EulerAngles(
                    _loading,
                    startValue: _loading.localEulerAngles,
                    endValue: Vector3.forward * 360f,
                    duration: 1f
                )
            )
            .Chain(Tween.Alpha(iconImage, startValue: 1f, endValue: 0f, duration: 0.5f))
            .Chain(Tween.Alpha(checkImage, startValue: 0f, endValue: 1f, duration: 0.5f))
            .Group(
                Tween.Scale(
                    _check,
                    startValue: Vector3.zero,
                    endValue: Vector3.one,
                    duration: 0.5f,
                    ease: Ease.OutBack
                )
            )
            .Chain(
                Tween.Alpha(
                    checkImage,
                    startValue: 1f,
                    endValue: 0f,
                    duration: 0.5f,
                    startDelay: 0.5f
                )
            )
            .Chain(Tween.Alpha(_orientationGroup, startValue: 0f, endValue: 1f, duration: 0.5f))
            .Chain(
                Tween.EulerAngles(
                    _phone,
                    startValue: Vector3.zero,
                    endValue: Vector3.forward * -90,
                    duration: 1f
                )
            );
    }
}
