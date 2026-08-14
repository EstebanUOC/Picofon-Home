namespace Picofon.Utils
{
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    public static class GamePrefs
    {
        public static string PreferredLanguage
        {
            get => PlayerPrefs.GetString(nameof(PreferredLanguage), "CA");
            set
            {
                if (PreferredLanguage == value)
                {
                    return;
                }

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                int index = 0;

                if (value[0] != 'C')
                {
                    index = 1;
                }

                LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[
                    index
                ];

                PlayerPrefs.SetString(nameof(PreferredLanguage), value);
                Save();
            }
        }

        public static LanguageID PreferredLanguageID
        {
            get
            {
                string language = PreferredLanguage;

                if (language[0] != 'C')
                {
                    return LanguageID.Spanish;
                }

                return LanguageID.Catalan;
            }
            set
            {
                if (PreferredLanguageID == value)
                {
                    return;
                }

                if (value == LanguageID.Catalan)
                {
                    PreferredLanguage = "CA";
                }
                else
                {
                    PreferredLanguage = "ES";
                }
            }
        }

        public static int LastCompletedLevel
        {
            get => PlayerPrefs.GetInt(nameof(LastCompletedLevel), 0);
            set
            {
                PlayerPrefs.SetInt(nameof(LastCompletedLevel), value);
                Save();
            }
        }

        public static bool HasAcceptedTerms
        {
            get => PlayerPrefs.GetInt(nameof(HasAcceptedTerms), 0) == 1;
            set
            {
                PlayerPrefs.SetInt(nameof(HasAcceptedTerms), value ? 1 : 0);
                Save();
            }
        }

        public static bool DebugMode;

        public static void ClearAll()
        {
            PlayerPrefs.DeleteAll();
            Save();
        }

        private static void Save()
        {
            PlayerPrefs.Save();
        }
    }
}
