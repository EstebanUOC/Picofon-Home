using System;
using System.Collections.Generic;
using UnityEngine;

// 🔹 Modelo simplificado del JSON del modo 0
[System.Serializable]
public class ActivitySimple
{
    public string pregunta;
    public string palabra_principal;
    public string palabra_principal_PATH;
    public int palabra_principal_id;
    public string opcion1;
    public string opcion1_PATH;
    public int opcion1_id;
    public bool respuesta;
    public string feedback_positiu;
    public string feedback_neutre;
    public string feedback_no_resposta;
}

[System.Serializable]
public class AvailabilitySimple
{
    public int words_available;
    public int questions_possible;
    public int therapy_plan_requested;
    public int words_per_question;
    public bool sufficient_words;
    public int activity_number;
    public int activities_created;
    public int activities_requested;
}

[System.Serializable]
public class DataSimple
{
    public ActivitySimple activity1;
    public ActivitySimple activity2;
    public ActivitySimple activity3;
    public AvailabilitySimple availability_info;
}

[System.Serializable]
public class ApiResponseSimple
{
    public bool success;
    public Message message;
    public DataSimple data;
}

[System.Serializable]
public class Message
{
    public List<string> content;
    public bool displayable;
}
