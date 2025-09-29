using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SupportNeedsUI : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle toggleOption1;
    public Toggle toggleOption2;
    public Toggle toggleOption3;
    public Toggle toggleOther;

    [Header("Other Input Field")]
    public TMP_InputField otherInputField;

    private void Start()
    {
        // Make sure input field is hidden at start
        otherInputField.gameObject.SetActive(false);

        // Add listener for the "Other" toggle
        toggleOther.onValueChanged.AddListener(OnOtherToggleChanged);
    }

    private void OnOtherToggleChanged(bool isOn)
    {
        // Show or hide the input field
        otherInputField.gameObject.SetActive(isOn);
        
        if (!isOn)
        {
            // Clear text if user unchecks it
            otherInputField.text = "";
        }
    }
}
