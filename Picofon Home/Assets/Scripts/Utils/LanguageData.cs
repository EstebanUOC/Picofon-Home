namespace Picofon.Utils
{
    using UnityEngine;

    public enum LanguageCode
    {
        ES,
        CA,
    }

    public enum LanguageID : byte
    {
        Catalan = 1,
        Spanish = 2,
    }

    [CreateAssetMenu(fileName = "LanguageData", menuName = "Languages/LanguageData")]
    public class LanguageData : ScriptableObject
    {
        public LanguageCode Code;
        public Sprite Flag;
    }
}
