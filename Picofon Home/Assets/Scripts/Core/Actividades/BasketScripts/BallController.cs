using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IPointerClickHandler
{
    private Transform hoopTarget;
    private bool isMoving = false;
    private float speed = 5f;

    public void Initialize(Transform target)
    {
        hoopTarget = target;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isMoving)
            isMoving = true;
    }

    void Update()
    {
        if (isMoving && hoopTarget != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, hoopTarget.position, speed * Time.deltaTime);

            // When it reaches the basket
            if (Vector3.Distance(transform.position, hoopTarget.position) < 0.1f)
            {
                isMoving = false;
                Destroy(gameObject); // Optional: remove after "scoring"
            }
        }
    }
}
