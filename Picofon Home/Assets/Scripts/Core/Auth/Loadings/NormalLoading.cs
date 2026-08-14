namespace Picofon.Core.Auth.Loadings
{
    using PrimeTween;
    using UnityEngine;

    public class NormalLoading : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _loadingIcon;

        private Tween _rotation;

        public void Show()
        {
            _rotation = Tween.EulerAngles(
                _loadingIcon,
                startValue: Vector3.zero,
                endValue: Vector3.forward * 360,
                duration: 1,
                ease: Ease.OutCubic,
                cycles: -1
            );
        }

        public void Hide()
        {
            _rotation.Complete();

            gameObject.SetActive(false);
        }
    }
}
