using System;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private ComboBox _languageComboBox;

    [SerializeField]
    private CustomButtonRaised _saveButton;

    public Action OnClose;

    public void Awake()
    {
        _saveButton.OnClick += HandleSaveButtonClicked;
    }

    private void HandleSaveButtonClicked()
    {
        PerformanceLog.Log($"Selected language: {_languageComboBox.GetSelectedLanguage()}");

        OnClose?.Invoke();
    }
}
