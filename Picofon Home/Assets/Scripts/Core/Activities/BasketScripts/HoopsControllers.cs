using UnityEngine;

public class HoopsControllers : MonoBehaviour
{
    [Space(15)]
    public Hoop HoopPositive;
    public Hoop HoopNegative;

    public void Start()
    {
        BasketManager.Instance.OnActivityChange += UpdateHoops;
    }

    public Transform GetHoopTarget(HoopType hoopType)
    {
        return hoopType == HoopType.Positive
            ? HoopPositive.TargetPosition
            : HoopNegative.TargetPosition;
    }

    private void UpdateHoops(in BasketResponses.BasketActivity activity)
    {
        HoopPositive.Blocked = !activity.Answer;
        HoopNegative.Blocked = activity.Answer;
    }
}
