using UnityEngine;

public class Capsule : MonoBehaviour
{
    private float jumpHeight = 2f;

    private float speed = 2f;
    private float amplitude = 0.5f;

    [SerializeField]
    private float jumpDuration = 0.5f;

    private bool isJumping = false;
    private float jumpTime;
    private float startY;

    public void Start()
    {
        startY = transform.position.y;
    }

    public void Update()
    {
        float offset = Mathf.Sin(Time.time * speed) * amplitude;

        transform.position = new Vector3(
            transform.position.x,
            startY + offset,
            transform.position.z
        );

        if (Input.GetKeyDown(KeyCode.A) && !isJumping)
        {
            isJumping = true;
            jumpHeight = 2f;
            startY = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.B) && !isJumping)
        {
            isJumping = true;
            jumpHeight = 5f;
            startY = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.C) && !isJumping)
        {
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            float t = jumpTime / jumpDuration;

            if (t >= 1f)
            {
                isJumping = false;
                jumpTime = 0f;
                transform.position = new Vector3(
                    transform.position.x,
                    startY,
                    transform.position.z
                );
                return;
            }

            float height = 4 * jumpHeight * t * (1 - t);

            transform.position = new Vector3(
                transform.position.x,
                startY + height,
                transform.position.z
            );
        }
    }
}
