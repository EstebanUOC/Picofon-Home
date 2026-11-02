using Picofon.Utils;

namespace Picofon.Core.MapPath
{
    /// <summary>
    /// Mensajes del mapa al empezar el screening inicial (HOME_DISCOVERY) y la
    /// malla curricular. Van en el idioma de trabajo del niño (el de sus planes),
    /// no en el idioma de la interfaz. Primero una frase para el niño y, más
    /// pequeño, la indicación para el logopeda / docente.
    /// </summary>
    public static class MapRouteTexts
    {
        public static string ScreeningStart(LanguageID language) =>
            language == LanguageID.Catalan
                ? "Anem a descobrir els teus superpoders amb els sons!\n\n"
                    + "<size=70%>Per a l'adult: comença el cribratge inicial (Home Discovery). "
                    + "Els nivells daurats són el cribratge; després començarà la malla curricular.</size>"
                : "¡Vamos a descubrir tus superpoderes con los sonidos!\n\n"
                    + "<size=70%>Para el adulto: empieza el screening inicial (Home Discovery). "
                    + "Los niveles dorados son el screening; después empezará la malla curricular.</size>";

        public static string CurriculumStart(LanguageID language) =>
            language == LanguageID.Catalan
                ? "Molt bé! Ara comença la teva aventura amb els sons.\n\n"
                    + "<size=70%>Per a l'adult: cribratge completat. Comença la malla curricular.</size>"
                : "¡Muy bien! Ahora empieza tu aventura con los sonidos.\n\n"
                    + "<size=70%>Para el adulto: screening completado. Empieza la malla curricular.</size>";
    }
}
