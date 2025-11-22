using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ChildFields
{
    ID,
    FirstName,
    LastName,
    BirthDate,
    Disorder,
    School,
    Grade,
}

public class Form : MonoBehaviour
{
    // public Modal modal;
    public Button ContinueButton;

    public string ParentId { get; set; } = string.Empty;

    private readonly Dictionary<ChildFields, FormInput> _fields = new();

    public void Awake()
    {
        FormInput[] inputs = GetComponentsInChildren<FormInput>();
        foreach (FormInput input in inputs)
        {
            input.AddOnValidatedListener(UpdateContinueButton);
            _fields.Add(input.Key, input);
        }
        ContinueButton.interactable = false;
    }

    private void UpdateContinueButton()
    {
        bool allValid = true;
        foreach (KeyValuePair<ChildFields, FormInput> pair in _fields)
        {
            if (!pair.Value.Valid)
            {
                allValid = false;
                break;
            }
        }

        Debug.Log($"Form validation status: {allValid}");
        ContinueButton.interactable = allValid;
    }

    public ChildCreateDTO GatherChildData()
    {
        // string id = ChildIDField.GetData();
        // string firstName = ChildNameField.GetData();
        // string lastName = ChildLastNameField.GetData();
        //
        // string year = BirthYearField.text;
        // string month = BirthMonthField.text.PadLeft(2, '0');
        // string day = BirthDayField.text.PadLeft(2, '0');
        // string birthDate = $"{year}-{month}-{day}";
        //
        // string school = ChildSchoolField.GetData();
        //
        // int grade = ChildGradeField.value + 1;
        //
        // IEnumerable toggles = DisorderToggleGroup.ActiveToggles();
        //
        // string disorder = "No";
        // foreach (Toggle toggle in toggles)
        // {
        //     if (toggle.name == "Other")
        //     {
        //         disorder = OtherInput != null ? OtherInput.text : string.Empty;
        //     }
        //     disorder = toggle.name;
        // }

        ChildCreateDTO child = new();

        return child;
    }

    // private void OnInputChange(string value)
    // {
    //     // ValidateAllFields();
    // }
    //
    // private void OnDropdownChange(int value)
    // {
    //     // ValidateAllFields();
    // }
    //
    // private void OnToggleChange(bool value)
    // {
    //     bool valid = NoToggle.isOn || TELToggle.isOn || TEAToggle.isOn || TDAHToggle.isOn;
    //
    //     toogleGroupValid = valid;
    //
    //     if (!valid && OtherToggle.isOn)
    //     {
    //         OtherInput.onValueChanged.RemoveAllListeners();
    //         OtherInput.onValueChanged.AddListener(text =>
    //         {
    //             bool otherValid = !string.IsNullOrWhiteSpace(OtherInput.text);
    //             toogleGroupValid = otherValid;
    //             UpdateContinueButton();
    //         });
    //     }
    //
    //     UpdateContinueButton();
    // }
    //
    // private void OnToggleOtherChanged(bool isOn)
    // {
    //     OtherInput.gameObject.SetActive(isOn);
    //     if (!isOn)
    //         OtherInput.text = string.Empty;
    // }

    // private void ValidateAllFields()
    // {
    //     bool nameValid = !string.IsNullOrWhiteSpace(ChildNameField.text);
    //     bool lastNameValid = !string.IsNullOrWhiteSpace(ChildLastNameField.text);
    //     inputGroupValid = nameValid && lastNameValid;
    //
    //     schoolGroupValid = !string.IsNullOrWhiteSpace(ChildSchoolField.text);
    //
    //     bool dayValid = !string.IsNullOrWhiteSpace(BirthDayField.text);
    //     bool monthValid = !string.IsNullOrWhiteSpace(BirthMonthField.text);
    //     bool yearValid = !string.IsNullOrWhiteSpace(BirthYearField.text);
    //     birthdateGroupValid = dayValid && monthValid && yearValid;
    //
    //     UpdateContinueButton();
    // }
}
