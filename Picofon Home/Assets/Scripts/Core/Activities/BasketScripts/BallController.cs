using UnityEngine;

public class BallController : MonoBehaviour
{
    private BallMovement _movement;

    public void Awake()
    {
        _movement = GetComponent<BallMovement>();
    }

    public void LaunchBall(Transform target)
    {
        _movement.Launch(target);
    }

    public void Reset(in BasketResponses.BasketActivity _)
    {
        _movement.Reset();
    }
}
