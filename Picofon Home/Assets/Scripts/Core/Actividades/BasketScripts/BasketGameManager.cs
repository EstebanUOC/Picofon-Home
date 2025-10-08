using UnityEngine;
using System.Collections.Generic;

public class BasketGameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform ballContainer;
    public Transform hoopTarget;
    public int ballCount = 4; // Default number of balls

    private List<GameObject> activeBalls = new List<GameObject>();

    void Start()
    {
        SpawnBalls();
    }

    public void SpawnBalls()
    {
        ClearBalls();

        float startX = -400f;
        float spacing = 250;
        float yPos = 300f;

        for (int i = 0; i < ballCount; i++)
        {
            Vector3 position = new Vector3(startX + i * spacing, yPos, 0f);
            GameObject ball = Instantiate(ballPrefab, ballContainer);
            ball.GetComponent<RectTransform>().anchoredPosition = position;
            ball.GetComponent<BallController>().Initialize(hoopTarget);
            activeBalls.Add(ball);
        }
    }

    public void ClearBalls()
    {
        foreach (var b in activeBalls)
            Destroy(b);
        activeBalls.Clear();
    }

    public void SetBallCount(int count)
    {
        ballCount = count;
        SpawnBalls();
    }
}
