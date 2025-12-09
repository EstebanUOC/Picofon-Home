using UnityEngine;

public enum HoopType
{
    Positive,
    Negative,
}

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    [Space(15)]
    public BallTest Ball;

    [Space(15)]
    public Hoop HoopPositive;
    public Hoop HoopNegative;

    public void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public void LaunchBall(HoopType hoopType)
    {
        switch (hoopType)
        {
            case HoopType.Positive:
                Ball.TargetPosition = HoopPositive.BallTarget;
                break;
            case HoopType.Negative:
                Ball.TargetPosition = HoopNegative.BallTarget;
                break;
        }
        Ball.Launch();
    }
}
