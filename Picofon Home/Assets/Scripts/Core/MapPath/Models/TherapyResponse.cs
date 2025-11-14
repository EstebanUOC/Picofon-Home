using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

[Serializable]
public class TherapyResponse
{
    [JsonInclude]
    public bool Success { get; set; } = false;

    [JsonInclude]
    public List<TherapyPlan> Data { get; set; }

    public static TherapyResponse FromJson(string json)
    {
        return JsonHelper.FromJson<TherapyResponse>(json);
    }
}
