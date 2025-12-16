using BasketResponses;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum HoopType
{
    Positive,
    Negative,
}

public class BasketManager : MonoBehaviour
{
    public static BasketManager Instance;

    [Space(15)]
    public BallTest Ball;

    [Space(15)]
    public Hoop HoopPositive;
    public Hoop HoopNegative;

    [Space(15)]
    public SpriteRenderer ImageLeftWord;
    public SpriteRenderer ImageRightWord;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        LoadActivities().Forget();
    }

    public async UniTaskVoid LoadActivities()
    {
        BasketService basketService = new();

        ApiResult<ActivitiesData> result = await basketService.GetActivities<ActivitiesData>();

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        BasketResponses.Activity[] activities = result.Data.Activities;

        string word1Path = activities[0].Words[0].Path;
        string word2Path = activities[0].Words[1].Path;

        Debug.Log(
            $"Successfully loaded Judge activity: {word1Path} vs {word2Path}"
        );

        ImageLeftWord.sprite = LoadSprite(word1Path);
        ImageRightWord.sprite = LoadSprite(word2Path);
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"No se encontró sprite: {file}");

        return s;
    }

    public void LaunchBall(HoopType hoopType)
    {
        Ball.TargetPosition =
            hoopType == HoopType.Positive
                ? HoopPositive.TargetPosition
                : HoopNegative.TargetPosition;

        Ball.Launch();
    }
}
