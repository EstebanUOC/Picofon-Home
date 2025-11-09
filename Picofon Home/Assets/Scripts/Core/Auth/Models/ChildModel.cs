using System;
using System.Text.Json.Serialization;

[Serializable]
public class ChildModel
{
    [JsonInclude]
    public string FirstName;

    [JsonInclude]
    public string LastName;

    [JsonInclude]
    public string BirthDate;

    [JsonInclude]
    public string Disorder;

    [JsonInclude]
    public string School;

    [JsonInclude]
    public int Grade;

    [JsonInclude]
    public int CenterId;

    [JsonInclude]
    public string OwnerId;

    [JsonInclude]
    public string Id;

    public static bool Validate(ChildModel child)
    {
        if (child == null)
            return false;

        bool isValid =
            !string.IsNullOrEmpty(child.FirstName)
            && !string.IsNullOrEmpty(child.LastName)
            && !string.IsNullOrEmpty(child.BirthDate)
            && !string.IsNullOrEmpty(child.Disorder)
            && !string.IsNullOrEmpty(child.School)
            && child.Grade > 0
            && child.CenterId > 0
            && !string.IsNullOrEmpty(child.OwnerId);

        if (!isValid)
            return false;

        return true;
    }

    public string ToJson()
    {
        return JsonHelper.ToJson(this);
    }
}
