using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UI;

public class LevelPath : MonoBehaviour
{
    [SerializeField]
    private GameObject _scroll;

    [SerializeField]
    private GameObject _content;

    private float _offset;

    public void Awake()
    {
        ScrollRect scrollRect = _scroll.GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScroll);

        _offset = _content.transform.position.x * -1;
    }

    public void ChangePath(Span<Vector2> points)
    {
        SplineContainer _container = GetComponent<SplineContainer>();
        var spline = _container.Spline;
        spline.Clear();

        if (spline.Count > points.Length)
        {
            spline.Resize(points.Length);
            return;
        }

        float3[] positions = new float3[points.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            Vector2 point = points[i];
            positions[i] = new float3(point.x, point.y, 0f);
        }

        spline.AddRange(positions);
    }

    private void OnScroll(Vector2 value)
    {
        Vector2 algo = new(x: _content.transform.position.x + _offset, y: 0);

        transform.position = algo;
        Debug.Log($"transform position: {transform.position}");
    }
}
