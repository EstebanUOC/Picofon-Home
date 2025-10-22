using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BallController : MonoBehaviour, IPointerClickHandler
{
    [Header("References")]
    public Image innerImage; // Image shown on top of the ball
    private Transform hoopTarget;
    private bool isMoving = false;
    private float timeElapsed = 0f;
    private float travelTime = 1.0f;
    private float arcHeight = 200f;
    private bool isClickable = true;

    public void Initialize(Transform target, Sprite contentSprite, bool clickable)
    {
        hoopTarget = target;
        isClickable = clickable;

        if (innerImage != null && contentSprite != null)
            innerImage.sprite = contentSprite;
    }

    public void StartMoveTo(Transform target)
    {
        hoopTarget = target;
        isMoving = true;
        timeElapsed = 0f;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isClickable || isMoving || hoopTarget == null)
            return;

        isMoving = true;
        timeElapsed = 0f;
    }

    void Update()
    {
        if (!isMoving) return;

        timeElapsed += Time.deltaTime;
        float t = timeElapsed / travelTime;

        Vector3 startPos = transform.position;
        Vector3 targetPos = hoopTarget.position;

        Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
        currentPos.y += arcHeight * Mathf.Sin(Mathf.PI * t);
        transform.position = currentPos;

        if (t >= 1f)
        {
            isMoving = false;
            Destroy(gameObject, 0.2f);
        }
    }
}
