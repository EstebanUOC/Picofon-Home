using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IdInput : BasicInput
{
    public TMP_Text Label;
    public TMP_Text Placeholder;
    public TMP_Text InfoText;
    public Toggle PassportToggle;

    private readonly int DNILimit = 9;
    private readonly int passportLimit = 12;

    private Color colorInfo;
    private string errorMessage;

    public override void Start()
    {
        base.Start();
        PassportToggle.onValueChanged.AddListener(OnToggleChange);
        colorInfo = InfoText.color;
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
        Error = false;
        Valid = false;
        OnInputChange(Input.text);
    }

    protected override void OnInputChange(string input)
    {
        Input.text = input.ToUpper();
        base.OnInputChange(input);
    }

    protected override void OnValid()
    {
        base.OnValid();
        Input.image.color = _validColor;
    }

    protected override void OnError()
    {
        base.OnError();
        InfoText.text = errorMessage;
        InfoText.color = _errorColor;
    }

    protected override void OnReset()
    {
        base.OnReset();
        InfoText.text = PassportToggle.isOn
            ? "Passaport: 3 lletres seguides de 6 números"
            : "DNI: 8 dígits + lletra | NIE: X/Y/Z + 7 dígits + lletra";
        InfoText.color = colorInfo;
    }

    protected override void ValidateInput(string input)
    {
        if (PassportToggle.isOn)
            ValidatePassport(input);
        else
            ValidateLegalId(input);
    }

    private void ValidatePassport(string passport)
    {
        Valid = !string.IsNullOrEmpty(passport) && passport.Length >= passportLimit;
    }

    private void ValidateLegalId(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < DNILimit)
        {
            Error = false;
            Valid = false;
            return;
        }

        bool isDNI = char.IsDigit(input[0]);
        bool isValid = isDNI ? ValidateDNI(input) : ValidateNIE(input);

        if (isValid)
        {
            Valid = true;
            return;
        }

        errorMessage = isDNI ? "DNI invàlid." : "NIE invàlid.";
        Error = true;
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
