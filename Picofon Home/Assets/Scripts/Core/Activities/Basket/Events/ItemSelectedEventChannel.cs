namespace Picofon.Activities.Basket.Events
{
    using System;

    public class ItemSelectedEventChannel
    {
        public event Action<int> OnItemSelected;

        public void Raise(int index)
        {
            OnItemSelected?.Invoke(index);
        }
    }
}
