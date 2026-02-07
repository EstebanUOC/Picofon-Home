using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SessionService
{
    private readonly string url = ApiConfig.BaseUrl + "therapy-sessions";

    public async UniTask<ApiResult> CreateTherapySession(CancellationToken token = default)
    {
        string url = this.url;

        byte[] rawResponse;

        TherapySessionDTO requestData = new()
        {
            Id = 10,
            TherapyPlanId = 99,
            ChildId = "19013454K",
        };

        string json = JsonHelper.ToJson(requestData);

        Debug.Log($"Request JSON: {json}");

        string json1 = "{\"id\":10,\"therapy_plan_id\":99,\"child_id\":\"19013454K\"}";

        // Struct: setter privado, no se asigna therapy_plan_id
        var structResult = JsonHelper.FromJson<TherapySessionDTO>(json1);
        Debug.Log($"Struct: TherapyPlanId = {structResult.TherapyPlanId}");

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
