using UnityEngine;

public class LanguageItem : MonoBehaviour
{
    [SerializeField]
    private LanguageData _data;

    public LanguageData Data => _data;
}
