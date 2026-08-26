namespace Picofon.Utils
{
    using System;
    using UnityEngine;

    public class SimplePhysicalButton : MonoBehaviour
    {
        #region References

        [SerializeField]
        private bool _interactable = true;

        #endregion

        public event Action OnClick;

        public bool Interactable
        {
            get => _interactable;
            set => _interactable = value;
        }

        public void OnMouseDown()
        {
            if (!_interactable)
                return;

            OnClick?.Invoke();
        }
    }
}
