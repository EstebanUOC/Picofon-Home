using Picofon.Components;
using Picofon.Utils;

namespace Picofon.Core.Auth.Loadings
{
    using Cysharp.Threading.Tasks;
    using PrimeTween;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class MapLoading : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _loading;

        [SerializeField]
        private RectTransform _check;

        [SerializeField]
        private RectTransform _fail;

        [SerializeField]
        private RectTransform _animation;

        [SerializeField]
        private CanvasGroup _instruction;

        [SerializeField]
        private GameObject _button;

        [Space]
        [SerializeField]
        private UIResponsiveTransform[] _responsiveTransforms;

        private Tween _loadingTween;

        public void Show()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            _loadingTween = Tween.EulerAngles(
                _loading,
                startValue: Vector3.zero,
                endValue: Vector3.forward * 360f,
                duration: 1f,
                cycles: -1
            );
        }

        public void Continue(bool success)
        {
            Image loadingImage = _loading.GetComponent<Image>();

            RectTransform phone = _animation.GetChild(1) as RectTransform;

            RectTransform statusRect = _check;

            if (!success)
            {
                statusRect = _fail;
            }

            Image statusImage = statusRect.GetComponent<Image>();

            _loadingTween.Stop();

            Sequence sequence = Sequence
                .Create()
                .Group(
                    Tween.EulerAngles(
                        _loading,
                        startValue: _loading.localEulerAngles,
                        endValue: Vector3.forward * 360f,
                        duration: 1f
                    )
                )
                .Chain(Tween.Alpha(loadingImage, startValue: 1f, endValue: 0f, duration: 0.5f))
                .Chain(Tween.Alpha(statusImage, startValue: 0f, endValue: 1f, duration: 0.5f))
                .Group(
                    Tween.Scale(
                        statusRect,
                        startValue: Vector3.zero,
                        endValue: Vector3.one,
                        duration: 0.5f,
                        ease: Ease.OutBack
                    )
                );

            if (!success)
            {
                sequence.Chain(
                    Tween.ShakeLocalPosition(
                        statusRect,
                        strength: Vector3.right * 10f,
                        duration: 0.5f,
                        frequency: 20f
                    )
                );

                void onCompleteAction()
                {
                    Color whiteTransparent = new(1f, 1f, 1f, 0f);

                    loadingImage.color = Color.white;
                    statusImage.color = whiteTransparent;

                    gameObject.SetActive(false);
                }

                sequence.OnComplete(onCompleteAction);
                return;
            }

            sequence
                .Chain(
                    Tween.Alpha(
                        statusImage,
                        startValue: 1f,
                        endValue: 0f,
                        duration: 0.5f,
                        startDelay: 0.5f
                    )
                )
                .Chain(Tween.Alpha(_instruction, startValue: 0f, endValue: 1f, duration: 0.5f))
                .Chain(
                    Tween.EulerAngles(
                        phone,
                        startValue: Vector3.zero,
                        endValue: Vector3.forward * -90,
                        duration: 1f
                    )
                )
                .OnComplete(() =>
                {
                    SceneOrientationHelper.LockToLandscape();
                    OnOrientationChanged();
                });
        }

        private async UniTaskVoid ShowButton()
        {
            const float duration = 0.3f;

            await UniTask.WaitForSeconds(0.5f);

            CanvasGroup animationCanvasGroup = _animation.GetComponent<CanvasGroup>();
            CanvasGroup buttonCanvasGroup = _button.GetComponent<CanvasGroup>();

            _ = Sequence
                .Create()
                .Group(
                    Tween.Alpha(
                        animationCanvasGroup,
                        startValue: 1f,
                        endValue: 0f,
                        duration: duration
                    )
                )
                .Chain(
                    Tween.Alpha(buttonCanvasGroup, startValue: 0f, endValue: 1f, duration: duration)
                );
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

            ShowButton().Forget();

            SimpleButton buttonComponent = _button.GetComponent<SimpleButton>();

            buttonComponent.OnClick += () => SceneManager.LoadScene("MapPathScene");
        }
    }
}
