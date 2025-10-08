using UnityEngine;
using UnityEngine.EventSystems;

public class BallController : MonoBehaviour, IPointerClickHandler
{
    private Transform hoopTarget;
    private bool isMoving = false;
    private float timeElapsed = 0f;
    private float travelTime = 1.0f; // Duration of the throw
    private Vector3 startPos;
    private Vector3 targetPos;
    private float arcHeight = 200f; // Adjust for how high the ball should travel

    public void Initialize(Transform target)
    {
        hoopTarget = target;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isMoving || hoopTarget == null)
            return;

        isMoving = true;
        startPos = transform.position;
        targetPos = hoopTarget.position;
        timeElapsed = 0f;
    }

    void Update()
    {
        if (!isMoving) return;

        timeElapsed += Time.deltaTime;
        float t = timeElapsed / travelTime;

        // Parabolic interpolation
        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * t);

        transform.position = currentPos;

        if (t >= 1f)
        {
            isMoving = false;
            Destroy(gameObject, 0.2f); // Optional delay before disappearing
        }
    }
}
