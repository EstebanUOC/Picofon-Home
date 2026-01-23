using System;
using DG.Tweening;
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

    public void Prueba()
    {
        transform.DOLocalMoveY(-1.4f, 0.5f);
    }

    private void HoopSelected(int hoopIndex)
    {
        OnHoopSelected?.Invoke(hoopIndex);
    }
}
