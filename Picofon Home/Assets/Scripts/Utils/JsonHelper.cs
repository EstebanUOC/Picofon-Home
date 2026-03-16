using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class JsonHelper
{
    static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new CaseInsensitiveEnumConverter<UserRole>(),
            new CaseInsensitiveEnumConverter<TherapyStatus>(),
        },
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

    public static byte[] ToBytes<T>(T data)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(data, jsonOptions);
        return bytes;
    }

    public static byte[] ToBytes<T>(in T data)
    {
        byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(data, jsonOptions);
        return bytes;
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

public class CaseInsensitiveEnumConverter<T> : JsonConverter<T>
    where T : struct, Enum
{
    public override T Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var value = reader.GetString();
        if (Enum.TryParse<T>(value, true, out var result))
            return result;
        throw new JsonException($"Unable to convert \"{value}\" to Enum \"{typeof(T)}\"");
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToUpperInvariant());
    }
}
