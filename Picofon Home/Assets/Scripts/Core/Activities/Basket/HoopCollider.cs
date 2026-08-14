namespace Picofon.Activities.Basket
{
    using UnityEngine;

    public class HoopCollider : MonoBehaviour
    {
        [Space(15)]
        [SerializeField]
        private AudioClip _scoreClip;

        [Space(15)]
        [SerializeField]
        private GameObject _borderCollider;

        [SerializeField]
        private GameObject _blockerCollider;

        public bool Blocked
        {
            get { return _blocked; }
            set
            {
                if (_blocked == value)
                    return;

                _blocked = value;
                _blockerCollider.SetActive(value);
                _borderCollider.SetActive(!value);
            }
        }

        private bool _blocked = false;

        public void OnTriggerEnter2D(Collider2D _)
        {
            AudioManager.Instance.PlaySFX(_scoreClip);
        }
    }
}
