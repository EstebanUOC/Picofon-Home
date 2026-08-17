using Picofon.Core.Auth.DTOs;
using Picofon.Core.Auth.Models.Login;
using Picofon.Core.Auth.Models.User;
using Picofon.Core.Auth.Panels;
using Picofon.Core.MapPath.Services;
using Picofon.Core.Network;
using Picofon.Utils;

namespace Picofon.Core.Auth.Services
{
    using System.Text.Json;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public readonly struct LoginData
    {
        public readonly bool IsNewUser { get; init; }

        public readonly UserModel User { get; init; }
    }

    public readonly struct LoginMailData
    {
        public readonly string IdToken { get; init; }
    }

    public readonly struct UpdateUserRoleRequest
    {
        public readonly UserRole Role { get; init; }
    }

    public readonly struct RegisterRequest
    {
        public readonly string FirebaseIdToken { get; init; }

        public bool LegalAccepted { get; init; }

        public UserRole Role { get; init; }
    }

    public readonly struct RegisterResponse
    {
        public readonly UserModel User { get; init; }
    }

    public class UserService
    {
        private readonly string ChildrenURL = ApiConfig.BaseUrl + "children/";

        public async UniTask<ApiResult<RegisterResponse>> RegisterWithFirebaseToken(
            string firebaseToken,
            bool disclaimerAccepted,
            UserRole role,
            CancellationToken token = default
        )
        {
            string url = $"{ApiConfig.BaseUrl}auth/register";

            byte[] rawResponse;

            RegisterRequest request = new()
            {
                FirebaseIdToken = firebaseToken,
                LegalAccepted = disclaimerAccepted,
                Role = role,
            };

            byte[] jsonRequest = JsonHelper.ToBytes(in request);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception e)
            {
                PerformanceLog.LogError(
                    $"Error registering user: {e.Message}, URL: {url}, Payload: {request}"
                );
                return ApiResult<RegisterResponse>.Fail("NET_ERR_REG_MAIL_CREDENTIALS");
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            ApiResponseView<RegisterResponse> responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult<RegisterResponse>.Fail(responseView.ErrorMessage);
            }

            return ApiResult<RegisterResponse>.Ok(responseView.Data);
        }

        public async UniTask<ApiResult<RegisterResponse>> RegisterCredentials(
            string email,
            string password,
            CancellationToken token = default
        )
        {
            string url =
                "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyCguAf29FXSGYB73ZPNb0uCBBdymzSrerc";

            byte[] rawResponse;

            FirebaseRequest request = new()
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true,
            };

            byte[] jsonRequest = JsonHelper.ToBytes(in request);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (UnityWebRequestException e)
            {
                if (e.ResponseCode != 400)
                    return ApiResult<RegisterResponse>.Fail("NET_ERR_REG_MAIL_CREDENTIALS");

                using JsonDocument errorDoc = JsonDocument.Parse(e.Text);
                JsonElement errorRoot = errorDoc.RootElement;

                if (!errorRoot.TryGetProperty("error", out var errorElement))
                {
                    return ApiResult<RegisterResponse>.Fail("NET_ERR_REG_MAIL_CREDENTIALS");
                }

                return ApiResult<RegisterResponse>.Fail(
                    errorElement.GetProperty("message").GetString()
                );
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("idToken", out var dataElement))
            {
                return ApiResult<RegisterResponse>.Fail("NET_ERR_REG_MAIL_CREDENTIALS");
            }

            ApiResult<RegisterResponse> result = await RegisterWithFirebaseToken(
                firebaseToken: dataElement.GetString(),
                disclaimerAccepted: true,
                UserRole.Parent,
                token
            );

            if (!result.Success)
            {
                return ApiResult<RegisterResponse>.Fail(result.Message);
            }

            return ApiResult<RegisterResponse>.Ok(result.Data);
        }

        public async UniTask<ApiResult<LoginData>> LoginWithFirebaseToken(
            string firebaseToken,
            CancellationToken token = default
        )
        {
            string url = $"{ApiConfig.BaseUrl}auth/login";

            byte[] rawResponse;

            LoginFirebaseRequest loginRequest = new() { FirebaseIdToken = firebaseToken };

            byte[] jsonRequest = JsonHelper.ToBytes(in loginRequest);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (UnityWebRequestException e)
            {
                if (e.ResponseCode != 404)
                {
                    PerformanceLog.LogError(
                        $"Error logging in user with Firebase token: {firebaseToken}\n"
                    );
                    return ApiResult<LoginData>.Fail("Network error occurred while logging in.");
                }

                LoginData newUserData = new() { IsNewUser = true };

                return ApiResult<LoginData>.Ok(newUserData);
            }
            catch (System.Exception)
            {
                return ApiResult<LoginData>.Fail("Network error occurred while logging in.");
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            ApiResponseView<LoginData> responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult<LoginData>.Fail(responseView.ErrorMessage);
            }

            return ApiResult<LoginData>.Ok(responseView.Data);
        }

        public async UniTask<ApiResult<LoginData>> LoginWithCredentials(
            string email,
            string password,
            CancellationToken token = default
        )
        {
            string url =
                "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyCguAf29FXSGYB73ZPNb0uCBBdymzSrerc";

            byte[] rawResponse;

            FirebaseRequest request = new()
            {
                Email = email,
                Password = password,
                ReturnSecureToken = true,
            };

            byte[] jsonRequest = JsonHelper.ToBytes(in request);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (UnityWebRequestException e)
            {
                if (e.ResponseCode != 400)
                    return ApiResult<LoginData>.Fail("NET_ERR_LOGIN_MAIL_CREDENTIALS");

                using JsonDocument errorDoc = JsonDocument.Parse(e.Text);
                JsonElement errorRoot = errorDoc.RootElement;

                if (!errorRoot.TryGetProperty("error", out var errorElement))
                {
                    return ApiResult<LoginData>.Fail("NET_ERR_LOGIN_MAIL_CREDENTIALS");
                }

                return ApiResult<LoginData>.Fail(errorElement.GetProperty("message").GetString());
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("idToken", out var dataElement))
            {
                return ApiResult<LoginData>.Fail("NET_ERR_LOGIN_MAIL_CREDENTIALS");
            }

            ApiResult<LoginData> result = await LoginWithFirebaseToken(
                dataElement.GetString(),
                token
            );

            if (!result.Success)
            {
                return ApiResult<LoginData>.Fail(result.Message);
            }

            return ApiResult<LoginData>.Ok(result.Data);
        }

        public async UniTask<ApiResult> UpdateUserRole(
            string userId,
            UserRole newRole,
            CancellationToken token = default
        )
        {
            if (newRole == UserRole.Admin)
            {
                return ApiResult.Fail("Cannot assign Admin role through this method.");
            }

            string url = $"{ApiConfig.BaseUrl}/users/{userId}/role";

            byte[] rawResponse;

            UpdateUserRoleRequest requestData = new() { Role = newRole };

            byte[] jsonRequest = JsonHelper.ToBytes(requestData);

            try
            {
                rawResponse = await HttpClientUnity.PatchAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception)
            {
                return ApiResult.Fail("Network error occurred while updating user role.");
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            ApiResponseView responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult.Fail(responseView.ErrorMessage);
            }

            return ApiResult.Ok();
        }

        public async UniTask<ApiResult<ChildListItemDTO[]>> GetUserChildren(
            string userId,
            int centerId = -1,
            CancellationToken token = default
        )
        {
            string url;

            if (centerId != -1)
            {
                url = $"{ChildrenURL}/user/{userId}/center/{centerId}";
            }
            else
            {
                url = $"{ChildrenURL}/user/{userId}";
            }

            byte[] rawResponse;

            try
            {
                rawResponse = await HttpClientUnity.GetAsyncBytes(
                    url: url,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception)
            {
                return ApiResult<ChildListItemDTO[]>.Fail(
                    "Network error occurred while fetching activities."
                );
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            ApiResponseView<ChildListItemDTO[]> responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult<ChildListItemDTO[]>.Fail(responseView.ErrorMessage);
            }

            return ApiResult<ChildListItemDTO[]>.Ok(responseView.Data);
        }

        public async UniTask<ApiResult> RegisterChild(
            CreateChildDTO childCreateDTO,
            CancellationToken token = default
        )
        {
            string url = ChildrenURL;

            byte[] rawResponse;

            byte[] jsonRequest = JsonHelper.ToBytes(childCreateDTO);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception)
            {
                return ApiResult.Fail("Network error occurred while registering child.");
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);
            JsonElement root = doc.RootElement;

            ApiResponseView responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult.Fail(responseView.ErrorMessage);
            }

            PerformanceLog.Log(
                $"Child registered successfully: {childCreateDTO.FirstName} {childCreateDTO.LastName}, ID: {childCreateDTO.Id}"
            );

            // After successfully registering the child, create default therapy plans for the child

            url = $"{ApiConfig.BaseUrl}/therapy/default";

            CreateDefaultPlansRequest request = new()
            {
                ChildId = childCreateDTO.Id,
                AssignedById = childCreateDTO.UserId,
            };

            jsonRequest = JsonHelper.ToBytes(in request);

            try
            {
                rawResponse = await HttpClientUnity.PostAsyncBytes(
                    url: url,
                    data: jsonRequest,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception e)
            {
                PerformanceLog.LogError(
                    $"Error creating default therapy plans for child {childCreateDTO.Id}: {e.Message}, URL: {url}, Payload: {request}"
                );
                return ApiResult.Fail(
                    "Network error occurred while creating default therapy plans."
                );
            }

            using JsonDocument planDoc = JsonDocument.Parse(rawResponse);
            JsonElement planRoot = planDoc.RootElement;

            ApiResponseView planResponseView = new(planRoot);

            if (!planResponseView.Success)
            {
                return ApiResult.Fail(planResponseView.ErrorMessage);
            }

            PerformanceLog.Log(
                $"Default therapy plans created successfully for child ID: {childCreateDTO.Id}"
            );

            return ApiResult.Ok();
        }

        public async UniTask<ApiResult<CenterDTO[]>> GetCenters(
            string userId,
            CancellationToken token = default
        )
        {
            string url = $"{ApiConfig.BaseUrl}/centers/user/{userId}";

            byte[] rawResponse;

            try
            {
                rawResponse = await HttpClientUnity.GetAsyncBytes(
                    url: url,
                    timeoutSeconds: 5,
                    cancellationToken: token
                );
            }
            catch (System.Exception e)
            {
                PerformanceLog.LogError(
                    $"Error fetching centers for user {userId}: {e.Message}, URL: {url}"
                );
                return ApiResult<CenterDTO[]>.Fail(
                    "Network error occurred while fetching centers."
                );
            }

            using JsonDocument doc = JsonDocument.Parse(rawResponse);

            JsonElement root = doc.RootElement;

            ApiResponseView<CenterDTO[]> responseView = new(root);

            if (!responseView.Success)
            {
                return ApiResult<CenterDTO[]>.Fail(responseView.ErrorMessage);
            }

            return ApiResult<CenterDTO[]>.Ok(responseView.Data);
        }
    }
}
