using UnityEngine;

public class FloatItem : MonoBehaviour
{
    [SerializeField]
    private bool _isFloating = true;

    private float _startY;

    private const float _speed = 5f;
    private const float _amplitude = 0.05f;

    public void Start()
    {
        _startY = transform.position.y;
    }

    public void FixedUpdate()
    {
        if (!_isFloating)
            return;

        float offset = Mathf.Sin(Time.time * _speed) * _amplitude;

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );
    }
}
