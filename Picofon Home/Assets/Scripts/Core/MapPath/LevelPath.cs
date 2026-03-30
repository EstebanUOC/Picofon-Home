using System;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.UI;

public class LevelPath : MonoBehaviour
{
    [SerializeField]
    private GameObject _scroll;

    [SerializeField]
    private GameObject _content;

    private float _offset;

    private const int CountPerPoint = 5;

    public void Awake()
    {
        ScrollRect scrollRect = _scroll.GetComponent<ScrollRect>();
        scrollRect.onValueChanged.AddListener(OnScroll);

        _offset = _content.transform.position.x * -1;
    }

    public void ChangePath(Span<Vector2> points)
    {
        SplineComputer spline = GetComponent<SplineComputer>();

        if (spline.pointCount > points.Length)
        {
            Resize(spline, points.Length);
            return;
        }

        AddRange(spline, points);
    }

    private void Resize(SplineComputer spline, int length)
    {
        SplinePoint[] points = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[length];

        ObjectController objectController = GetComponent<ObjectController>();

        objectController.spawnCount = length * CountPerPoint;

        for (int i = 0; i < points.Length; i++)
        {
            if (i < length)
            {
                newPoints[i] = points[i];
            }
        }

        spline.SetPoints(newPoints);
    }

    private void AddRange(SplineComputer spline, Span<Vector2> points)
    {
        if (spline.pointCount == points.Length)
            return;

        SplinePoint[] oldPoints = spline.GetPoints();
        SplinePoint[] newPoints = new SplinePoint[points.Length];

        ObjectController objectController = GetComponent<ObjectController>();

        objectController.spawnCount = points.Length * CountPerPoint;

        for (int i = 0; i < points.Length; i++)
        {
            if (i < spline.pointCount)
            {
                newPoints[i] = oldPoints[i];
                continue;
            }

            newPoints[i] = new SplinePoint(points[i]);
        }

        spline.SetPoints(newPoints);
    }

    private void OnScroll(Vector2 value)
    {
        Vector2 algo = new(x: _content.transform.position.x + _offset, y: 0);

        transform.position = algo;
    }
}
