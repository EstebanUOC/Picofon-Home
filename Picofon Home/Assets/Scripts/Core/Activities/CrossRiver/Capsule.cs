using UnityEngine;

public class Capsule : MonoBehaviour
{
    [SerializeField]
    private float jumpDuration = 0.5f;

    private float _targetY;

    private float speed = 2f;
    private float amplitude = 0.5f;

    private float jumpTime;
    private float startY;

    private bool isJumping = false;

    private const float _offset = 1f;

    public void Start()
    {
        startY = transform.position.y;
    }

    public void Update()
    {
        // float offset = Mathf.Sin(Time.time * speed) * amplitude;
        //
        // transform.position = new Vector3(
        //     transform.position.x,
        //     startY + offset,
        //     transform.position.z
        // );

        if (Input.GetKeyDown(KeyCode.A) && !isJumping)
        {
            isJumping = true;
            _targetY = 1.3f;
            startY = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.B) && !isJumping)
        {
            isJumping = true;
            _targetY = -3.4f;
            startY = transform.position.y;
        }

        if (Input.GetKeyDown(KeyCode.C) && !isJumping)
        {
            transform.position = new Vector3(transform.position.x, startY, transform.position.z);
        }

        // if (isJumping)
        // {
        //     jumpTime += Time.deltaTime;
        //     float t = jumpTime / jumpDuration;
        //
        //     if (t >= 1f)
        //     {
        //         isJumping = false;
        //         jumpTime = 0f;
        //         transform.position = new Vector3(
        //             transform.position.x,
        //             startY,
        //             transform.position.z
        //         );
        //         return;
        //     }
        //
        //     float height = 4 * jumpHeight * t * (1 - t);
        //
        //     transform.position = new Vector3(
        //         transform.position.x,
        //         startY + height,
        //         transform.position.z
        //     );
        // }

        if (isJumping)
        {
            jumpTime += Time.deltaTime;
            float t = jumpTime / jumpDuration;

            if (t >= 1f)
            {
                isJumping = false;
                jumpTime = 0f;
                return;
            }

            Vector3 a = transform.position;
            Vector3 b = new(transform.position.x, _targetY + _offset, transform.position.z);

            Vector3 pos = Vector3.Lerp(a, b, t);

            transform.position = pos;
        }
    }
}
