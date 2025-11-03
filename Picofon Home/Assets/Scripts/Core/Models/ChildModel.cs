using System;

[Serializable]
public class ChildModel
{
    public string FirstName;
    public string LastName;
    public string BirthDate;
    public string Disorder;
    public string School;
    public string Grade;
    public int CenterId;
    public string OwnerId;
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
            && !string.IsNullOrEmpty(child.Grade)
            && child.CenterId > 0
            && !string.IsNullOrEmpty(child.OwnerId);

        if (!isValid)
            return false;

        return true;
    }

    public string ToJson()
    {
        string jsonString =
            $@"{{
                    ""first_name"": ""{FirstName}"",
                    ""last_name"": ""{LastName}"",
                    ""birth_date"": ""{BirthDate}"",
                    ""disorder"": ""{Disorder}"",
                    ""school"": ""{School}"",
                    ""grade"": ""{Grade}"",
                    ""center_id"": {CenterId},
                    ""owner_id"": ""{OwnerId}"",
                    ""id"": ""{Id}""
                }}";
        return jsonString;
    }
}
