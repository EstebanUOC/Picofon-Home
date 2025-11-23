using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupField : FormInput
{
    [Space(15)]
    public ToggleGroup ToggleGroup;

    [Space(15)]
    public Toggle OtherToggle;
    public FormInput OtherInput;

    private Action onValidatedHandler;
    private Action onInvalidatedHandler;

    public void Start()
    {
        OtherInput.gameObject.SetActive(false);

        foreach (Toggle toggle in ToggleGroup.GetComponentsInChildren<Toggle>())
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        OtherToggle.onValueChanged.RemoveAllListeners();
        OtherToggle.onValueChanged.AddListener(OnOtherToggleChanged);

        onValidatedHandler = () => Valid = true;
        onInvalidatedHandler = () => Valid = false;
    }

    public override string GetData()
    {
        if (OtherToggle.isOn)
            return OtherInput.GetData();

        IEnumerable toggles = ToggleGroup.ActiveToggles();

        string disorder = string.Empty;
        foreach (Toggle toggle in toggles)
        {
            disorder = toggle.name;
        }

        return disorder;
    }

    private void OnOtherToggleChanged(bool isOn)
    {
        OtherInput.gameObject.SetActive(isOn);
        if (isOn)
        {
            OtherInput.OnValidated += onValidatedHandler;
            OtherInput.OnInvalidated += onInvalidatedHandler;
            Valid = OtherInput.Valid;
        }
        else
        {
            OtherInput.OnValidated -= onValidatedHandler;
            OtherInput.OnInvalidated -= onInvalidatedHandler;
            Valid = false;
        }
    }

    private void OnToggleChanged(bool value)
    {
        Valid = value;
    }
}
