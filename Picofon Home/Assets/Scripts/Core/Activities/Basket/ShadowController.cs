namespace Picofon.Activities.Basket
{
    using UnityEngine;

    public class ShadowController : MonoBehaviour
    {
        public Transform BallTransform;

        private float initialY;

        public void Start()
        {
            initialY = transform.position.y;
        }

        public void FixedUpdate()
        {
            Vector3 newPosition = BallTransform.position;
            newPosition.y = initialY;

            transform.position = newPosition;
        }
    }
}
