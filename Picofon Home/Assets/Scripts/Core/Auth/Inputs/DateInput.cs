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
    private Color colorInfo;

    public void Start()
    {
        _defaultColorBlock = DayInput.colors;
        colorInfo = InfoText.color;

        DayInput.onEndEdit.AddListener(OnDayInputChange);
        MonthInput.onEndEdit.AddListener(OnMonthInputChange);
        YearInput.onEndEdit.AddListener(OnYearInputChange);
    }

    private void OnDayInputChange(string input)
    {
        if (!int.TryParse(input, out int day))
            return;

        if (day <= 0)
            day = 1;

        if (day < 10)
            DayInput.text = day.ToString().PadLeft(2, '0');
        else if (day > 31)
            DayInput.text = "31";
    }

    private void OnMonthInputChange(string input)
    {
        if (!int.TryParse(input, out int month))
            return;

        if (month <= 0)
            month = 1;

        if (month < 10)
            MonthInput.text = month.ToString().PadLeft(2, '0');
        else if (month > 12)
            MonthInput.text = "12";
    }

    private void OnYearInputChange(string input)
    {
        if (!int.TryParse(input, out int year))
            return;

        int currentYear = System.DateTime.Now.Year;
        int age = currentYear - year;

        if (year < 2000)
            YearInput.text = "2000";
        else if (year > currentYear)
            YearInput.text = currentYear.ToString();

        Debug.Log("Date: " + GetData());
        // ValidateInput(GetData());
    }

    protected override void OnError() { }

    protected override void OnReset() { }

    protected override void OnValid() { }

    protected override void ValidateInput(string input)
    {
        string[] parts = input.Split('-');
        if (parts.Length != 3)
        {
            Valid = false;
            return;
        }

        if (
            !int.TryParse(parts[0], out int year)
            || !int.TryParse(parts[1], out int month)
            || !int.TryParse(parts[2], out int day)
        )
        {
            Valid = false;
            return;
        }

        Valid = System.DateTime.TryParse($"{year}-{month}-{day}", out _);
    }

    public override string GetData()
    {
        string day = DayInput.text;
        string month = MonthInput.text;
        string year = YearInput.text;

        return $"{year}-{month}-{day}";
    }
}
