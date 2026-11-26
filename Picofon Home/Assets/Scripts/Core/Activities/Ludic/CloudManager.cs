using System;
using System.Collections.Generic;
using UnityEngine;

namespace Picofon.Activities
{
    public class CloudManager : MonoBehaviour
    {
        private const int _maxTouches = 3;
        private const float _friendOffset = 1.76f;

        // -5.37 | -3.27

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private Transform _friendContainer;

        [SerializeField]
        private GameObject _friendPrefab;

        [SerializeField]
        private Vector2 _initialFriendPosition = new(-5.37f, -3.27f);

        private Dictionary<GameObject, int> _clouds = new(7);

        public event Action<Transform> OnCloudTouched;

        public void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider == null)
            {
                return;
            }

            Transform cloudTransform = hit.collider.transform;

            OnCloudTouched.Invoke(cloudTransform);
        }

        public bool CloudTouched(Transform cloud)
        {
            if (!_clouds.ContainsKey(cloud.gameObject))
            {
                _clouds[cloud.gameObject] = 0;
            }

            _clouds[cloud.gameObject]++;

            if (_clouds[cloud.gameObject] == _maxTouches)
            {
                cloud.gameObject.SetActive(false);

                // Instantiate(_friendPrefab, _friendContainer).transform.position = new Vector3(
                //     _initialFriendPosition.x + (_clouds.Count - 1) * _friendOffset,
                //     _initialFriendPosition.y,
                //     0
                // );

                Instantiate(_friendPrefab, _friendContainer).transform.position = cloud.position;

                return false;
            }

            return true;
        }
    }
}
