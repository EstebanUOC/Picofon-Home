using System.Text.Json;
using System.Threading;
using BasketResponses;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public enum ActivityType : byte
{
    Judge = 1,
    Select = 2,
    Relate = 3,
}

public struct ActivityRequestParams
{
    public string PlanId;
    public string ChildId;
}

public class BasketService
{
    private const string UrlBase =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/";

    public async UniTask<ApiResult<T>> GetActivities<T>(CancellationToken token = default)
    {
        // string url = $"{UrlBase}{@params.PlanId}/{@params.ChildId}";
        //
        // byte[] rawResponse = await HttpClientUnity.GetAsyncBytes(
        //     url: url,
        //     cancellationToken: token
        // );
        //
        // using JsonDocument doc = JsonDocument.Parse(rawResponse);
        using JsonDocument doc = JsonDocument.Parse(RootPrueba());
        JsonElement root = doc.RootElement;

        ApiResponseView<T> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<T>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<T>.Ok(responseView.Data);
    }

    public async UniTask<ApiResult<T>> GetActivities<T>(
        ActivityRequestParams @params,
        CancellationToken token = default
    )
    {
        // string url = $"{UrlBase}{@params.PlanId}/{@params.ChildId}";
        //
        // byte[] rawResponse = await HttpClientUnity.GetAsyncBytes(
        //     url: url,
        //     cancellationToken: token
        // );
        //
        // using JsonDocument doc = JsonDocument.Parse(rawResponse);

        using JsonDocument doc = JsonDocument.Parse(RootPrueba(ActivityType.Select));
        JsonElement root = doc.RootElement;

        ApiResponseView<T> responseView = new(root);

        if (!responseView.Success)
        {
            return ApiResult<T>.Fail(responseView.ErrorMessage);
        }

        return ApiResult<T>.Ok(responseView.Data);
    }

    public string RootPrueba(ActivityType activityType = ActivityType.Judge)
    {
        string json =
            @"
{
    ""success"": true,
    ""data"": {
        ""activities"": [
            {
                ""words"": [
                    {
                        ""word"": ""maki"",
                        ""path"": ""IMG_0017.png"",
                        ""id"": 1001,
                        ""syllabified_word"": ""ma#ki"",
                        ""word_sound"": ""sound001.wav""
                    },
                    {
                        ""word"": ""mare"",
                        ""path"": ""IMG_0018.png"",
                        ""id"": 1002,
                        ""syllabified_word"": ""ma#re"",
                        ""word_sound"": ""sound002.wav""
                    }
                ],
                ""answer"": true
            },
            {
                ""words"": [
                    {
                        ""word"": ""martell"",
                        ""path"": ""IMG_0019.png"",
                        ""id"": 1003,
                        ""syllabified_word"": ""mar#tell"",
                        ""word_sound"": ""sound00x.wav""
                    },
                    {
                        ""word"": ""calze"",
                        ""path"": ""IMG_0031.png"",
                        ""id"": 1004,
                        ""syllabified_word"": ""cal#ze"",
                        ""word_sound"": ""sound00x.wav""
                    }
                ],
                ""answer"": false
            }
        ]
    }
}
";

        string jsonSelect =
            @"
{
    ""success"": true,
    ""data"": {
        ""activities"": [
            {
                ""words"": [
                    {
                        ""word"": ""maki"",
                        ""path"": ""IMG_0017.png"",
                        ""id"": 1001,
                        ""syllabified_word"": ""ma#ki"",
                        ""word_sound"": ""sound001.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""mare"",
                        ""path"": ""IMG_0018.png"",
                        ""id"": 1002,
                        ""syllabified_word"": ""ma#re"",
                        ""word_sound"": ""sound002.wav"",
                        ""answer"": false

                    },
                    {
                        ""word"": ""bagul"",
                        ""path"": ""img3.png"",
                        ""id"": 1003,
                        ""syllabified_word"": ""ba#gul"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""martell"",
                        ""path"": ""img4.png"",
                        ""id"": 1003,
                        ""syllabified_word"": ""mar#tell"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    }
                ]
            },
            {
                ""words"": [
                    {
                        ""word"": ""martell"",
                        ""path"": ""IMG_0019.png"",
                        ""id"": 1003,
                        ""syllabified_word"": ""mar#tell"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""calze"",
                        ""path"": ""IMG_0031.png"",
                        ""id"": 1004,
                        ""syllabified_word"": ""cal#ze"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""dames"",
                        ""path"": ""img3.png"",
                        ""id"": 1003,
                        ""syllabified_word"": ""da#mes"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""mare"",
                        ""path"": ""img2.png"",
                        ""id"": 1002,
                        ""syllabified_word"": ""ma#re"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    }
                ]
            }
        ]
    }
}
";

        return activityType switch
        {
            ActivityType.Judge => json,
            ActivityType.Select => jsonSelect,
            ActivityType.Relate => jsonSelect,
            _ => RootPrueba(),
        };
    }
}
