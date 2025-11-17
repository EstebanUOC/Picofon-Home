using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class UserChildrenCountResponse : MonoBehaviour
{
    [JsonInclude]
    public bool Success { get; set; } = false;

    [JsonInclude]
    public List<ChildModel> Data { get; set; }

    public static UserChildrenCountResponse FromJson(string json)
    {
        return JsonHelper.FromJson<UserChildrenCountResponse>(json);
    }
}
