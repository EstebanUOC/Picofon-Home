using UnityEngine;

public class BallController : MonoBehaviour
{
    [Space(15)]
    public Transform PositiveTarget;
    public Transform NegativeTarget;

    private BallMovement BallMovement;

    public void Start()
    {
        BallMovement = GetComponent<BallMovement>();

        BasketManager.Instance.OnAnswerSelected += LaunchBall;
        BasketManager.Instance.OnActivityChange += Reset;
    }

    private void LaunchBall(HoopType hoopType)
    {
        Transform target = hoopType == HoopType.Positive ? PositiveTarget : NegativeTarget;
        BallMovement.Launch(target);
    }

    private void Reset(in BasketResponses.BasketActivity _)
    {
        BallMovement.Reset();
    }
}
