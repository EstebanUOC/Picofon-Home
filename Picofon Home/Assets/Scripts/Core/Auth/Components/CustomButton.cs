namespace Picofon.Core.Auth.Components
{
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField]
        private RectTransform _backgroundRect;

        [SerializeField]
        private RectTransform _contentRect;

        [Space]
        [SerializeField]
        private Image _inactiveOverlay;

        public event Action OnClick;

        public bool Interactable
        {
            get => _interactable;
            set => SetInteractable(value);
        }

        private float _defaultContentY;
        private bool _interactable = true;

        public void Awake()
        {
            _defaultContentY = _contentRect.anchoredPosition.y;

            if (_inactiveOverlay != null)
            {
                Color32 inactiveColor = _backgroundRect.gameObject.GetComponent<Image>().color;
                inactiveColor.a = 130;

                _inactiveOverlay.color = inactiveColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            Vector2 contentPos = (_defaultContentY - 11f) * Vector2.up;
            Vector2 bgSize = 11f * Vector2.down;
            Vector2 bgMoveY = 5.5f * Vector2.down;

            _contentRect.anchoredPosition = contentPos;
            _backgroundRect.sizeDelta = bgSize;
            _backgroundRect.anchoredPosition = bgMoveY;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_interactable)
                return;

            Vector2 contentPos = _contentRect.anchoredPosition;
            contentPos.y = _defaultContentY;

            _contentRect.anchoredPosition = contentPos;
            _backgroundRect.sizeDelta = Vector2.zero;
            _backgroundRect.anchoredPosition = Vector2.zero;

            OnClick?.Invoke();
        }

        private void SetInteractable(bool value)
        {
            if (_interactable == value)
                return;

            _interactable = value;

            _inactiveOverlay.gameObject.SetActive(!_interactable);
        }
    }
}
