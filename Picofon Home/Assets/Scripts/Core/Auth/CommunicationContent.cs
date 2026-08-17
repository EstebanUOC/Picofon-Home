using Picofon.Core.Auth.Components;
using Picofon.Core.Auth.DTOs;

namespace Picofon.Core.Auth
{
    using System;
    using TMPro;
    using UnityEngine;

    public class CommunicationContent : MonoBehaviour
    {
        [SerializeField]
        private CustomButtonLoading _submit;

        [SerializeField]
        private CustomToggleGroup _languageToggleGroup;

        [SerializeField]
        private CustomToggleGroup _disorderToggleGroup;

        [SerializeField]
        private CustomToggle _otherDisorderToggle;

        [SerializeField]
        private TMP_InputField _otherDisorderInput;

        public event Action OnValidationSuccess;

        public void Start()
        {
            _submit.OnClick += OnSubmit;

            _otherDisorderToggle.OnToggle += OnOtherDisorderToggled;
        }

        public void EndLoading()
        {
            _submit.EndLoading();
        }

        private void OnOtherDisorderToggled(bool isOn)
        {
            if (isOn)
            {
                _submit.Interactable = _otherDisorderInput.text.Length > 2;
                _otherDisorderInput.onEndEdit.AddListener(OnEndEditInput);
                _otherDisorderInput.onSelect.AddListener(OnSelectInput);
                return;
            }

            _submit.Interactable = true;
            _otherDisorderInput.onEndEdit.RemoveListener(OnEndEditInput);
            _otherDisorderInput.onSelect.RemoveListener(OnSelectInput);
        }

        private void OnSelectInput(string _)
        {
            _submit.Interactable = false;
        }

        private void OnEndEditInput(string text)
        {
            _submit.Interactable = text.Length > 2;
        }

        private void OnSubmit()
        {
            OnValidationSuccess?.Invoke();
        }

        internal void SetData(CreateChildDTO childData)
        {
            childData.LanguagePreference = _languageToggleGroup.GetSelectedIndex();

            if (_otherDisorderToggle.IsSelected)
            {
                childData.Disorder = _otherDisorderInput.text;
                return;
            }

            string disorder = _disorderToggleGroup.GetSelectedIndex() switch
            {
                0 => "No",
                1 => "Trastorno Especifico del Lenguaje (TEL)",
                2 => "Trastorno del Espectro Autista (TEA)",
                3 => "Trastorno por Déficit de Atención e Hiperactividad (TDAH)",
                _ => null,
            };

            childData.Disorder = disorder;
        }
    }
}
