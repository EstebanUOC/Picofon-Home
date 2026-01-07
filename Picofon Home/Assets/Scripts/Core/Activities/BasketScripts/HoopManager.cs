using BasketResponses;
using UnityEngine;

public class HoopManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private Hoop[] _hoops;

    public void SetHoopStates(in AnswerDTO data)
    {
        if (data.Answers.Length != _hoops.Length)
        {
            Debug.LogError("Hoop count does not match answer count.");
            return;
        }

        for (int i = 0; i < _hoops.Length; i++)
        {
            Hoop hoop = _hoops[i];
            hoop.Blocked = !data.Answers[i];
        }
    }

    public Transform GetHoopTransform(int index)
    {
        if (index < 0 || index >= _hoops.Length)
        {
            Debug.LogError("Invalid hoop index.");
            return null;
        }

        return _hoops[index].TargetPosition;
    }
}
