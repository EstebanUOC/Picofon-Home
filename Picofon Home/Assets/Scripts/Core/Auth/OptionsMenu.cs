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

        string preferredLanguage = GamePrefs.PreferredLanguage;

        Debug.Log($"Preferred language: {preferredLanguage}");

        if (preferredLanguage[0] == 'C')
            return;

        if (Enum.TryParse(preferredLanguage, out LanguageCode language))
        {
            _languageComboBox.SetSelectedLanguage(language);
        }
    }

    private void HandleSaveButtonClicked()
    {
        GamePrefs.PreferredLanguage = _languageComboBox.GetSelectedLanguage().ToString();

        OnClose?.Invoke();
    }
}
