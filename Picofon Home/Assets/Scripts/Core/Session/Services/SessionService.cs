using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SessionService
{
    private readonly string url = ApiConfig.BaseUrl + "therapy-sessions";

    public async UniTask<ApiResult> CreateTherapySession(
        GeneralSessionDTO sessionInfo,
        TherapySessionDTO[] sessions,
        CancellationToken token = default
    )
    {
        string url = this.url;

        byte[] rawResponse;

        TherapySessionCreateRequest sessionRequest = new()
        {
            General = sessionInfo,
            Sessions = sessions,
        };

        string json = JsonHelper.ToJson(sessionRequest);

        Debug.Log($"Request JSON: {json}");

        // TherapySessionCreateRequest requestData = new()
        // {
        //     TherapyPlanId = therapyPlanId,
        //     ChildId = childId,
        // };

        // byte[] jsonRequest = JsonHelper.ToBytes(in requestData);

        try
        {
            // rawResponse = await HttpClientUnity.PostAsyncBytes(
            //     url: url,
            //     data: jsonRequest,
            //     timeoutSeconds: 5,
            //     cancellationToken: token
            // );
        }
        catch (System.Exception)
        {
            return ApiResult.Fail("Network error occurred while creating therapy session.");
        }

        // using JsonDocument doc = JsonDocument.Parse(rawResponse);
        // JsonElement root = doc.RootElement;
        //
        // ApiResponseView<TherapySessionDTO> responseView = new(root);
        //
        // if (!responseView.Success)
        // {
        //     return ApiResult.Fail(responseView.ErrorMessage);
        // }

        return ApiResult.Ok();
    }
}
