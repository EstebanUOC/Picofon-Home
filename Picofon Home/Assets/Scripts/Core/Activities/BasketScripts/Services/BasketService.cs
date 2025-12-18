using System.Text.Json;
using System.Threading;
using BasketResponses;
using Cysharp.Threading.Tasks;
using Picofon.Core.Network;

public class BasketService
{
    private const string UrlBase =
        "https://ehc-picofon2.techlab.uoc.edu/api/v1/unity-proxy/questions/";

    public async UniTask<ApiResult<T>> GetActivities<T>(CancellationToken token = default)
    {
        // string planId = "36";
        // string childId = "98765432M";
        // string url = $"{UrlBase}{planId}/{childId}";
        //
        // byte[] rawResponse = await HttpClientUnity.GetAsyncBytes(url: url, cancellationToken: token);
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

    public string RootPrueba()
    {
        string json = @"
{
    ""success"": true,
    ""message"": {
        ""content"": [
            ""Activities generated""
        ],
        ""displayable"": true
    },
    ""data"": {
        ""general_data"": {
            ""audio_Intro"": ""audio_I.wav"",
            ""feedback_positive"": ""audio_FP.wav"",
            ""feedback_neutral"": ""audio_FN.wav"",
            ""feedback_no_answer"": ""audio_FNA.wav""
        },
        ""availability_info"": {
            ""words_available"": 10,
            ""questions_possible"": 10,
            ""therapy_plan_requested"": 5,
            ""words_per_question"": 2,
            ""sufficient_words"": true,
            ""activity_number"": 1,
            ""activities_created"": 5,
            ""activities_requested"": 5
        },
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
        return json;
    }
}
