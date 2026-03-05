using TMPro;
using UnityEngine;

public class DropdownField : FormInput
{
    [Space]
    public TMP_Dropdown Dropdown;

    public void Start()
    {
        Valid = true;
    }

    public override string GetData()
    {
        int value = Dropdown.value + 1;
        return value.ToString();
    }
}
