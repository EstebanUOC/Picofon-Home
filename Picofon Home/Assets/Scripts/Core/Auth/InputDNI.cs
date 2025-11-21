using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputDNI : InputField
{
    public TMP_Text Label;
    public TMP_Text Placeholder;
    public TMP_Text InfoText;
    public Toggle IsPassportToggle;

    private readonly int DNILimit = 9;
    private readonly int passportLimit = 12;

    public override void Start()
    {
        base.Start();
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

        Input.characterLimit = isOn ? passportLimit : DNILimit;
    }

    protected override void OnInputChange(string input)
    {
        Input.text = input.ToUpper();
        base.OnInputChange(input);
    }

    protected override void ValidateInput(string input)
    {
        ValidateChildId(input);
    }

    private void ValidateChildId(string input)
    {
        if (IsPassportToggle.isOn)
            ValidatePassport(input);
        else
            ValidateLegalId(input);
    }

    private void ValidateLegalId(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < DNILimit)
        {
            _error = false;
            _valid = false;
            return;
        }

        bool isDNI = char.IsDigit(input[0]);
        _valid = isDNI ? ValidateDNI(input) : ValidateNIE(input);

        if (!_valid)
        {
            InfoText.text =
                "Format invàlid. DNI: 8 dígits + lletra, NIE: X/Y/Z + 7 dígits + lletra";
            InfoText.color = _errorColor;
            _error = true;
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
    }

    private bool ValidateDNI(string dni)
    {
        if (string.IsNullOrEmpty(dni) || dni.Length != 9)
            return false;

        string numbers = dni[..8];

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
