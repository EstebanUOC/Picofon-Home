using Picofon.Core.Auth.Components;
using Picofon.Core.Auth.DTOs;
using Picofon.Core.Auth.Panels;
using Picofon.Utils;

namespace Picofon.Core.Auth
{
    using System;
    using System.Globalization;
    using TMPro;
    using UnityEngine;

    public class PersonalContent : MonoBehaviour
    {
        # region References

        [SerializeField]
        private RectTransform _panel;

        [SerializeField]
        private UIManager _uiManager;

        [SerializeField]
        private TMP_InputField _nameInput;

        [SerializeField]
        private TMP_InputField _lastNameInput;

        [SerializeField]
        private TMP_InputField _ageInput;

        [SerializeField]
        private CustomButton _submit;

        [SerializeField]
        private SimpleButton _advice;

        [SerializeField]
        private GameObject _advicePanel;

        #endregion

        public event Action OnValidationSuccess;

        public void Start()
        {
            _submit.Interactable = false;
            _submit.OnClick += OnSubmit;

            _advicePanel.SetActive(false);

            _advice.OnClick += () =>
            {
                _advicePanel.transform.position = _advice.transform.position;

                RectTransform panelRect = _advicePanel.GetComponent<RectTransform>();

                panelRect.anchoredPosition += new Vector2(-24, 0);

                _advicePanel.SetActive(!_advicePanel.activeSelf);
            };

            _ageInput.onValidateInput = ValidateDateChar;

            _nameInput.onSelect.AddListener(OnInputSelected);
            _lastNameInput.onSelect.AddListener(OnInputSelected);
            _ageInput.onSelect.AddListener(OnInputSelected);

            _nameInput.onEndEdit.AddListener(ValidateInputs);
            _lastNameInput.onEndEdit.AddListener(ValidateInputs);
            _ageInput.onEndEdit.AddListener(ValidateInputs);

            OnEnable();
        }

        public void SetUpdateData(ChildDataDTO data)
        {
            _nameInput.text = data.FirstName;
            _lastNameInput.text = data.LastName;
            _ageInput.text = DateTime.Parse(data.BirthDate).ToString("dd/MM/yyyy");
        }

        public void OnEnable()
        {
            if (RegisterChild.IsUpdate)
            {
                _submit.Interactable = true;
            }
        }

        public void OnDisable()
        {
            _nameInput.text = string.Empty;
            _lastNameInput.text = string.Empty;
            _ageInput.text = string.Empty;
        }

        public bool CanSubmit()
        {
            return _submit.Interactable;
        }

        public void SetData(CreateChildDTO data)
        {
            data.FirstName = _nameInput.text;
            data.LastName = _lastNameInput.text;

            string formattedDate =
                $"{_ageInput.text.Substring(6, 4)}-{_ageInput.text.Substring(3, 2)}-{_ageInput.text[..2]}";

            data.BirthDate = formattedDate;
        }

        private void OnInputSelected(string _)
        {
            _submit.Interactable = false;
        }

        private void ValidateInputs(string _)
        {
            if (_ageInput.text.Length < 10 || _nameInput.text.Length < 3)
            {
                _submit.Interactable = false;
                return;
            }

            _submit.Interactable = true;
        }

        private void OnSubmit()
        {
            if (!ValidateDate(_ageInput.text))
            {
                _ = _uiManager.ShowModal(
                    new ModalData
                    {
                        Title = "Error",
                        Message =
                            "La fecha de nacimiento ingresada no es válida. El niño debe tener entre 3 y 7 años. Por favor, verifica los datos e intenta nuevamente.",
                        Panel = _panel,
                    }
                );

                return;
            }

            OnValidationSuccess?.Invoke();
        }

        private bool ValidateDate(string date)
        {
            if (
                !DateTime.TryParseExact(
                    date,
                    "dd/MM/yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime birthDate
                )
            )
            {
                PerformanceLog.Log("Fecha de nacimiento no tiene el formato correcto.");
                return false;
            }

            DateTime today = DateTime.Today;

            int age = today.Year - birthDate.Year;

            if (birthDate > today.AddYears(-age))
                age--;

            return age >= 3 && age <= 7;
        }

        private char ValidateDateChar(string text, int charIndex, char addedChar)
        {
            if (!char.IsDigit(addedChar) && addedChar != '/')
                return '\0';

            if (charIndex == 2 || charIndex == 5)
            {
                if (addedChar != '/')
                    return '\0';
            }
            else
            {
                if (!char.IsDigit(addedChar))
                    return '\0';
            }

            if (text.Length >= 10)
                return '\0';

            return addedChar;
        }
    }
}
