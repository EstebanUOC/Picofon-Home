namespace Picofon.Utils
{
    using UnityEngine.Localization.Settings;

    /// <summary>
    /// Idioma que ve el usuario en la interfaz (locale seleccionado), para textos
    /// escritos en código que no pasan por las tablas de Localization.
    /// </summary>
    public static class UiLanguage
    {
        public static bool IsCatalan
        {
            get
            {
                var locale = LocalizationSettings.SelectedLocale;
                if (locale != null)
                {
                    return locale.Identifier.Code.StartsWith("ca");
                }

                string preferred = GamePrefs.PreferredLanguage;
                return !string.IsNullOrEmpty(preferred) && preferred[0] == 'C';
            }
        }

        public static string Text(string es, string ca) => IsCatalan ? ca : es;
    }
}
