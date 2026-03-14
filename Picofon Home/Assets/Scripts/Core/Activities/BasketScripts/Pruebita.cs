using System;
using UnityEngine;

[Serializable]
public struct LabeledString
{
    public string label;
    public string value;
}

public class Pruebita : MonoBehaviour
{
    [SerializeField]
    private LabeledString myLabeledString;
}
