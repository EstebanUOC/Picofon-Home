using System;
using System.Reflection.PortableExecutable;
using Picofon.Utils;
using PrimeTween;
using UnityEngine;

namespace Picofon.Activities
{
    public class Cannon : MonoBehaviour
    {
        private const float _chargeDuration = 0.5f;
        private const float _chargeScale = 2.7f;

        [SerializeField]
        private Transform _charge;

        [SerializeField]
        private Transform _barrel;

        [SerializeField]
        private Transform _markerBarrel;

        [SerializeField]
        private GameObject _bullet;

        private Transform _cloud;

        private bool _canShoot = true;

        private Action _onBulletImpact;
        private Action _onBarrelRotationComplete;

        private Action _onCompleteCharge;

        public void Start()
        {
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

            _onCompleteCharge = () =>
            {
                _canShoot = true;
            };
        }

        public void Shoot(Transform cloud)
        {
            _cloud = cloud;
            _canShoot = false;

            _charge.localScale = new Vector3(0, _charge.localScale.y, 1);

            Tween
                .ScaleX(_charge, endValue: _chargeScale, duration: _chargeDuration)
                .OnComplete(_onCompleteCharge);

            Vector3 direction = cloud.position - _barrel.position;

            if (direction.normalized == _barrel.up)
            {
                _onBarrelRotationComplete.Invoke();
                return;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            angle -= 90f;

            Tween
                .Rotation(_barrel, new Vector3(0, 0, angle), 0.2f, Ease.OutQuad)
                .OnComplete(_onBarrelRotationComplete);
        }

        public bool CanShoot()
        {
            return _canShoot;
        }
    }
}
