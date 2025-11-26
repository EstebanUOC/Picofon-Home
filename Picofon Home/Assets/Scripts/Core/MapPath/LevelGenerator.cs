using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Level;

public class PortraitLevelMapGenerator : MonoBehaviour
{
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
       // Modify here depen of what GameMechanic you want to render.
       "CrossRiverScene",
       "CrossRiverScene",
       "CrossRiverScene",

        // this is the normal order.
        //"BalloonPopSeaScene",
        //"BalloonPopParty",
        //"CrossRiverScene",
    };

    private string ChildId = string.Empty;

    public void Start()
    {
        // TESTING: Hardcode a specific child ID
         ChildId = "19013454K"; // ← Add this line

        bool existsData = LevelDataStore.ExistsPlans();
        if (existsData)
        {
            List<TherapyPlan> plans = LevelDataStore.Instance.GetAllPlans();
            GenerateMap(plans);
            return;
        }
        // Comment out or remove this line since we're hardcoding above
        //ChildId = MapPathPayload.ChildId;

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

        // Print all plan IDs obtained from API
        Debug.Log("📋 All Plan IDs obtained from API:");

        for (int i = 0; i < plans.Count; i++)
        {
            Debug.Log($"   Level {i + 1}: Plan ID = {plans[i].Id}, Scene = {scenes[i % scenes.Length]}");
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
                "CrossRiverScene" => RiverData,
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
            // Print individual level creation info
            Debug.Log($"🎮 Created Level {i + 1}: Plan ID = {planId}, Scene = {sceneName}, Position = {position}, Locked = {isLocked}");
        }
        Debug.Log($"✅ Total levels generated: {plans.Count}");
    }

    private void OnSelectLevel(int planId, string sceneName)
    {
        LevelPayload.PlanId = planId;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Debug.Log($" LevelPayload.PlanId {LevelPayload.PlanId}");
    }
}