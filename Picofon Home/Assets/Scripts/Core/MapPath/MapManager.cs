using System;
using System.Text.Json.Serialization;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum ActivityType : byte
{
    Judge = 1,
    Select = 2,
    Relate = 3,
}

public sealed class TherapyData
{
    [JsonInclude]
    public TherapyPlan[] Plans;
}

public class MapManager : MonoBehaviour
{
    [SerializeField]
    private LevelSelectEventChannel _eventChannel;

    [Space(15)]
    [SerializeField]
    private LevelItemRenderer _renderer;

    private string _childId = string.Empty;

    public void Start()
    {
        bool existsData = LevelDataStore.Instance.HasPlans();
        if (existsData)
        {
            int count = LevelDataStore.Instance.GetPlansCount();
            _renderer.RenderLevels(count);
            return;
        }

        _childId = MapPathPayload.ChildId;

        if (string.IsNullOrEmpty(_childId))
        {
#if !UNITY_EDITOR
            Debug.LogError("ChildId is null or empty in MapPathPayload.");
            return;
# else
            _childId = "19013454K";
            Debug.LogWarning("Using default ChildId for testing in Unity Editor.");
# endif
        }

        if (!GamePrefs.DebugMode)
        {
            LoadPlans().Forget();
            return;
        }

        _renderer.RenderLevels(5);
    }

    public void OnEnable()
    {
        _eventChannel.OnEventRaised += HandleLevelSelected;
    }

    public void OnDestroy()
    {
        _eventChannel.OnEventRaised -= HandleLevelSelected;
    }

    private async UniTaskVoid LoadPlans()
    {
        TherapyPlanService service = new();
        CancellationTokenSource token = new();
        token.CancelAfterSlim(TimeSpan.FromSeconds(5));

        ApiResult<TherapyData> result = await service.GetAllPlans<TherapyData>(_childId);

        if (!result.Success)
        {
            Debug.LogError($"Error loading activities: {result.Message}");
            return;
        }

        TherapyPlan[] plans = result.Data.Plans;

        if (plans is null || plans.Length == 0)
        {
            Debug.LogError("No hay actividades cargadas.");
            return;
        }

        LevelDataStore store = LevelDataStore.Instance;
        store.SavePlans(plans);

        _renderer.RenderLevels(plans.Length);
    }

    private void HandleLevelSelected(LevelConfig config, int index)
    {
        TherapyPlan plan = LevelDataStore.Instance.GetPlanByIndex(index);

        ActivityRequestParams @params = new()
        {
            PlanId = plan.TherapyPlanId,
            ChildId = plan.ChildId,
        };

        LevelPayload.Params = @params;
        LevelPayload.Skill = (ActivitySkill)plan.TherapyTemplate.Skill.Id;

        TherapyTemplate template = plan.TherapyTemplate;
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
