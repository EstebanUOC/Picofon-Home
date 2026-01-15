using UnityEngine;

public enum LevelScene
{
    BasketScene,
    BalloonPopSeaScene,
    BalloonPopParty,
    CrossRiverScene,
}

public class LevelItemRenderer : MonoBehaviour
{
    public LevelScene scene;

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
        "BasketScene",
        "BalloonPopSeaScene",
        "BalloonPopParty",
        "CrossRiverScene",
    };

    public void RenderLevels(TherapyPlan[] plans)
    {
        int lastCompleted = GamePrefs.LastCompletedLevel;

        for (int i = 0; i < plans.Length; i++)
        {
            float x = (i % 2 == 0) ? RightX : LeftX;
            float y = StartY + (i * StepY);
            Vector2 position = new(x, y);

            GameObject levelObject = Instantiate(LevelPrefab, LevelContainer);
            levelObject.GetComponent<RectTransform>().anchoredPosition = position;

            LevelItemView levelComponent = levelObject.GetComponent<LevelItemView>();

            bool isLocked = i > lastCompleted;

            string sceneName = scenes[(int)scene];

            LevelData data = sceneName switch
            {
                "BalloonPopSeaScene" => SeaData,
                "BalloonPopParty" => PartyData,
                "CrossRiverScene" => RiverData,
                _ => BasketData,
            };

            LevelType type = i % 2 == 0 ? LevelType.Syllable : LevelType.Phoneme;

            LevelState state = isLocked ? LevelState.Locked : LevelState.Unlocked;

            levelComponent.Init(data, state, type);
        }
    }

    private void OnSelectLevel(int planId, string sceneName)
    {
        LevelPayload.PlanId = planId;
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        Debug.Log($" LevelPayload.PlanId {LevelPayload.PlanId}");
    }
}
