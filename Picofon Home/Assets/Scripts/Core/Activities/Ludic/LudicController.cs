using Picofon.Utils;
using PrimeTween;
using UnityEngine;

namespace Picofon.Activities
{
    public class LudicController : MonoBehaviour
    {
        [SerializeField]
        private Transform _barrel;

        [SerializeField]
        private CloudManager _cloudManager;

        public void Start()
        {
            _cloudManager.OnCloudTouched += CloudTouched;
        }

        public void FixedUpdate() { }

        public void CloudTouched(Vector3 position)
        {
            // Quaternion target = Quaternion.LookRotation(position - _barrel.position);
            //
            // Tween.LocalRotation(_barrel, target, 0.5f);

            _barrel.up = position - _barrel.position;
        }
    }
}
