using System.Collections.Generic;
using UnityEngine;

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
    public CustomButtonLoading ContinueButton;

    public string ParentId { get; set; } = string.Empty;

    private readonly Dictionary<ChildFields, FormInput> _fields = new();

    public void Awake()
    {
        Transform parent = transform;
        foreach (Transform child in parent)
        {
            FormInput input = child.GetComponent<FormInput>();
            if (input != null)
            {
                input.OnValidated += UpdateContinueButton;
                input.OnInvalidated += DisableButton;
                _fields.Add(input.Key, input);
            }
        }
        ContinueButton.Interactable = false;
    }

    private void DisableButton()
    {
        ContinueButton.Interactable = false;
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

        ContinueButton.Interactable = allValid;
    }

    public ChildCreateDTO GatherChildData()
    {
        ChildCreateDTO child = new()
        {
            OwnerId = ParentId,
            Id = _fields[ChildFields.ID].GetData(),
            FirstName = _fields[ChildFields.FirstName].GetData(),
            LastName = _fields[ChildFields.LastName].GetData(),
            BirthDate = _fields[ChildFields.BirthDate].GetData(),
            Disorder = _fields[ChildFields.Disorder].GetData(),
            School = _fields[ChildFields.School].GetData(),
            Grade = int.Parse(_fields[ChildFields.Grade].GetData()),
        };

        return child;
    }
}
