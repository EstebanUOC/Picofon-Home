using UnityEngine;

public class BallTest : MonoBehaviour
{
    public Transform TargetPoint;
    public Transform DriblePosition;

    private float time = 0;
    private Vector3 initial;
    private readonly float duration = 0.5f;

    private bool ballIsFlying = false;
    private bool ballIsDribling = true;

    private void Update()
    {
        if (ballIsFlying)
        {
            time += Time.deltaTime;
            float t01 = time / duration;

            Vector3 a = initial;
            Vector3 b = TargetPoint.position;

            Vector3 pos = Vector3.Lerp(a, b, t01);
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            transform.position = pos + arc;

            if (t01 >= 1)
            {
                Rigidbody2D body = GetComponent<Rigidbody2D>();
                body.bodyType = RigidbodyType2D.Dynamic;
                ballIsFlying = false;
                time = 0;
            }
        }

        if (ballIsDribling)
        {
            time += Time.deltaTime;
            Vector3 drible = Vector3.up * Mathf.Abs(Mathf.Sin(time * 5));
            transform.position = DriblePosition.position + drible;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            body.bodyType = RigidbodyType2D.Kinematic;
            initial = transform.position;
            time = 0;

            ballIsFlying = true;
            ballIsDribling = false;
        }
    }
}
