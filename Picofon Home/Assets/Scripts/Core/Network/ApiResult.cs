namespace Picofon.Core.Network
{
#nullable enable

    public class ApiResult<T>
    {
        public bool Success { get; }
        public string? Message { get; }
        public T? Data { get; }

        // Código HTTP + error_code del backend cuando la petición devolvió error
        public ApiError? Error { get; }

        public ApiResult(bool success, string? message, T? data, ApiError? error = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Error = error;
        }

        public static ApiResult<T> Ok(T data) => new(true, null, data);

        public static ApiResult<T> Fail(string message) => new(false, message, default);

        public static ApiResult<T> Fail(ApiError error) =>
            new(false, error.Message, default, error);
    }

    public class ApiResult
    {
        public bool Success { get; }
        public string? Message { get; }

        // Código HTTP + error_code del backend cuando la petición devolvió error
        public ApiError? Error { get; }

        public ApiResult(bool success, string? message, ApiError? error = null)
        {
            Success = success;
            Message = message;
            Error = error;
        }

        public static ApiResult Ok() => new(true, null);

        public static ApiResult Fail(string message) => new(false, message);

        public static ApiResult Fail(ApiError error) => new(false, error.Message, error);
    }
}
