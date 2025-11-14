using System.Collections;
using UnityEngine;
using static Level;

public class PortraitLevelMapGenerator : MonoBehaviour
{
    [Header("API var")]
    public string ChildId = "1805359203";

    [Header("Levels")]
    public RectTransform LevelContainer;
    public GameObject LevelPrefab;

    [Header("Level Placement")]
    public float LeftX = 300;
    public float RightX = 750;
    public float StartY = -300;
    public float StepY = -500;

    [Header("Button Overlays")]
    public LevelData SeaData;
    public LevelData PartyData;
    public LevelData BasketData;
    public LevelData RiverData;

    private readonly string[] scenes = new string[]
    {
        "BalloonPopSeaScene",
        "BalloonPopParty",
        "CrossTheRiverScene",
    };

    private int levelCount = 0;

    private void Start()
    {
        StartCoroutine(LoadMapData());
    }

    IEnumerator LoadMapData()
    {
        TherapyAPI api = gameObject.AddComponent<TherapyAPI>();

        yield return StartCoroutine(
            api.LoadTherapyPlans(
                ChildId,
                (plans) =>
                {
                    if (plans != null && plans.Count > 0)
                    {
                        levelCount = plans.Count;
                        Debug.Log($"✅ Child {ChildId} has {levelCount} therapy plans.");

                        // foreach (var plan in plans)
                        //     Debug.Log($"Template ID: {plan.Name}");
                    }
                    else
                    {
                        Debug.LogWarning("⚠️ No plans found — using default 12 levels.");
                        levelCount = 12;
                    }

                    GenerateMap();
                }
            )
        );
    }

    private void GenerateMap()
    {
        int lastCompleted = GamePrefs.LastCompletedLevel;

        for (int i = 0; i < levelCount; i++)
        {
            float x = (i % 2 == 0) ? RightX : LeftX;
            float y = StartY + (i * StepY);
            Vector2 position = new(x, y);

            GameObject levelObject = Instantiate(LevelPrefab, LevelContainer);
            levelObject.GetComponent<RectTransform>().anchoredPosition = position;

            Level levelComponent = levelObject.GetComponent<Level>();

            bool isLocked = i > lastCompleted;

            string sceneName = scenes[i % scenes.Length];
            LevelData levelData = sceneName switch
            {
                "BasketScene" => BasketData,
                "BalloonPopSeaScene" => SeaData,
                "BalloonPopParty" => PartyData,
                "CrossTheRiverScene" => RiverData,
                _ => BasketData,
            };

            LevelType levelType = i % 2 == 0 ? LevelType.Syllable : LevelType.Phoneme;

            levelComponent.Init(
                levelData: levelData,
                number: i + 1,
                locked: isLocked,
                levelType: levelType
            );
        }
    }
}
