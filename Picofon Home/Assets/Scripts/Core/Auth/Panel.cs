namespace Picofon.Core.Auth
{
    using System;
    using UnityEngine;

    public class Panel : MonoBehaviour
    {
        public Action OnShow;
        public Action OnHide;

        public virtual void Show()
        {
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }

        public virtual void Hide()
        {
            OnHide?.Invoke();
        }
    }
}
