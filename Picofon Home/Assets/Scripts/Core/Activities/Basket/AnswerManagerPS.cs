using Picofon.Activities.Basket.Events;

namespace Picofon.Activities.Basket
{
    using System;
    using PrimeTween;
    using UnityEngine;

    public class AnswerManagerPS : MonoBehaviour
    {
        public event Action<int> OnHoopSelected;

        private ItemSelectedEventChannel _itemSelectedEventChannel;

        public void Start()
        {
            _itemSelectedEventChannel = new ItemSelectedEventChannel();

            HoopManager manager = GetComponent<HoopManager>();

            for (int i = 0; i < manager.Hoops.Length; i++)
            {
                ItemSelectable item = manager
                    .Hoops[i]
                    .transform.GetChild(0)
                    .GetComponent<ItemSelectable>();
                item.ItemSelectedEventChannel = _itemSelectedEventChannel;
                item.ItemIndex = i;
            }

            _itemSelectedEventChannel.OnItemSelected += HoopSelected;
        }

        public void Prueba()
        {
            Tween.LocalPositionY(transform, -1.4f, 0.5f);
        }

        private void HoopSelected(int hoopIndex)
        {
            OnHoopSelected?.Invoke(hoopIndex);
        }
    }
}
