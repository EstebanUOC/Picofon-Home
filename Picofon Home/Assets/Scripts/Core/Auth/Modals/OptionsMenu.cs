using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField]
    private ComboBox _languageComboBox;

    [SerializeField]
    private CustomButtonRaised _saveButton;

    [SerializeField]
    private TMPro.TMP_Text _versionText;

    public GenericEventChannel EventChannel;

    public void Awake()
    {
        _saveButton.OnClick += HandleClick;

        string preferredLanguage = GamePrefs.PreferredLanguage;

        if (preferredLanguage[0] == 'C')
            return;

        if (Enum.TryParse(preferredLanguage, out LanguageCode language))
        {
            _languageComboBox.SetSelectedLanguage(language);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void SetVersionText(float version)
    {
        _versionText.text = version.ToString("0.00");
    }

    private void HandleClick()
    {
        GamePrefs.PreferredLanguage = _languageComboBox.GetSelectedLanguage().ToString();

        int index = 0;

        if (GamePrefs.PreferredLanguage[0] != 'C')
        {
            index = 1;
        }

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];

        EventChannel.Raise();
    }
}
