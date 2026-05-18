using System;
using System.Globalization;
using TMPro;
using UnityEngine;

public class PersonalContent : MonoBehaviour
{
    [SerializeField]
    private RectTransform _panel;

    [SerializeField]
    private UIManager _uiManager;

    [SerializeField]
    private TMP_InputField _idInput;

    [SerializeField]
    private TMP_InputField _nameInput;

    [SerializeField]
    private TMP_InputField _lastNameInput;

    [SerializeField]
    private TMP_InputField _ageInput;

    [SerializeField]
    private CustomButton _submit;

    public event Action OnValidationSuccess;

    private bool _isSpain;

    public void Start()
    {
        _submit.Interactable = false;
        _submit.OnClick += OnSubmit;

        _ageInput.onValidateInput = ValidateDateChar;

        _idInput.onSelect.AddListener(OnInputSelected);
        _nameInput.onSelect.AddListener(OnInputSelected);
        _lastNameInput.onSelect.AddListener(OnInputSelected);
        _ageInput.onSelect.AddListener(OnInputSelected);

        _idInput.onEndEdit.AddListener(ValidateInputs);
        _nameInput.onEndEdit.AddListener(ValidateInputs);
        _lastNameInput.onEndEdit.AddListener(ValidateInputs);
        _ageInput.onEndEdit.AddListener(ValidateInputs);

        OnEnable();
    }

    public void SetUpdateData(ChildDataDTO data)
    {
        _idInput.text = data.Id;
        _nameInput.text = data.FirstName;
        _lastNameInput.text = data.LastName;
        _ageInput.text = DateTime.Parse(data.BirthDate).ToString("dd/MM/yyyy");
    }

    public void OnEnable()
    {
        if (RegisterChild.IsUpdate)
        {
            _idInput.interactable = false;
            _submit.Interactable = true;
        }
    }

    public void OnDisable()
    {
        _idInput.text = string.Empty;
        _nameInput.text = string.Empty;
        _lastNameInput.text = string.Empty;
        _ageInput.text = string.Empty;
    }

    public void SetIsSpain(bool isSpain)
    {
        _isSpain = isSpain;
    }

    public bool CanSubmit()
    {
        return _submit.Interactable;
    }

    public void SetData(CreateChildDTO data)
    {
        data.Id = _idInput.text;
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
        if (
            _idInput.text.Length < 9
            || _ageInput.text.Length < 10
            || _nameInput.text.Length < 3
            || _lastNameInput.text.Length < 3
        )
        {
            _submit.Interactable = false;
            return;
        }

        _submit.Interactable = true;
    }

    private void OnSubmit()
    {
        if (!ValidateIdentification(_idInput.text))
        {
            _ = _uiManager.ShowModal(
                new ModalData
                {
                    Title = "Error",
                    Message =
                        "La identificación ingresada no es válida. Por favor, verifica los datos e intenta nuevamente.",
                    Panel = _panel,
                }
            );

            return;
        }

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

    private bool ValidateIdentification(string id)
    {
        if (_isSpain)
        {
            return ValidateDNI(id) || ValidateNIE(id) || ValidatePassport(id);
        }

        return !string.IsNullOrEmpty(id) && id.Length >= 5 && id.Length <= 10;
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

    private bool ValidatePassport(string passport)
    {
        return !string.IsNullOrEmpty(passport) && passport.Length >= 9;
    }

    private bool ValidateDNI(string dni)
    {
        if (string.IsNullOrEmpty(dni) || dni.Length != 9)
            return false;

        string numbers = dni[..8];

        if (!int.TryParse(numbers, out int _))
        {
            return false;
        }

        char letter = dni[8];

        return char.IsLetter(letter);
    }

    private bool ValidateNIE(string nie)
    {
        if (string.IsNullOrEmpty(nie) || nie.Length != 9)
            return false;

        bool endsWithLetter = char.IsLetter(nie[^1]);

        if (!endsWithLetter)
        {
            return false;
        }

        bool someChar = false;

        for (int i = 1; i < nie.Length - 1; i++)
        {
            if (char.IsLetter(nie[i]))
            {
                someChar = true;
                break;
            }
        }

        return !someChar;
    }
}
