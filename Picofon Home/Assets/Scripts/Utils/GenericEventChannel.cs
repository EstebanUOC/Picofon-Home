namespace Picofon.Utils
{
    using System;

    public class GenericEventChannel<T>
    {
        public event Action<T> OnRaised;

        public void Raise(T item)
        {
            OnRaised?.Invoke(item);
        }
    }

    public class GenericEventChannel
    {
        public event Action OnRaised;

        public void Raise()
        {
            OnRaised?.Invoke();
        }
    }
}
