namespace Picofon.Activities.CrossRiver
{
    using System;
    using UnityEngine;

    public class FloatSelectable : MonoBehaviour
    {
        [SerializeField]
        private bool _interactable = true;

        public event Action OnClick;

        public bool Interactable
        {
            get => _interactable;
        }

        public void OnMouseDown()
        {
            OnClick?.Invoke();
        }
    }
}
