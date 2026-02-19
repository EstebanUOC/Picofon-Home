using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum ActivityType : byte
{
    Judge = 1,
    Select = 2,
    Relate = 3,
}

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space]
    [SerializeField]
    private LevelItemManager _itemManager;

    public void Start()
    {
        string childId = MapPathPayload.ChildId;

        if (string.IsNullOrEmpty(childId))
        {
#if !DEBUG
            Debug.LogError("ChildId is null or empty in MapPathPayload.");
            return;
# else
            childId = "19013454K";
            Debug.LogWarning("Using default ChildId for testing in Unity Editor.");
# endif
        }

        LoadPlans(childId).Forget();
    }

    public void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleLevelSelected;
    }

    public void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleLevelSelected;
    }

    private async UniTaskVoid LoadPlans(string childId)
    {
        LevelDataStore instance = LevelDataStore.Instance;

        bool existsData = instance.HasPlans();

        int levelCount;

        if (existsData)
        {
            levelCount = instance.GetPlansCount();
        }
        else
        {
            await GetPlans(childId);

            levelCount = instance.GetPlansCount();
        }

        _itemManager.RenderLevels(
            count: levelCount,
            last: instance.LastLevel,
            current: instance.CurrentLevel
        );
    }

    private async UniTask GetPlans(string childId)
    {
        TherapyPlanService service = new();
        CancellationToken token = this.GetCancellationTokenOnDestroy();

        ApiResult<TherapyData> result = await service.GetAllPlans<TherapyData>(childId, token);

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            Debug.LogError("No therapy plans found for the child.");
            return;
        }

        LevelDataStore.Instance.SavePlans(plans);
    }

    private void HandleLevelSelected(LevelConfig config, int index)
    {
        TherapyPlan plan = LevelDataStore.Instance.GetPlanByIndex(index);

        ActivityRequestParams @params = new()
        {
            PlanId = plan.TherapyPlanId,
            ChildId = plan.ChildId,
        };

        TherapyTemplate template = plan.TherapyTemplate;

        LevelPayload.Params = @params;
        LevelPayload.Skill = (ActivitySkill)template.Skill.Id;

        ActivityType type = (ActivityType)template.TaskType.Id;

        string suffix = type switch
        {
            ActivityType.Judge => "J",
            ActivityType.Select => "S",
            ActivityType.Relate => "R",
            _ => "",
        };

        string scene = $"{config.SceneName}_{suffix}";

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }
}
