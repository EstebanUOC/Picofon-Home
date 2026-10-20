using System;
using Picofon.Utils;
using UnityEngine;

namespace Picofon.Activities
{
    public class CloudManager : MonoBehaviour
    {
        [SerializeField]
        private Camera _camera;

        public event Action<Vector3> OnCloudTouched;

        public void Update()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }

            Vector3 mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null)
            {
                Transform cloudTransform = hit.collider.transform;
                PerformanceLog.Log($"Position nube: {cloudTransform.position}");

                OnCloudTouched.Invoke(cloudTransform.position);
            }
        }
    }
}
