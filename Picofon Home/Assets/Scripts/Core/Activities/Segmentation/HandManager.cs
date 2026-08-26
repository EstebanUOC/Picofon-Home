using UnityEngine;

namespace Picofon.Activities
{
    public class HandManager : MonoBehaviour
    {
        # region References

        [SerializeField]
        private GameObject[] hands;

        # endregion

        private int _fingers = 0;

        public int Fingers
        {
            get { return _fingers; }
            set
            {
                if (_fingers == value)
                    return;

                if (value < 0 || value >= hands.Length)
                    return;

                hands[_fingers].SetActive(false);

                _fingers = value;

                hands[_fingers].SetActive(true);
            }
        }
    }
}
