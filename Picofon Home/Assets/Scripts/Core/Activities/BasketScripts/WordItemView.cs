using UnityEngine;

public sealed class WordItemView : MonoBehaviour
{
    [Space(15)]
    [SerializeField]
    private GameObject IconObject;

    [SerializeField]
    private GameObject TextObject;

    public GameObject Icon => IconObject;

    public GameObject Text => TextObject;
}
