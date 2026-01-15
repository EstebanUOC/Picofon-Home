#nullable enable

public class ApiResult<T>
{
    public bool Success { get; }
    public string? Message { get; }
    public T? Data { get; }

    public ApiResult(bool success, string? message, T? data)
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public static ApiResult<T> Ok(T data) => new(true, null, data);

    public static ApiResult<T> Fail(string message) => new(false, message, default);
}
