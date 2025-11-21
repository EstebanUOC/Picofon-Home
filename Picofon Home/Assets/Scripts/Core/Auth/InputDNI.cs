using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputDNI : MonoBehaviour
{
    public TMP_Text Label;
    public TMP_InputField Input;
    public TMP_Text Placeholder;
    public TMP_Text InfoText;
    public Toggle IsPassportToggle;

    private bool _valid = false;
    public bool Valid
    {
        get { return _valid; }
    }

    public void Start()
    {
        Input.onValueChanged.AddListener(OnInputChange);

        IsPassportToggle.onValueChanged.AddListener(OnToggleChange);
    }

    private void OnToggleChange(bool isOn)
    {
        string label = isOn ? "Passaport" : "DNI / NIE";
        string placeholder = isOn ? "ABC123456" : "12345678A / X1234567A";
        string infoMessage = isOn
            ? "Passaport: 3 lletres seguides de 6 números"
            : "DNI: 8 dígits + lletra | NIE: X/Y/Z + 7 dígits + lletra";

        Label.text = label;
        Placeholder.text = placeholder;
        InfoText.text = infoMessage;

        Input.characterLimit = isOn ? 12 : 9;
    }

    private void OnInputChange(string input)
    {
        Input.text = input.ToUpper();
        ValidateChildId(input);
    }

    private void ValidateChildId(string input)
    {
        if (IsPassportToggle.isOn)
        {
            ValidatePassport(input);
            return;
        }

        if (string.IsNullOrEmpty(input))
        {
            _valid = false;
            return;
        }

        char firstChar = input.ToUpper()[0];
        if (char.IsDigit(firstChar))
        {
            _valid = ValidateDNI(input);
        }
        else if (firstChar == 'X' || firstChar == 'Y' || firstChar == 'Z')
        {
            _valid = ValidateNIE(input);
        }
        else
        {
            _valid = false;
        }
    }

    private void ValidatePassport(string passport)
    {
        if (string.IsNullOrEmpty(passport))
        {
            _valid = false;
            return;
        }

        _valid = passport.Length >= 5 && passport.Length <= 12;

        if (_valid)
        {
            Input.image.color = Color.green;
        }
    }

    private bool ValidateDNI(string dni)
    {
        if (string.IsNullOrEmpty(dni) || dni.Length != 9)
            return false;

        string numbers = dni.Substring(0, 8);

        if (!int.TryParse(numbers, out int dniNumber))
            return false;

        char letter = dni.ToUpper()[8];
        char calculatedLetter = CalculateControlLetter(dniNumber);

        return letter == calculatedLetter;
    }

    private bool ValidateNIE(string nie)
    {
        if (string.IsNullOrEmpty(nie) || nie.Length != 9)
            return false;

        char firstChar = nie.ToUpper()[0];

        if (firstChar != 'X' && firstChar != 'Y' && firstChar != 'Z')
            return false;

        int firstDigit =
            firstChar == 'X' ? 0
            : firstChar == 'Y' ? 1
            : 2;

        string middleNumbers = nie.Substring(1, 7);

        if (!int.TryParse(middleNumbers, out int nieNumbers))
            return false;

        string fullNumber = firstDigit.ToString() + middleNumbers;
        int nieNumber = int.Parse(fullNumber);

        char letter = nie.ToUpper()[8];
        char calculatedLetter = CalculateControlLetter(nieNumber);

        return letter == calculatedLetter;
    }

    private char CalculateControlLetter(int number)
    {
        string letters = "TRWAGMYFPDXBNJZSQVHLCKE";

        int remainder = number % 23;

        return letters[remainder];
    }
}
