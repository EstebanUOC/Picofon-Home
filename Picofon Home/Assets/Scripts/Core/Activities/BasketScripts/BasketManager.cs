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

        var activities = await basketService.GetActivities();

        Debug.Log(
            $"Successfully loaded Judge activity: {activities.Activity1.Word1.Path} vs {activities.Activity1.Word2.Path}"
        );

        // Provisional code to load images
        ImageLeftWord.sprite = LoadSprite(activities.Activity1.Word1.Path);
        ImageRightWord.sprite = LoadSprite(activities.Activity1.Word2.Path);
    }

    private Sprite LoadSprite(string p)
    {
        string file = System.IO.Path.GetFileNameWithoutExtension(p);
        Sprite s = Resources.Load<Sprite>($"Images/ImgButtons/{file}");

        if (!s)
            Debug.LogWarning($"⚠ No se encontró sprite: {file}");

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
