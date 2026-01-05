using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField]
    private Transform _driblePosition;

    [SerializeField]
    private AudioClip _bounceSfx;

    private const float _duration = 1;

    private Transform TargetPosition { get; set; }

    private Vector3 _initial;
    private Rigidbody2D _body;

    private float _time = 0;
    private bool _isFlying = false;
    private bool _isDribling = true;
    private bool _hasBounced = false;

    public void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if (_isFlying)
        {
            _time += Time.deltaTime;
            float t01 = _time / _duration;

            Vector3 a = _initial;
            Vector3 b = TargetPosition.position;

            Vector3 pos = Vector3.Lerp(a, b, t01);
            Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * 3.14f);

            transform.position = pos + arc;

            if (t01 >= 1)
            {
                _body.bodyType = RigidbodyType2D.Dynamic;
                // body.velocity = new Vector2(-1, -1) * 12.5f;
                _body.velocity = new Vector2(1, -1) * 12.5f;
                _isFlying = false;
            }
        }

        if (_isDribling)
        {
            float nose = Mathf.Sin(_time * 5);
            _time += Time.deltaTime;
            Debug.Log(nose);
            Vector3 drible = Vector3.up * Mathf.Abs(nose);
            transform.position = _driblePosition.position + drible;
        }
    }

    public void Launch(Transform target)
    {
        TargetPosition = target;

        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.angularVelocity = 200;
        _initial = transform.position;
        _time = 0;

        _isFlying = true;
        _isDribling = false;
    }

    public void Reset()
    {
        transform.position = _driblePosition.position;
        _body.bodyType = RigidbodyType2D.Kinematic;
        _body.angularVelocity = 0;
        _body.rotation = 0;
        _body.velocity = Vector2.zero;

        _isDribling = true;
    }
}
