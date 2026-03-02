using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    Center,
}

public class Form : MonoBehaviour
{
    [SerializeField]
    private CustomButtonLoading _submitButton;

    public string ParentId { get; set; } = string.Empty;

    public event Func<ChildCreateDTO, UniTask> OnSubmit;

    private readonly Dictionary<ChildFields, FormInput> _fields = new();

    private readonly InputChangedEventChannel _inputChangedEventChannel = new();

    public void Start()
    {
        _inputChangedEventChannel.OnInputChanged += HandleInputChanged;

        foreach (Transform child in transform)
        {
            FormInput input = child.GetComponent<FormInput>();
            if (input != null)
            {
                input.SetInputChangedEventChannel(_inputChangedEventChannel);
                _fields.Add(input.Key, input);
            }
        }

        _submitButton.Interactable = false;
        _submitButton.OnClickAsync += HandleSubmit;
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
            CenterId = int.Parse(_fields[ChildFields.Center].GetData()),
        };

        return child;
    }

    private async UniTask HandleSubmit()
    {
        ChildCreateDTO data = GatherChildData();

        if (OnSubmit != null)
            await OnSubmit.Invoke(data);
    }

    private void HandleInputChanged(bool valid)
    {
        if (valid)
            UpdateContinueButton();
        else
            _submitButton.Interactable = false;
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

        _submitButton.Interactable = allValid;
    }
}
