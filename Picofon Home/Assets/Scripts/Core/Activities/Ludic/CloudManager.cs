using System;
using UnityEngine;

namespace Picofon.Activities
{
    public class CloudManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

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
    }
}
