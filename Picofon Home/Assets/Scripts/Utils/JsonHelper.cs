using System.Text;
using System.Text.Json;

public static class JsonHelper
{
    static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
        PropertyNameCaseInsensitive = true,
    };

    public static T FromJson<T>(string json)
    {
        T data = JsonSerializer.Deserialize<T>(json, jsonOptions);
        return data;
    }

    public static T FromJsonElement<T>(JsonElement element)
    {
        T data = JsonSerializer.Deserialize<T>(element, jsonOptions);
        return data;
    }

    public static string ToJson<T>(T data)
    {
        string json = JsonSerializer.Serialize(data, jsonOptions);
        return json;
    }
}

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return name;

        var sb = new StringBuilder();
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }
}
