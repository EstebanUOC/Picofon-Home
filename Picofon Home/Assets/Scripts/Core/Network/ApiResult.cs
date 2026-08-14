namespace Picofon.Core.Network
{
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

    public class ApiResult
    {
        public bool Success { get; }
        public string? Message { get; }

        public ApiResult(bool success, string? message)
        {
            Success = success;
            Message = message;
        }

        public static ApiResult Ok() => new(true, null);

        public static ApiResult Fail(string message) => new(false, message);
    }
}
