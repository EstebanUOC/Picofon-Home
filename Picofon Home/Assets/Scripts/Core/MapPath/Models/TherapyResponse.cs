using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyResponse
{
    [JsonInclude]
    public bool Success;

    [JsonInclude]
    public List<TherapyPlan> Data;

    public static TherapyResponse FromJson(string json)
    {
        return JsonHelper.FromJson<TherapyResponse>(json);
    }
}
