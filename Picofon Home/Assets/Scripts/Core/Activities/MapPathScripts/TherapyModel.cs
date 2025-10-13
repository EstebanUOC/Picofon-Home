using System;
using UnityEngine;

[Serializable]
public class TherapyChildData
{
    public int total_levels;     // e.g., 10
    public string child_name;    // optional
    public int therapy_template_id; // optional
}

[Serializable]
public class TherapyResponse
{
    public bool success;
    public TherapyChildData data;
}
