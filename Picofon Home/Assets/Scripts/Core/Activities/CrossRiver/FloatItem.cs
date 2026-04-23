using UnityEngine;

public class FloatItem : MonoBehaviour
{
    private bool _isFloating = false;

    private float _startY;

    private float _time;

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

        _time += Time.deltaTime;

        float offset = Mathf.Sin(_time * _speed) * _amplitude;

        transform.position = new Vector3(
            transform.position.x,
            _startY + offset,
            transform.position.z
        );
    }

    public void SetFloating(bool isFloating)
    {
        _isFloating = isFloating;

        _time = Mathf.PI;
    }
}
