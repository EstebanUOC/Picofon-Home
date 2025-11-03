using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleGroupWithOther : MonoBehaviour
{
    [Header("Properties")]
    public Toggle otherToggle;
    public TMP_InputField otherInputField;

    private void Start()
    {
        otherInputField.gameObject.SetActive(false);

        otherToggle.onValueChanged.AddListener(OnOtherToggleChanged);
    }

    private void OnOtherToggleChanged(bool isOn)
    {
        otherInputField.gameObject.SetActive(isOn);

        if (!isOn)
            otherInputField.text = "";
    }
}
