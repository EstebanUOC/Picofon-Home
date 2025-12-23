using UnityEngine;

public class BallMovement : MonoBehaviour
{
    public Transform DriblePosition;

    private float time = 0;
    private readonly float duration = 1;

    private Vector3 initial;
    private bool ballIsFlying = false;
    private bool ballIsDribling = true;

    private Transform TargetPosition { get; set; }

    private Rigidbody2D body;

    public void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (ballIsFlying)
        {
            time += Time.deltaTime;
            float t01 = time / duration;

            Vector3 a = initial;
            Vector3 b = TargetPosition.position;

            Vector3 pos = Vector3.Lerp(a, b, t01);
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            transform.position = pos + arc;

            if (t01 >= 1)
            {
                body.bodyType = RigidbodyType2D.Dynamic;
                // body.velocity = new Vector2(-1, -1) * 12.5f;
                body.velocity = new Vector2(1, -1) * 12.5f;
                ballIsFlying = false;
            }
        }

        if (ballIsDribling)
        {
            time += Time.deltaTime;
            Vector3 drible = Vector3.up * Mathf.Abs(Mathf.Sin(time * 5));
            transform.position = DriblePosition.position + drible;
        }
    }

    public void Launch(Transform target)
    {
        TargetPosition = target;

        body.bodyType = RigidbodyType2D.Kinematic;
        body.angularVelocity = 200;
        initial = transform.position;
        time = 0;

        ballIsFlying = true;
        ballIsDribling = false;
    }

    public void Reset()
    {
        transform.position = DriblePosition.position;
        body.bodyType = RigidbodyType2D.Kinematic;
        body.angularVelocity = 0;
        body.rotation = 0;
        body.velocity = Vector2.zero;

        ballIsDribling = true;
    }
}
