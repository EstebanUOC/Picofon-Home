using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class TherapyTemplate
{
    public string name;
    public string description;
    public int task_type_id;
    public int sound_id;
    public int skill_id;
    public int syllables_number;
    public string syllable_structure;
    public string syllable_position;
    public string difficulty_level;
    public int id;
    public bool is_active;
}

[Serializable]
public class TherapyPlan
{
    public int therapy_template_id;
    public string assigned_by_id;
    public string plan_number;
    public string name;
    public int target_sessions;
    public string notes;
    public string status;
    public string start_date;
    public string child_id;
    public int id;
    public string created_at;
    public string updated_at;
    public TherapyTemplate therapy_template;
}

[Serializable]
public class TherapyResponse
{
    public bool success;
    public List<TherapyPlan> data;
}
