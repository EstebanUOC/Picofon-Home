using UnityEngine;

namespace Picofon.Activities
{
    public class LudicController : MonoBehaviour
    {
        [SerializeField]
        private CloudManager _cloudManager;

        [SerializeField]
        private Cannon _cannon;

        public void Start()
        {
            _cloudManager.OnCloudTouched += CloudTouched;
        }

        public void CloudTouched(Transform cloud)
        {
            if (_cannon.CanShoot())
            {
                _cannon.Shoot(cloud);
            }
        }
    }
}
