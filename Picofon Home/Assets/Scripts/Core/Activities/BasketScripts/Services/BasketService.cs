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

    // TODO: Remove duplicate
    public async UniTask<ApiResult<T>> GetActivitiesP<T>(
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

        using JsonDocument doc = JsonDocument.Parse(RootPrueba(ActivityType.Relate));
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
                        ""word"": ""mare"",
                        ""path"": ""IMG_0015.png"",
                        ""id"": 2912,
                        ""syllabified_word"": ""ma#re"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""maça"",
                        ""path"": ""IMG_0011.png"",
                        ""id"": 2908,
                        ""syllabified_word"": ""ma#ça"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""ramat"",
                        ""path"": ""IMG_0608.png"",
                        ""id"": 3597,
                        ""syllabified_word"": ""ra#mat"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""mamà"",
                        ""path"": ""IMG_0105.png"",
                        ""id"": 2910,
                        ""syllabified_word"": ""ma#mà"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    }
                ]
            },
            {
                ""words"": [
                    {
                        ""word"": ""maki"",
                        ""path"": ""IMG_0012.png"",
                        ""id"": 2909,
                        ""syllabified_word"": ""ma#ki"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""nadal"",
                        ""path"": ""IMG_0311.png"",
                        ""id"": 3230,
                        ""syllabified_word"": ""na#dal"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""maça"",
                        ""path"": ""IMG_0011.png"",
                        ""id"": 2908,
                        ""syllabified_word"": ""ma#ça"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""matí"",
                        ""path"": ""IMG_0016.png"",
                        ""id"": 2914,
                        ""syllabified_word"": ""ma#tí"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    }
                ]
            }
        ]
    }
}
";

        string jsonRelate =
            @"
{
    ""success"": true,
    ""data"": {
        ""activities"": [
            {
                ""main_word"": {
                    ""word"": ""maça"",
                    ""path"": ""IMG_0011.png"",
                    ""id"": 2908,
                    ""syllabified_word"": ""ma#ça"",
                    ""word_sound"": ""sound00x.wav""
                },
                ""words"": [
                    {
                        ""word"": ""mamà"",
                        ""path"": ""IMG_0105.png"",
                        ""id"": 2910,
                        ""syllabified_word"": ""ma#mà"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""xarop"",
                        ""path"": ""IMG_2113.png"",
                        ""id"": 4393,
                        ""syllabified_word"": ""xa#rop"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""galta"",
                        ""path"": ""IMG_2412.png"",
                        ""id"": 5412,
                        ""syllabified_word"": ""gal#ta"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""ballet"",
                        ""path"": ""IMG_2435.png"",
                        ""id"": 4525,
                        ""syllabified_word"": ""ba#llet"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    }
                ]
            },
            {
                ""main_word"": {
                    ""word"": ""matí"",
                    ""path"": ""IMG_0016.png"",
                    ""id"": 2914,
                    ""syllabified_word"": ""ma#tí"",
                    ""word_sound"": ""sound00x.wav""
                },
                ""words"": [
                    {
                        ""word"": ""mare"",
                        ""path"": ""IMG_0015.png"",
                        ""id"": 2912,
                        ""syllabified_word"": ""ma#re"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": true
                    },
                    {
                        ""word"": ""saltar"",
                        ""path"": ""IMG_1164.png"",
                        ""id"": 4064,
                        ""syllabified_word"": ""sal#tar"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""natja"",
                        ""path"": ""IMG_1609.png"",
                        ""id"": 3232,
                        ""syllabified_word"": ""nat#ja"",
                        ""word_sound"": ""sound00x.wav"",
                        ""answer"": false
                    },
                    {
                        ""word"": ""capa"",
                        ""path"": ""IMG_2099.png"",
                        ""id"": 5533,
                        ""syllabified_word"": ""ca#pa"",
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
            ActivityType.Relate => jsonRelate,
            _ => RootPrueba(),
        };
    }
}
