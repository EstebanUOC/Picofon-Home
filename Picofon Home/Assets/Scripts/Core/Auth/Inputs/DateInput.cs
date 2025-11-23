using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DateInput : FormInput
{
    [Space(15)]
    public TMP_InputField DayInput;
    public TMP_InputField MonthInput;
    public TMP_InputField YearInput;

    [Space(15)]
    public TMP_Text InfoText;

    private ColorBlock _defaultColorBlock;
    private Color _colorImage;
    private Color _colorInfo;

    private const int MinDay = 1;
    private const int MaxDay = 31;
    private const int MinMonth = 1;
    private const int MaxMonth = 12;
    private const int MinYear = 2000;

    public void Start()
    {
        _defaultColorBlock = DayInput.colors;
        _colorImage = DayInput.image.color;
        _colorInfo = InfoText.color;

        DayInput.onEndEdit.AddListener(input =>
            ValidateAndFormatInput(DayInput, MinDay, MaxDay, input)
        );
        MonthInput.onEndEdit.AddListener(input =>
            ValidateAndFormatInput(MonthInput, MinMonth, MaxMonth, input)
        );
        YearInput.onEndEdit.AddListener(OnYearInputChange);
    }

    private void ValidateAndFormatInput(TMP_InputField inputField, int min, int max, string input)
    {
        if (!int.TryParse(input, out int value))
        {
            Valid = false;
            Error = false;
            return;
        }

        value = Mathf.Clamp(value, min, max);
        inputField.text = value.ToString("D2");
        ValidateInput();
    }

    private void OnYearInputChange(string input)
    {
        if (!int.TryParse(input, out int year))
        {
            Valid = false;
            Error = false;
            return;
        }

        int currentYear = System.DateTime.Now.Year;
        year = Mathf.Clamp(year, MinYear, currentYear);
        YearInput.text = year.ToString();
        ValidateInput();
    }

    protected override void OnError()
    {
        _defaultColorBlock.selectedColor = _errorColor;
        UpdateInputColors(_defaultColorBlock, _errorColor);
        UpdateInfoContent("Data invàlida. Si us plau, comprova els valors.", _errorColor);
    }

    protected override void OnReset()
    {
        _defaultColorBlock.selectedColor = _defaultColor;
        UpdateInputColors(_defaultColorBlock, _colorImage);
        UpdateInfoContent("Introdueix la teva data de naixement.", _colorInfo);
    }

    protected override void OnValid()
    {
        _defaultColorBlock.selectedColor = _validColor;
        UpdateInputColors(_defaultColorBlock, _validColor);
        UpdateInfoContent("Introdueix la teva data de naixement.", _colorInfo);
    }

    private void UpdateInfoContent(string message, Color color)
    {
        InfoText.text = message;
        InfoText.color = color;
    }

    private void UpdateInputColors(ColorBlock colorBlock, Color imageColor)
    {
        DayInput.colors = colorBlock;
        MonthInput.colors = colorBlock;
        YearInput.colors = colorBlock;

        DayInput.image.color = imageColor;
        MonthInput.image.color = imageColor;
        YearInput.image.color = imageColor;
    }

    protected override void ValidateInput()
    {
        bool allFieldsFilled =
            !string.IsNullOrEmpty(DayInput.text)
            && !string.IsNullOrEmpty(MonthInput.text)
            && !string.IsNullOrEmpty(YearInput.text);

        if (!allFieldsFilled)
        {
            Valid = false;
            return;
        }

        string data = GetData();

        bool isValid = System.DateTime.TryParse(data, out _);

        if (isValid)
            Valid = true;
        else
            Error = true;
    }

    public override string GetData()
    {
        string day = DayInput.text;
        string month = MonthInput.text;
        string year = YearInput.text;

        return $"{year}-{month}-{day}";
    }
}
