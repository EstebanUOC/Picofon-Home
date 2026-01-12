using BasketResponses;
using UnityEngine;

public class HoopManager : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private GameObject[] _hoops;

    public GameObject[] Hoops => _hoops;

    public void SetHoopStates(in AnswerDTO data)
    {
        if (data.Answers.Length != _hoops.Length)
        {
            Debug.LogError("Hoop count does not match answer count.");
            return;
        }

        for (int i = 0; i < _hoops.Length; i++)
        {
            Hoop hoop = _hoops[i].GetComponent<Hoop>();
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

        Hoop hoop = _hoops[index].GetComponent<Hoop>();
        return hoop.TargetPosition;
    }
}
