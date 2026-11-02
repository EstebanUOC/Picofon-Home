#nullable enable

using System.Text.Json;

namespace Picofon.Core.Network
{
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// error_code que devuelve el backend en /segment-levels/* y POST /therapy/bulk.
    /// segment-levels lo pone al inicio de message.content ("CODE: detalle");
    /// /therapy/bulk lo pone en data.error_code.
    /// </summary>
    public static class ApiErrorCodes
    {
        // POST /therapy/bulk (Adaptive Policy v2, 409)
        public const string MixedPolicyVersions = "MIXED_POLICY_VERSIONS";
        public const string PolicyLanguageMismatch = "POLICY_LANGUAGE_MISMATCH";
        public const string PinnedPolicyVersionMismatch = "PINNED_POLICY_VERSION_MISMATCH";

        // POST /therapy/bulk (HOME_DISCOVERY v3): 409 ya iniciado, 422 lote inválido
        public const string HomeDiscoveryAlreadyStarted = "HOME_DISCOVERY_ALREADY_STARTED";
        public const string HomeDiscoveryPrefix = "HOME_DISCOVERY_";

        // /segment-levels/* (409)
        public const string PolicyCurriculumAlreadyStarted = "POLICY_CURRICULUM_ALREADY_STARTED";
        public const string HomeDiscoveryInProgress = "HOME_DISCOVERY_IN_PROGRESS";
        public const string ActiveLevelsRemain = "ACTIVE_LEVELS_REMAIN";
        public const string TherapyPlanNotCompleted = "THERAPY_PLAN_NOT_COMPLETED";
    }

    public readonly struct ApiError
    {
        public long StatusCode { get; init; }

        public string? ErrorCode { get; init; }

        public string? Message { get; init; }

        public bool Is(string errorCode) => ErrorCode == errorCode;

        /// <summary>
        /// El niño ya tiene ruta o niveles pendientes: no es un fallo, basta con
        /// recargar los planes y jugar el ACTIVE.
        /// </summary>
        public bool MeansReloadPlans =>
            StatusCode == 409
            && (
                Is(ApiErrorCodes.PolicyCurriculumAlreadyStarted)
                || Is(ApiErrorCodes.HomeDiscoveryInProgress)
                || Is(ApiErrorCodes.HomeDiscoveryAlreadyStarted)
                || Is(ApiErrorCodes.ActiveLevelsRemain)
                || Is(ApiErrorCodes.TherapyPlanNotCompleted)
            );

        public override string ToString() => $"HTTP {StatusCode} {ErrorCode}: {Message}";

        public static ApiError From(UnityWebRequestException e)
        {
            string? message = null;
            string? errorCode = null;

            try
            {
                if (!string.IsNullOrEmpty(e.Text))
                {
                    using JsonDocument doc = JsonDocument.Parse(e.Text);
                    JsonElement root = doc.RootElement;

                    message = new ApiResponseView(root).ErrorMessage;

                    if (
                        root.TryGetProperty(ApiJsonKeys.Data, out JsonElement data)
                        && data.ValueKind == JsonValueKind.Object
                        && data.TryGetProperty("error_code", out JsonElement code)
                    )
                    {
                        errorCode = code.GetString();
                    }
                }
            }
            catch (JsonException)
            {
                message = e.Text;
            }

            errorCode ??= CodeFromMessage(message);

            return new ApiError
            {
                StatusCode = e.ResponseCode,
                ErrorCode = errorCode,
                Message = message ?? e.Message,
            };
        }

        // "POLICY_CURRICULUM_ALREADY_STARTED: child_id=..." -> POLICY_CURRICULUM_ALREADY_STARTED
        private static string? CodeFromMessage(string? message)
        {
            if (string.IsNullOrEmpty(message))
                return null;

            int end = message.IndexOf(':');
            string candidate = end > 0 ? message[..end] : message;

            foreach (char c in candidate)
            {
                if (!(char.IsUpper(c) || char.IsDigit(c) || c == '_'))
                    return null;
            }

            return candidate;
        }
    }
}
