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
    private RectTransform _phone;

    [SerializeField]
    private CanvasGroup _orientationGroup;

    [Space]
    [SerializeField]
    private UIResponsiveTransform[] _responsiveTransforms;

    public void Start()
    {
        Pruebita().Forget();

        enabled = false;
    }

    public void FixedUpdate()
    {
        bool changeOrientation = SceneOrientationHelper.ChangeOrientation();

        if (changeOrientation)
        {
            Debug.Log($"Orientation changed to: {Screen.orientation}");
            OnOrientationChanged();
            enabled = false;
        }
    }

    private async UniTaskVoid Pruebita()
    {
        await UniTask.WaitForSeconds(1f);

#if UNITY_ANDROID
        await UniTask.WaitForSeconds(9f);
#endif

        Image iconImage = _loading.GetComponent<Image>();

        Image checkImage = _check.GetComponent<Image>();

        Tween tween = Tween.EulerAngles(
            _loading,
            startValue: Vector3.zero,
            endValue: Vector3.forward * 360f,
            duration: 1f,
            cycles: -1
        );

        await UniTask.WaitForSeconds(2.3f);

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
            )
            .OnComplete(() =>
            {
                SceneOrientationHelper.UnlockPortrait();
                enabled = true;
            });
    }

    private void OnOrientationChanged()
    {
        foreach (UIResponsiveTransform responsiveTransform in _responsiveTransforms)
        {
            RectTransform target = responsiveTransform.Target;

            target.anchoredPosition = responsiveTransform.Position;
            target.anchorMin = responsiveTransform.AnchorMin;
            target.anchorMax = responsiveTransform.AnchorMax;
            target.pivot = responsiveTransform.Pivot;
            target.localScale = responsiveTransform.Scale;
        }
    }
}
