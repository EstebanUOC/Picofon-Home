using System;
using PrimeTween;
using UnityEngine;

namespace Picofon.Activities
{
    public class LudicController : MonoBehaviour
    {
        [SerializeField]
        private Transform _barrel;

        [SerializeField]
        private Transform _markerBarrel;

        [SerializeField]
        private CloudManager _cloudManager;

        [SerializeField]
        private GameObject _bullet;

        private Action _onBulletImpact;
        private Action _onBarrelRotationComplete;

        private Transform _cloud;

        public void Start()
        {
            _cloudManager.OnCloudTouched += CloudTouched;

            _bullet.SetActive(false);

            _onBulletImpact = () =>
            {
                _bullet.SetActive(false);

                Tween.ShakeScale(
                    _cloud,
                    strength: Vector3.one * 0.2f,
                    duration: 0.5f,
                    frequency: 10,
                    enableFalloff: true,
                    easeBetweenShakes: Ease.OutQuad
                );
            };

            _onBarrelRotationComplete = () =>
            {
                _bullet.transform.position = _markerBarrel.position;
                _bullet.SetActive(true);

                Tween
                    .Position(_bullet.transform, _cloud.position, 0.20f, Ease.InQuad)
                    .OnComplete(_onBulletImpact);
            };
        }

        public void FixedUpdate() { }

        public void CloudTouched(Transform cloud)
        {
            _cloud = cloud;

            Vector3 direction = cloud.position - _barrel.position;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle -= 90f;

            Tween
                .Rotation(_barrel, new Vector3(0, 0, angle), 0.2f, Ease.OutQuad)
                .OnComplete(_onBarrelRotationComplete);
        }
    }
}
