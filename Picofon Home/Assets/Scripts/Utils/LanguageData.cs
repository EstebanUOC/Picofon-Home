using UnityEngine;

public enum LanguageCode
{
    ES,
    CA,
}

[CreateAssetMenu(fileName = "LanguageData", menuName = "Languages/LanguageData")]
public class LanguageData : ScriptableObject
{
    public LanguageCode Code;
    public Sprite Flag;
}
