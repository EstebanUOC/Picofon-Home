using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        bool existsData = LevelDataStore.ExistsPlans();
        if (existsData)
        {
            List<TherapyPlan> plans = LevelDataStore.Instance.GetAllPlans();
            GenerateMap(plans);
            return;
        }
        StartCoroutine(LoadMapData());
    }

    IEnumerator LoadMapData()
    {
        TherapyAPI api = gameObject.AddComponent<TherapyAPI>();
        LevelDataStore store = LevelDataStore.Instance;

        yield return StartCoroutine(
            api.LoadTherapyPlans(
                ChildId,
                (plans) =>
                {
                    if (plans is null)
                    {
                        Debug.LogError("❌ Failed to load therapy plans.");
                        return;
                    }

                    foreach (var plan in plans)
                    {
                        store.RegisterPlan(plan);
                        Debug.Log($" plan to be registered {plan}");
                    }

                    GenerateMap(plans);
                }
            )
        );
    }

    private void GenerateMap(List<TherapyPlan> plans)
    {
        int lastCompleted = GamePrefs.LastCompletedLevel;
        Debug.Log($" MAP: GamePrefs.LastCompletedLevel {lastCompleted}");
        for (int i = 0; i < plans.Count; i++)
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
                "BalloonPopSeaScene" => SeaData,
                "BalloonPopParty" => PartyData,
                "CrossTheRiverScene" => RiverData,
                _ => BasketData,
            };

            LevelType levelType = i % 2 == 0 ? LevelType.Syllable : LevelType.Phoneme;
            int planId = plans[i].Id;

            levelComponent.Init(
                levelData: levelData,
                number: i + 1,
                locked: isLocked,
                levelType: levelType,
                onClick: () => OnSelectLevel(planId: planId, sceneName: sceneName)
            );
        }
    }

    private void OnSelectLevel(int planId, string sceneName)
    {
        LevelPayload.PlanId = planId;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Debug.Log($" LevelPayload.PlanId {LevelPayload.PlanId}");
    }
}
