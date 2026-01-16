#nullable enable

using System.Text.Json;
using UnityEngine;

namespace Picofon.Core.Network
{
    public static class ApiJsonKeys
    {
        public const string Success = "success";
        public const string Message = "message";
        public const string Content = "content";
        public const string Data = "data";
    }

    public readonly struct ApiResponseView<T>
    {
        private readonly JsonElement _root;

        public ApiResponseView(JsonElement root)
        {
            _root = root;
        }

        public bool Success
        {
            get
            {
                if (!_root.TryGetProperty(ApiJsonKeys.Success, out var success))
                {
                    Debug.Log("[Debug] 'success' property not found in JSON response.");
                    return false;
                }
                return success.GetBoolean();
            }
        }

        public string? ErrorMessage
        {
            get
            {
                if (!_root.TryGetProperty(ApiJsonKeys.Message, out var msg))
                {
                    Debug.Log("[Debug] 'message' property not found in JSON response.");
                    return null;
                }

                if (!msg.TryGetProperty(ApiJsonKeys.Content, out var content))
                {
                    Debug.Log("[Debug] 'content' property not found in 'message' JSON object.");
                    return null;
                }

                return content.GetArrayLength() > 0 ? content[0].GetString() : null;
            }
        }

        public T? Data
        {
            get
            {
                if (!_root.TryGetProperty(ApiJsonKeys.Data, out var dataElement))
                {
                    Debug.Log("[Debug] 'data' property not found in JSON response.");
                    return default;
                }

                return JsonHelper.FromJsonElement<T>(dataElement);
            }
        }
    }
}
