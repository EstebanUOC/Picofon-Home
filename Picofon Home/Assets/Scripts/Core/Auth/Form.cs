using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Form : MonoBehaviour
{
    [Header("Basic Info")]
    public InputDNI ChildIDField;
    public Toggle IsPassportToggle;
    public TMP_InputField ChildNameField;
    public TMP_InputField ChildLastNameField;
    public TMP_InputField ChildSchoolField;
    public TMP_Dropdown ChildGradeField;

    [Header("Birthdate")]
    public TMP_InputField BirthDayField;
    public TMP_InputField BirthMonthField;
    public TMP_InputField BirthYearField;

    [Header("Disorder Toggles")]
    public ToggleGroup DisorderToggleGroup;
    public Toggle NoToggle;
    public Toggle TELToggle;
    public Toggle TEAToggle;
    public Toggle TDAHToggle;
    public Toggle OtherToggle;
    public TMP_InputField OtherInput;

    public Button ContinueButton;

    public string ParentId { get; set; } = string.Empty;

    // [Header("Modal")]
    // public Modal modal;

    private bool inputGroupValid = false;
    private bool toogleGroupValid = false;
    private bool birthdateGroupValid = false;
    private bool schoolGroupValid = false;

    public void Start()
    {
        InitComponents();
        ContinueButton.interactable = false;
    }

    public ChildCreateDTO GatherChildData()
    {
        // string id = ChildIDField.text.Trim();
        string firstName = ChildNameField.text.Trim();
        string lastName = ChildLastNameField.text.Trim();

        string year = BirthYearField.text.Trim();
        string month = BirthMonthField.text.Trim().PadLeft(2, '0');
        string day = BirthDayField.text.Trim().PadLeft(2, '0');
        string birthDate = $"{year}-{month}-{day}";

        string school = ChildSchoolField.text.Trim();

        int grade = ChildGradeField.value + 1;

        IEnumerable toggles = DisorderToggleGroup.ActiveToggles();

        string disorder = "No";
        foreach (Toggle toggle in toggles)
        {
            if (toggle.name == "Other")
            {
                disorder = OtherInput != null ? OtherInput.text : string.Empty;
            }
            disorder = toggle.name;
        }

        ChildCreateDTO child = new()
        {
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Disorder = disorder,
            School = school,
            Grade = grade,
            CenterId = 1,
            OwnerId = ParentId,
            // Id = id,
        };

        return child;
    }

    private void InitComponents()
    {
        OtherInput.gameObject.SetActive(false);

        // Listeners for input fields to update button state
        ChildNameField.onValueChanged.AddListener(OnInputChange);
        ChildLastNameField.onValueChanged.AddListener(OnInputChange);
        ChildSchoolField.onValueChanged.AddListener(OnInputChange);

        // Birthdate fields
        BirthDayField.onValueChanged.AddListener(OnInputChange);
        BirthMonthField.onValueChanged.AddListener(OnInputChange);
        BirthYearField.onValueChanged.AddListener(OnInputChange);

        // Grade dropdown
        ChildGradeField.onValueChanged.AddListener(OnDropdownChange);

        NoToggle.onValueChanged.AddListener(OnToggleChange);
        TELToggle.onValueChanged.AddListener(OnToggleChange);
        TEAToggle.onValueChanged.AddListener(OnToggleChange);
        TDAHToggle.onValueChanged.AddListener(OnToggleChange);
        OtherToggle.onValueChanged.AddListener(OnToggleChange);
        OtherToggle.onValueChanged.AddListener(OnToggleOtherChanged);
    }

    private void OnInputChange(string value)
    {
        ValidateAllFields();
    }

    private void OnDropdownChange(int value)
    {
        ValidateAllFields();
    }

    private void OnToggleChange(bool value)
    {
        bool valid = NoToggle.isOn || TELToggle.isOn || TEAToggle.isOn || TDAHToggle.isOn;

        toogleGroupValid = valid;

        if (!valid && OtherToggle.isOn)
        {
            OtherInput.onValueChanged.RemoveAllListeners();
            OtherInput.onValueChanged.AddListener(text =>
            {
                bool otherValid = !string.IsNullOrWhiteSpace(OtherInput.text);
                toogleGroupValid = otherValid;
                UpdateContinueButton();
            });
        }

        UpdateContinueButton();
    }

    private void OnToggleOtherChanged(bool isOn)
    {
        OtherInput.gameObject.SetActive(isOn);
        if (!isOn)
            OtherInput.text = string.Empty;
    }

    private void ValidateAllFields()
    {
        bool nameValid = !string.IsNullOrWhiteSpace(ChildNameField.text);
        bool lastNameValid = !string.IsNullOrWhiteSpace(ChildLastNameField.text);
        inputGroupValid = nameValid && lastNameValid;

        schoolGroupValid = !string.IsNullOrWhiteSpace(ChildSchoolField.text);

        bool dayValid = !string.IsNullOrWhiteSpace(BirthDayField.text);
        bool monthValid = !string.IsNullOrWhiteSpace(BirthMonthField.text);
        bool yearValid = !string.IsNullOrWhiteSpace(BirthYearField.text);
        birthdateGroupValid = dayValid && monthValid && yearValid;

        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        bool allValid =
            inputGroupValid && toogleGroupValid && birthdateGroupValid && schoolGroupValid;
        ContinueButton.interactable = allValid;
    }
}
