using UnityEngine;

public class BasketManager : MonoBehaviour
{
    [Space(15)]
    public Hoop HoopPositive;
    public Hoop HoopNegative;

    [Space(15)]
    public BallTest Ball;

    public enum HoopType
    {
        Positive,
        Negative,
    }

    public static BasketManager Instance;

    public void Awake()
    {
        if (Instance != this)
            Destroy(gameObject);

        Instance = this;

        ChooseHoop(HoopType.Negative);
    }

    public void ChooseHoop(HoopType hoopType)
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
    }
}
