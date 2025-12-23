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

    private void UpdateHoops(in BasketResponses.BasketActivity activity)
    {
        HoopPositive.Blocked = !activity.Answer;
        HoopNegative.Blocked = activity.Answer;
    }
}
