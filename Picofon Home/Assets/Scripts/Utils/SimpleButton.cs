namespace Picofon.Utils
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class SimpleButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private bool _interactable = true;

        public event Action OnClick;

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            OnClick?.Invoke();
        }
    }
}
