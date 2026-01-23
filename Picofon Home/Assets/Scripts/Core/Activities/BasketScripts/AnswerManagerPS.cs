using System;
using UnityEngine;

public class AnswerManagerPS : MonoBehaviour
{
    public event Action<int> OnHoopSelected;

    public void Start()
    {
        HoopManager manager = GetComponent<HoopManager>();

        for (int i = 0; i < manager.Hoops.Length; i++)
        {
            int index = i;
            ItemSelectable item = manager
                .Hoops[i]
                .transform.GetChild(0)
                .GetComponent<ItemSelectable>();
            item.OnItemSelected += () => HoopSelected(index);
        }
    }

    private void HoopSelected(int hoopIndex)
    {
        OnHoopSelected?.Invoke(hoopIndex);
    }
}
